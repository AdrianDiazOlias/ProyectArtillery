using UnityEngine;
using UnityEngine.InputSystem;



public class Cannon : MonoBehaviour

{
    [Header("Cannon Components")]
    [SerializeField] Transform pivot;
    [SerializeField] Transform shotPos;
    [SerializeField] ParticleSystem shootParticle;
    [SerializeField] LineRenderer trajectoryLine;
    [SerializeField] GameObject ballPrefab;

    [Header("Audio Sources")]
    [SerializeField] AudioSource shootAudio;
    [SerializeField] AudioSource pivotAudio;
    [SerializeField] AudioSource chargeAudio;

    [Header("Cannon Settings")]
    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] float maxRotationRight = 80f;
    [SerializeField] float maxRotationLeft = 0f;

    [Header("Shooting")]
    [SerializeField] float baseImpulse = 20f;
    [SerializeField] float maxImpulse = 50f;
    [SerializeField] float chargeSpeed = 30f;

    [Header("Trajectory")]
    [SerializeField] int trajectorySegments = 50;
    [SerializeField] float trajectoryTimeStep = 0.05f;

    GameManager GMref;

    float currentRotationZ;
    float currentImpulse;
    bool isCharging;
    float cachedBallMass;

    GameControls controls;
    InputAction ShootAction;
    InputAction AimAction;

    void Awake()
    {
        controls = new GameControls();
    }

    void OnEnable()
    {
        controls.Cannon.Enable();

        ShootAction = controls.Cannon.Shoot;
        AimAction = controls.Cannon.Aim;
    }

    void OnDisable()
    {
        if (controls != null)
            controls.Cannon.Disable();
    }

    void Start()

    {

        GMref = GameManager.Instance;

        currentRotationZ = pivot.eulerAngles.z;
        if (currentRotationZ > 180f) currentRotationZ -= 360f;

        currentImpulse = baseImpulse;

        Rigidbody rb = ballPrefab.GetComponent<Rigidbody>();
        cachedBallMass = rb.mass;

    }



    void Update()
    {
        HandleRotation();
        HandleCharging();
        UpdateTrajectoryLine();
    }



    void HandleRotation()
    {
        float input = 0f;

        if (AimAction != null)
        {
            input = AimAction.ReadValue<float>();
        }

        if (Mathf.Approximately(input, 0f)) return;

        float delta = input * rotationSpeed * Time.deltaTime;
        float newRotation = Mathf.Clamp(currentRotationZ + delta, maxRotationLeft, maxRotationRight);

        if (!Mathf.Approximately(newRotation, currentRotationZ))
        {
            currentRotationZ = newRotation;
            pivot.rotation = Quaternion.Euler(0f, 0f, currentRotationZ);

            if (!pivotAudio.isPlaying) pivotAudio.Play();
        }
    }

    void HandleCharging()
    {
        if (GMref.CheckAmmo() <= 0) return;
        if (ShootAction == null) return;

        if (!isCharging && ShootAction.WasPressedThisFrame() && GMref.isShootingEnabled)
        {
            isCharging = true;
            currentImpulse = baseImpulse;
        }
        else if (isCharging && ShootAction.IsPressed())
        {
            currentImpulse = Mathf.Min(currentImpulse + chargeSpeed * Time.deltaTime, maxImpulse);
            if (!chargeAudio.isPlaying && currentImpulse <= maxImpulse * 0.9f)
                chargeAudio.Play();
        }
        else if (isCharging && ShootAction.WasReleasedThisFrame())
        {
            isCharging = false;
            if (GMref.isShootingEnabled)
            {
                GMref.isShootingEnabled = false;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (shotPos == null) return;

        if (GMref.CheckAmmo() <= 0) return;

        shootParticle.Play();
        shootAudio.Play();
        GMref.UseAmmo();

        GameObject cannonBall = Instantiate(ballPrefab, shotPos.position, Quaternion.identity);
        Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
        rb.AddForce(shotPos.up * currentImpulse, ForceMode.Impulse);
    }

    void UpdateTrajectoryLine()
    {
        if (!isCharging || trajectoryLine == null)
        {
            trajectoryLine.positionCount = 0;
            return;
        }

        Vector3[] points = CalculateTrajectory();
        trajectoryLine.positionCount = points.Length;
        trajectoryLine.SetPositions(points);
    }

    Vector3[] CalculateTrajectory()
    {
        Vector3[] points = new Vector3[trajectorySegments];
        Vector3 startPos = shotPos.position;
        Vector3 initialVelocity = (shotPos.up * currentImpulse) / cachedBallMass;
        Vector3 gravity = Physics.gravity;

        for (int i = 0; i < trajectorySegments; i++)
        {
            float t = i * trajectoryTimeStep;

            Vector3 position = startPos + initialVelocity * t + 0.5f * gravity * t * t;
            points[i] = position;
        }

        return points;
    }
}