using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour{

    public Rigidbody BaseRb;
    public Transform BaseTF;
    public float reward;
    public Vector3 pos;
    public Vector3 rot;
    public float distanceToCenter;
    public float timeReward;

    public ResetScene reseter;

    float eulerDistance(float x, float y) {
        
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    }

    void Start() {
        timeReward = 0f;
    }

    void FixedUpdate() {
        pos = BaseTF.position;
        rot = BaseTF.eulerAngles;
        rot.x = Mathf.Abs(180 - Mathf.Abs(rot.x - 180));
        rot.y = Mathf.Abs(180 - Mathf.Abs(rot.y - 180));
        rot.z = Mathf.Abs(180 - Mathf.Abs(rot.z - 180));

        distanceToCenter = eulerDistance(BaseTF.position.x, BaseTF.position.z);
        
        if (reseter.done) {
            timeReward = 0f;
        }
        if (reseter.continuePermission) {
            timeReward += 0.1f * Time.deltaTime;
        }
        
        reward = BaseRb.velocity.z*5f + timeReward - Mathf.Pow(BaseTF.position.x, 2) * 3f - Mathf.Pow(5 - BaseTF.position.y, 3) *50f - rot.x * 0.1f + (180 - rot.y)*0.5f - rot.z * 0.1f;
           //- Mathf.Pow(rot.x, 2) * 0.3f - Mathf.Pow(rot.y, 2) * 0.3f - Mathf.Pow(rot.z, 2) * 0.3f;

    }
}
