import numpy as np
from ddpg_tf2 import Agent
#from utils import plot_learning_curve
import tcpseverclass
import time
import tensorflow as tf
from logger import Logger

if __name__ == '__main__':
    
    action_count = 1
    input_count = 4


    print("Num GPUs Available: ", len(tf.config.list_physical_devices('GPU')))

    server = tcpseverclass.Server()

    agent = Agent(input_dims=(input_count,), n_actions=action_count, action_space=(-1.,1.))

    target_step_time = 0.005

    logging = Logger("17thLog.txt")
    

    best_score = -10000000000
    avg_rewards = []
    max_rewards = [] 
    min_rewards = []
    scores = []

    load_checkpoint = False
    save = True
    evaluate = False

    server.listen()
    print("Waiting for client connection.")
    while True:
        if server.connected:
            break
    #fix this part
    if load_checkpoint:
        print("Checkpoint loading initiated")

        n_steps = 0
        server.send({"startPermission":True, "torques":[0]*action_count})
        telemetry = server.wait_for_response()
        observation = telemetry["parameters"]
        reward = telemetry["reward"]
        done = telemetry["done"]
        while n_steps <= agent.batch_size:
            action = (np.random.rand(1,action_count)-0.5)*2
            action_value = action.tolist()[0]
            server.send({"startPermission":True, "torques":action_value})
            telemetry = server.wait_for_response()
            observation_ = telemetry["parameters"]
            reward = telemetry["reward"]
            done = telemetry["done"]
            agent.remember(observation, action, reward, observation_, done)
            observation = observation_
            n_steps += 1
        evaluate = False
        agent.learn()
        agent.load_models()
    else:
        evaluate = False
        logging.first_line(["start_time", "i", "avg_reward", "min_reward", "max_reward", "update_per_second", "episode_time", "update_count", "score"])

    score_calculated = False
    i = 0

    max_noise = 0.15
    min_noise = 0.02
    noise = max_noise
    total_update = 0

    print("Checkpoint loading finished. Continuing the training")

    while True:
        
        #server.send({"startPermission":True, "torques":[0]*action_count})
        server.send({"reset":False, "force":0})
        telemetry = server.wait_for_response()
        #observation = telemetry["parameters"]
        observation = [telemetry["position"],telemetry["baseSpeed"],telemetry["stickAngle"],telemetry["stickSpeed"] ]
        old_time = telemetry["date"]
        done = telemetry["done"]
       
        
        random_exploration_episodes = 0
        '''
        #https://www.desmos.com/calculator/aumynlbgoe
        if i >= 100 and not load_checkpoint:
            if not score_calculated:
                avg_score = np.mean(scores)
                score_calculate = True

            current_max = max_noise - max_noise*(total_update/1000000)
            if current_max > min_noise:
                noise_coeff = current_max - min_noise
                noise = current_max - ((score - avg_score)/-avg_score) * noise_coeff

                if noise < min_noise:
                    noise = min_noise
                if noise > current_max:
                    noise = current_max

            else:
                noise = min_noise

        else:
            noise = min_noise
        '''

        agent.noise = 0.1
        update_count = 0
        start_time = time.time()

        reward_list = []

        trained = False
        while not done:
            
            trained = True
            step_start_time = time.time()

            if i < random_exploration_episodes:
                action = (np.random.rand(1,action_count)-0.5)*2
                action_value = action.tolist()[0]
            elif i >= random_exploration_episodes:
                action = agent.choose_action(observation, evaluate)
                action_value = action.numpy().tolist() 

            """    
            if update_count < 20:
                server.send({"startPermission":True, "torques":action_value})
            else:
                server.send({"startPermission":False, "torques":action_value})
            """

            server.send({"reset":False, "force":action_value[0]*50})
            telemetry = server.wait_for_response()
            delta_time = telemetry["date"] - old_time
            old_time = telemetry["date"]
            #observation_ = telemetry["parameters"]
            observation_ = [telemetry["position"],telemetry["baseSpeed"],telemetry["stickAngle"],telemetry["stickSpeed"] ]
            reward = telemetry["reward"]
            done = telemetry["done"]

            reward_list.append(reward)

            agent.remember(observation, action, reward, observation_, done)
            observation = observation_
            
            update_count += 1

            sleep_time = target_step_time - (time.time() - step_start_time)
            if sleep_time > 0: 
                time.sleep(sleep_time)
            
        if trained:
            i+=1
            total_update += update_count
            episode_time = time.time() - start_time
            print(f"Agent stated to learn for {update_count} time steps. (Total Update: {total_update})")
            for _ in range(update_count):
                agent.learn()

            avg_reward = np.mean(reward_list)
            avg_rewards.append(avg_reward)

            max_reward = np.max(reward_list)
            max_rewards.append(max_rewards)

            min_reward = np.min(reward_list)
            min_rewards.append(min_rewards)
            
            score = np.sum(reward_list)
            scores.append(score)
            running_average_score = np.average(scores[-100:])

            if i > 100 and update_count>10 and running_average_score > best_score:
                best_score = running_average_score
                
                if save:
                    agent.save_models()
                    
            update_per_second = update_count / (episode_time)
            logging.log([start_time, i, avg_reward, min_reward, max_reward, update_per_second, episode_time, update_count, score])
            print('episode ', i, 'average rewards %.1f' % avg_reward, 'min rew %.1f' % min_reward,'max rew %.1f' % max_reward, 'score %.1f' % score, "ups %.1f" % update_per_second, "best %.1f" % best_score, "noise:", noise)
            
