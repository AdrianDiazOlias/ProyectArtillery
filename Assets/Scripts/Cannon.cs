using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Cannon Components")]
    [SerializeField] GameObject Pivot;
    [SerializeField] GameObject ShotPos;
    [SerializeField] ParticleSystem shootParticle;
    [SerializeField] LineRenderer trajectoryLine;

    [Header("Audio GameObjects")]
    [SerializeField] GameObject shootSoundGO;
    [SerializeField] GameObject PivotSoundGO;
    [SerializeField] GameObject ChargeSoundGO;

    [Header("Cannon Settings")]
    [SerializeField] float rotationSpeed = 1.0f;
    [SerializeField] float MaxRotationRight = 80.0f;
    [SerializeField] float MaxRotationLeft = 0f;
    [SerializeField] float shootForce = 20f;
    [SerializeField] float maxShootForce = 50f;
    [SerializeField] float chargeSpeed = 10f;
    [SerializeField] int trajectorySegments = 50;
    [SerializeField] float trajectoryTimeStep = 0.05f;

    float currentRotationZ;
    float currentShootForce;
    bool isCharging = false;
    float cachedBallMass = 1f;
    GameManager GMref;

    void Start()
    {
        GMref = GameManager.Instance;
        GameObject testBall = GMref.SpawnBall(transform.position);
        Rigidbody testRb = testBall.GetComponent<Rigidbody>();

        currentRotationZ = Pivot.transform.eulerAngles.z;
        if (currentRotationZ > 180f) currentRotationZ -= 360f;
        currentShootForce = shootForce;

        if (testRb != null)
        {
            cachedBallMass = testRb.mass;
        }
        Destroy(testBall);
    }

    void Update()
    {
        float pivotRotation = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            pivotRotation = rotationSpeed;
            ClampRotation(pivotRotation);

        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            pivotRotation = -rotationSpeed;
            ClampRotation(pivotRotation);
        }

        HandleCharging();
        UpdateTrajectoryLine();
    }


    void ClampRotation(float pivotRotation)
    {
        float newRotationZ = currentRotationZ + pivotRotation;

        newRotationZ = Mathf.Clamp(newRotationZ, MaxRotationLeft, MaxRotationRight);


        if (newRotationZ != currentRotationZ)
        {
            currentRotationZ = newRotationZ;
            Pivot.transform.rotation = Quaternion.Euler(0, 0, currentRotationZ);
            PlayRotatingSound();
        }

        Debug.Log("Current Rotation: " + currentRotationZ);
    }

    void HandleCharging()
    {
        if (GMref.CheckAmmo() > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) && GMref.isShootingEnabled)
            {
                isCharging = true;
                currentShootForce = shootForce;
            }
            else if (Input.GetKey(KeyCode.Space) && isCharging)
            {
                currentShootForce = Mathf.Min(currentShootForce + chargeSpeed * Time.deltaTime, maxShootForce);
                if (!ChargeSoundGO.GetComponent<AudioSource>().isPlaying && currentShootForce <= maxShootForce * 0.9f)
                {
                    ChargeSoundGO.GetComponent<AudioSource>().Play();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space) && isCharging)
            {
                isCharging = false;
                if (GMref.isShootingEnabled)
                {
                    GMref.isShootingEnabled = false;
                    Shoot();
                }
            }
        }
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
            shootParticle.Play();
            shootSoundGO.GetComponent<AudioSource>().Play();
            GMref.UseAmmo();
            GameObject cannonBall = GMref.SpawnBall(ShotPos.transform.position);
            Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(ShotPos.transform.up * currentShootForce, ForceMode.Impulse);
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

    void UpdateTrajectoryLine()
    {
        if (trajectoryLine == null || ShotPos == null)
            return;

        if (isCharging)
        {
            Vector3[] trajectoryPoints = CalculateTrajectory();
            trajectoryLine.positionCount = trajectoryPoints.Length;
            trajectoryLine.SetPositions(trajectoryPoints);
        }
        else
        {
            trajectoryLine.positionCount = 0;
        }
    }

    Vector3[] CalculateTrajectory()
    {
        Vector3[] points = new Vector3[trajectorySegments];
        Vector3 position = ShotPos.transform.position;
        Vector3 velocity = (ShotPos.transform.up * currentShootForce) / cachedBallMass;

        for (int i = 0; i < trajectorySegments; i++)
        {
            points[i] = position;
            velocity += Physics.gravity * trajectoryTimeStep;
            position += velocity * trajectoryTimeStep;
        }

        return points;
    }

    void PlayRotatingSound()
    {
        if (!PivotSoundGO.GetComponent<AudioSource>().isPlaying)
        {
            PivotSoundGO.GetComponent<AudioSource>().Play();
        }
    }
}
