using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterHandler : MonoBehaviour{

    public ResetScene reseter;

    public Component[] configurableJoints;
    public Rigidbody baseRigidbody;
    List<float> parameters = new List<float>();
    List<Rigidbody> rigidBodies = new List<Rigidbody>();
    public ContactForce contactForceLeg1;
    public ContactForce contactForceLeg2;

    public float[] parameterArray;


    float LogisticFunction(float x, float k, float l, float a) {
        //k steepnes, l max, x value
        return l / (1 + a*Mathf.Exp(-x * k));
    }

    float scale(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax) {
        return (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin;
    }

    void Start(){
        configurableJoints = GetComponentsInChildren(typeof(ConfigurableJoint));
        baseRigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        foreach (ConfigurableJoint joint in configurableJoints) {
            rigidBodies.Add(joint.gameObject.GetComponent<Rigidbody>());
        }

        UpdateParameters();
    }

    void FixedUpdate() {

        if(reseter.continuePermission){
            UpdateParameters();
        }
    }

    void UpdateParameters() {
        parameters.Clear();

        int k = 0;
        foreach (ConfigurableJoint joint in configurableJoints) {

            Vector3 localangularvelocity = joint.transform.InverseTransformDirection(rigidBodies[k++].angularVelocity);
            if (joint.angularXMotion != ConfigurableJointMotion.Locked) {
                float NormalisedXAngle = scale(joint.transform.eulerAngles.x, 0, 360, 0, 1);
                parameters.Add(NormalisedXAngle);
                parameters.Add(localangularvelocity.x);
            }
            if (joint.angularYMotion != ConfigurableJointMotion.Locked) {
                float NormalisedYAngle = scale(joint.transform.eulerAngles.y, 0, 360, 0, 1);
                parameters.Add(NormalisedYAngle);
                parameters.Add(localangularvelocity.y);
            }
            if (joint.angularZMotion != ConfigurableJointMotion.Locked) {
                float NormalisedZAngle = scale(joint.transform.eulerAngles.z, 0, 360, 0, 1);
                parameters.Add(NormalisedZAngle);
                parameters.Add(localangularvelocity.z);
            }
        }
        float scaledConcact1 = LogisticFunction(contactForceLeg1.contactForce, 0.05f, 2, 1) - 1;
        float scaledConcact2 = LogisticFunction(contactForceLeg2.contactForce, 0.05f, 2, 1) - 1;
        parameters.Add(scaledConcact1);
        parameters.Add(scaledConcact2);

        for (int i = 0; i < 3; i++) {
            float scaledIPos = LogisticFunction(baseRigidbody.position[i], 0.5f, 2, 1) - 1;
            parameters.Add(scaledIPos);
            parameters.Add(baseRigidbody.velocity[i]);
            float NormalisedIAngle = scale(baseRigidbody.transform.eulerAngles[i], 0, 360, 0, 1);
            parameters.Add(NormalisedIAngle);
            parameters.Add(baseRigidbody.angularVelocity[i]);

        }
        parameterArray = parameters.ToArray();
    }

}
