using System;
using Unity.Mathematics;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject Pivot;
    [SerializeField] GameObject ShotPos;

    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float MaxRotationRight = 80.0f;
    [SerializeField] float MaxRotationLeft = 0f;
    [SerializeField] float shootForce;

    float currentRotationZ;
    GameManager GMref;

    void Start()
    {
        GMref = GameManager.Instance;
        currentRotationZ = Pivot.transform.eulerAngles.z;
        if (currentRotationZ > 180f) currentRotationZ -= 360f;
    }

    void Update()
    {
        float pivotRotation = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pivotRotation = rotationSpeed;
            ClampRotation(pivotRotation);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pivotRotation = -rotationSpeed;
            ClampRotation(pivotRotation);
        }


        if (Input.GetKeyDown(KeyCode.Space) && GMref.isShootingEnabled)
        {
            GMref.isShootingEnabled = false;
            Shoot();
        }
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

    void Shoot()
    {
        if (ShotPos == null)
        {
            Debug.LogError("ShotPos is not assigned!");
            return;
        }

        if (GMref.CheckAmmo() > 0)
        {
            GMref.UseAmmo();
            GameObject cannonBall = GMref.SpawnBall(ShotPos.transform.position);
            Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(ShotPos.transform.up * shootForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("CannonBall prefab is missing Rigidbody component!");
            }
        }
        else
        {
            Debug.Log("Cannot shoot: No ammo left!");
        }
    }
}
