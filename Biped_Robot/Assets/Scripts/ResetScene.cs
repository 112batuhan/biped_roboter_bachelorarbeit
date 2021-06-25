using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScene : MonoBehaviour {

    public Component[] rigidbodies;
    public Component[] transforms;
    public bool done;
    public bool continuePermission;
    public bool runOnce;
    public Collider terrainCollider;
    public bool timerActive;
    public Reward rewarder;
    
    public class Timer {

        public bool active;

        public float startTime;
        public float timeSinceStart;

        public Timer() {
            startTime = Time.time;
            timeSinceStart = 0;
        }
        public void updateTimer() {
            timeSinceStart = Time.time - startTime;
        }
    }

    public Timer resetTimer;
    public Timer episodeTimer;

    public class ObjectStartValues {

        public Vector3 position;
        public Quaternion rotation;
    }
    public ObjectStartValues[] resetValues;

    void Start() {
        done = false;
        continuePermission = true;
        timerActive = false;
        runOnce = false;
        
        rigidbodies = GetComponentsInChildren(typeof(Rigidbody));
        transforms = GetComponentsInChildren(typeof(Transform));

        episodeTimer = new Timer();

        resetValues = new ObjectStartValues[rigidbodies.Length];
        int i = 0;
        foreach (Transform currentTransform in transforms) {

            resetValues[i] = new ObjectStartValues();
            resetValues[i].position = currentTransform.position;
            resetValues[i++].rotation = currentTransform.rotation;
        }

    }

    
    void FixedUpdate() {
        /* use this if you want to keep the robot alive after a certain height for some seconds
         * but it's really not usefull since you wouldn't want to have data in replay buffer after the fall is inevitable.
         * it makes the training slower and it's useless data for the algorithm
         * i'm also aware that done and continuePermission variables are redundant
        Transform baseTransform = transforms[0] as Transform;
        if (baseTransform.position.y < 3 && !timerActive) {
            resetTimer = new Timer();
            timerActive = true;
        }
        if (baseTransform.position.y >= 3) {
            timerActive = false;
        }
        if (timerActive) {
            resetTimer.updateTimer();
        }
        episodeTimer.updateTimer();
        if( episodeTimer.timeSinceStart > 20 || (timerActive && resetTimer.timeSinceStart > 0)) {
            done = true;
            continuePermission = false;
            timerActive = false;
        }
        */

        Transform baseTransform = transforms[0] as Transform;
        episodeTimer.updateTimer();
        if (episodeTimer.timeSinceStart > 20 || baseTransform.position.y < 3) {
            done = true;
            continuePermission = false;
            
        }

        if (!continuePermission) {
            Reset();
            runOnce = true;
            episodeTimer = new Timer();
        } 
        else if (runOnce) {
            OnContinuation();
            runOnce = false;
        }
    }

    void OnCollisionEnter(Collision collisionInfo) {

        if (collisionInfo.collider == terrainCollider) {
            continuePermission = false;
            done = true;
            rewarder.reward -= 1000f;
        }

    }

    public void Reset() {
        int i = 0;
        foreach (Transform currentTransform in transforms) {
            currentTransform.position = resetValues[i].position;
            currentTransform.rotation = resetValues[i++].rotation;
            
        }
        foreach (Rigidbody currentRigidbody in rigidbodies) {
            currentRigidbody.velocity = Vector3.zero;
            currentRigidbody.angularVelocity = Vector3.zero;
        }

    }

    public void OnContinuation() {
        foreach (Transform currentTransform in transforms) {
            currentTransform.rotation *= Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }
        transform.position = new Vector3(Random.Range(-5f, 5f), 5.2f, Random.Range(-5f, 5f));
        transform.rotation *= Quaternion.Euler(0, Random.Range(0f,360f), 0);
    }

}


