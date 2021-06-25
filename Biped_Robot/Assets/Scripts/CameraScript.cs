using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour{
    public GameObject baseObject;      
    private Vector3 offset;

    void Start(){
        offset = transform.position - baseObject.transform.position;
    }

    
    void Update(){
        transform.position = baseObject.transform.position + offset;
        if (transform.position.y < 1) {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }
    }
}
