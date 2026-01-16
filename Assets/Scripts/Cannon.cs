using System;
using Unity.Mathematics;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public GameObject Pivot;
    public GameObject Barrel;

    public float rotationSpeed = 1.0f;
    public float MaxRotationRight = 80.0f;
    public float MaxRotationLeft = 0f;

    float currentRotationZ = 0f;

    void Start()
    {
        currentRotationZ = Pivot.transform.eulerAngles.z;
        if (currentRotationZ > 180f) currentRotationZ -= 360f;
    }

    void Update()
    {
        float pivotRotation = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pivotRotation = rotationSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pivotRotation = -rotationSpeed;
        }

        ClampRotation(pivotRotation);
    }


    void ClampRotation(float pivotRotation)
    {
        float newRotationZ = currentRotationZ + pivotRotation;

        newRotationZ = Mathf.Clamp(newRotationZ, MaxRotationLeft, MaxRotationRight);


        if (newRotationZ != currentRotationZ)
        {
            currentRotationZ = newRotationZ;
            Pivot.transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
        }

        Debug.Log("Current Rotation: " + currentRotationZ);
    }
}
