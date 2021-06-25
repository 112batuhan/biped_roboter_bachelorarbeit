using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensitySetter : MonoBehaviour{

    public Component[] rigidbodies;

    void Start(){

        rigidbodies = GetComponentsInChildren(typeof(Rigidbody));

        foreach (Rigidbody currentRigidbody in rigidbodies) {
            currentRigidbody.SetDensity(0.25f);
        }
        Rigidbody BaseRB = GetComponent(typeof(Rigidbody)) as Rigidbody;
        BaseRB.SetDensity(0.75f);
    }

}
