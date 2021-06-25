using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactForce : MonoBehaviour
{

    public float contactForce;
    public Collider terrainCollider;

    void OnCollisionStay(Collision collisionInfo) {

        if (collisionInfo.collider == terrainCollider) {
            contactForce = collisionInfo.impulse.magnitude / Time.fixedDeltaTime;
        }

    }
     
    void OnCollisionExit(Collision collisionInfo) {

        if (collisionInfo.collider == terrainCollider) {
            contactForce = 0f;
        }
    }
}
