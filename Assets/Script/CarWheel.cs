using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    public Vector3 wheelPosition = Vector3.zero;
    public WheelCollider TagetWheel;
    public Quaternion wheelRotation = Quaternion.identity;
    void Start()
    {
        
    }

 
    void Update()
    {
        TagetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;

    }
}
