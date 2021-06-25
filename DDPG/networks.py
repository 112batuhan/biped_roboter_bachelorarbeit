import os
import tensorflow as tf
import tensorflow.keras as keras
from tensorflow.keras.layers import Dense, GaussianNoise

class CriticNetwork(keras.Model):
    def __init__(self, num_nodes=500, num_layers=2, n_actions=2,
            name='critic', chkpt_dir='tmp\ddpg'):
        super(CriticNetwork, self).__init__()
        
        self.num_layers = num_layers

        self.model_name = name
        dirname = os.path.dirname(__file__)
        self.checkpoint_dir = chkpt_dir
        self.checkpoint_file = os.path.join(dirname,self.checkpoint_dir, 
                    self.model_name+'_ddpg.h5')

        self.dense_layers = []
        for _ in range(num_layers):
            self.dense_layers.append(Dense(num_nodes, activation="relu"))


        self.q = Dense(1, activation=None)

    def call(self, state, action):

        action_value = self.dense_layers[0](tf.concat([state, action], axis=1))
        for i in range(1,self.num_layers):
            action_value = self.dense_layers[i](action_value)

        q = self.q(action_value)

        return q

class ActorNetwork(keras.Model):
    def __init__(self, num_nodes=500, num_layers=2, n_actions=2, name='actor',
            chkpt_dir='tmp\ddpg'):
        super(ActorNetwork, self).__init__()
        self.num_layers = num_layers
        self.n_actions = n_actions

        self.model_name = name
        dirname = os.path.dirname(__file__)
        self.checkpoint_dir = chkpt_dir
        self.checkpoint_file = os.path.join(dirname,self.checkpoint_dir, 
                    self.model_name+'_ddpg.h5')

        self.dense_layers = []
        for _ in range(num_layers):
            self.dense_layers.append(Dense(num_nodes, activation="relu"))

        self.mu = Dense(self.n_actions, activation="tanh")

    def call(self, state):

        prob = self.dense_layers[0](state)

        for i in range(1,self.num_layers):
            prob = self.dense_layers[i](prob)

        mu = self.mu(prob)

        return mu