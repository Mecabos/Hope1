using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour {


    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp((new Vector3 (transform.position.x, transform.position.y, transform.position.z)), (new Vector3(Mathf.Clamp(desiredPosition.x, -17.0F, 30.0F), Mathf.Clamp(desiredPosition.y, 0.3F, 1.3F), transform.position.z)), smoothSpeed);
        transform.position = smoothedPosition;

        //transform.LookAt(target);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
