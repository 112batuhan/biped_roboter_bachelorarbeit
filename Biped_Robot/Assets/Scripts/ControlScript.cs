using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour {

    public Component[] configurableJoints;
    public int configurableAxisCount;
    public int configurableForceCount;
    public Vector3 targetRotationVector;

    void Start(){

        configurableJoints = GetComponentsInChildren(typeof(ConfigurableJoint));
        configurableAxisCount = 0;
        foreach (ConfigurableJoint joint in configurableJoints) {

            if (joint.angularXMotion != ConfigurableJointMotion.Locked) {
                configurableAxisCount++;
                configurableForceCount++;
            }
            if (joint.angularYMotion != ConfigurableJointMotion.Locked) {
                configurableAxisCount++;
            }
            if (joint.angularZMotion != ConfigurableJointMotion.Locked) {
                configurableAxisCount++;
            }
            if (joint.angularYMotion != ConfigurableJointMotion.Locked || joint.angularZMotion != ConfigurableJointMotion.Locked) {
                configurableForceCount++;
            }
        }


    }   

    public void UpdateJoints(float[] values) {

      
        if (values.Length != configurableAxisCount + configurableForceCount) {
            throw new Exception("Value size doesn't match with robot actuator parameter count.");
        }

        int i = 0;
        foreach (ConfigurableJoint joint in configurableJoints) {

            targetRotationVector = new Vector3(0,0,0);
            

            if (joint.angularXMotion != ConfigurableJointMotion.Locked) {
                float x_high_limit = joint.highAngularXLimit.limit;
                float x_low_limit = joint.lowAngularXLimit.limit;

                targetRotationVector.x = scale(values[i++], -1, 1, x_low_limit, x_high_limit);
                JointDrive drive = new JointDrive();
                drive.maximumForce = (1 + values[i++]) * 200;
                drive.positionDamper = 200;
                drive.positionSpring = 500;
                joint.angularXDrive = drive;
            }
            if (joint.angularYMotion != ConfigurableJointMotion.Locked) {
                float y_limit = joint.angularYLimit.limit;
                targetRotationVector.y = y_limit * values[i++];
            }
            if (joint.angularZMotion != ConfigurableJointMotion.Locked) {
                float z_limit = joint.angularZLimit.limit;
                targetRotationVector.z = z_limit * values[i++];
            }
            if (joint.angularYMotion != ConfigurableJointMotion.Locked || joint.angularZMotion != ConfigurableJointMotion.Locked) {
                JointDrive drive = new JointDrive();
                drive.maximumForce = (1 + values[i++]) * 200;
                drive.positionDamper = 200;
                drive.positionSpring = 500;
                joint.angularYZDrive = drive;
            }
            joint.targetRotation = Quaternion.Euler(targetRotationVector);

        }
    }

    public float scale(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax) {
        return (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin;
    }


}
