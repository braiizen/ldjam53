using UnityEngine;
using UnityEngine.InputSystem;


public class CarController : MonoBehaviour
{
    [SerializeField] private float springStrength = 400f;
    [SerializeField] private float springDamper = 15f;
    [SerializeField] private float carTopSpeed = 50.0f;
    [SerializeField] private float motorForce = 100.0f;

    [SerializeField] private TireController frontLeftWheel;
    [SerializeField] private TireController frontRightWheel;
    [SerializeField] private TireController rearLeftWheel;
    [SerializeField] private TireController rearRightWheel;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float tireMass;
    [SerializeField] private float suspensionRayHeight = 1.5f;
    [SerializeField] private AnimationCurve accelCurve;
    [SerializeField] private AnimationCurve brakeCurve;
    [SerializeField] private float suspensionRestHeight = 0.5f;
    [SerializeField] private float boostMultiplier = 200f;
    [SerializeField] private float tireGripFactor = .05f;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float pitchFromCar;
    [SerializeField] private float minSpeed = .3f;
    [SerializeField] private float maxSpeed = 40f;

    private Vector3 resetPosition = new Vector3(0, 1.71f, 0);
    private Rigidbody carRigidBody;
    private float currentSpeed;
    private float horizontalInput;
    private float verticalInput;
    private bool boostInput;
    private AudioSource carAudio;

    private void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();
        carAudio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        currentSpeed = Vector3.Dot(carRigidBody.transform.forward, carRigidBody.velocity);

        if (IsUpsideDown())
        {
            carRigidBody.AddForceAtPosition(-Vector3.up * 100, frontLeftWheel.transform.position);
            carRigidBody.AddForceAtPosition(-Vector3.up * 100, rearLeftWheel.transform.position);
        }
        
        if (boostInput)
        {
            UseBoost();
        }
        
        UpdateSuspension();
        RotateWheels();
        HandleSteering();
        HandleMotor();
        EngineSound();
    }

    #region Driving
    private void HandleMotor()
    {
        if (verticalInput > 0.1f)
        {
            float motorInput = verticalInput * motorForce;
            CalculateAccelForce(frontLeftWheel.transform, motorInput);
            CalculateAccelForce(frontRightWheel.transform, motorInput);
        }
        else if (verticalInput < -0.1f)
        {
            float brakeInput = verticalInput * motorForce;
            CalculateBrakeForce(frontLeftWheel.transform, brakeInput);
            CalculateBrakeForce(frontRightWheel.transform, brakeInput);
        } else if (currentSpeed > .1f)
        {
            CalculateBrakeForce(frontLeftWheel.transform, -motorForce / 2);
            CalculateBrakeForce(frontRightWheel.transform, -motorForce / 2);
        } else if (currentSpeed < -.1f)
        {
            CalculateBrakeForce(frontLeftWheel.transform, motorForce / 2);
            CalculateBrakeForce(frontRightWheel.transform, motorForce / 2);
        }
    }

    private void CalculateAccelForce(Transform tireTransform, float accelForce, float topSpeedMultiplier = 1)
    {
        Vector3 accelDir = tireTransform.forward;
        
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / carTopSpeed * topSpeedMultiplier);
        float availableTorque = accelCurve.Evaluate(normalizedSpeed) * accelForce;
        
        carRigidBody.AddForceAtPosition(accelDir * availableTorque, tireTransform.position);
    }

    private void CalculateBrakeForce(Transform tireTransform, float brakeForce)
    {
        Vector3 brakeDir = tireTransform.forward;
        
        float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentSpeed) / carTopSpeed);
        float availableTorque = brakeCurve.Evaluate(normalizedSpeed) * brakeForce;

        carRigidBody.AddForceAtPosition(brakeDir * availableTorque, tireTransform.position);
    }

    private void RotateWheels()
    {
        float steerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheel.transform.localRotation = Quaternion.Euler(0f, steerAngle, 0f);
        frontRightWheel.transform.localRotation = Quaternion.Euler(0f, steerAngle, 0f);
    }

    private void HandleSteering()
    {
        CalculateSteeringForce(frontLeftWheel);
        CalculateSteeringForce(frontRightWheel);
        CalculateSteeringForce(rearLeftWheel);
        CalculateSteeringForce(rearRightWheel);
    }

    private void CalculateSteeringForce(TireController tire)
    {
        Vector3 steeringDir = tire.transform.right;
        Vector3 tireWorldVel = carRigidBody.GetPointVelocity(tire.transform.position);

        float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);
        float desiredVelChange = -steeringVel * tireGripFactor;

        if (desiredVelChange < -.2)
        {
            tire.StartEmitter();
        }
        else
        {
            tire.StopEmitter();
        }
        
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        
        carRigidBody.AddForceAtPosition(steeringDir * (tireMass * desiredAccel), tire.transform.position);
    }
    
    private void UpdateSuspension()
    {
        CalculateSuspensionForce(frontLeftWheel.transform);
        CalculateSuspensionForce(frontRightWheel.transform);
        CalculateSuspensionForce(rearLeftWheel.transform);
        CalculateSuspensionForce(rearRightWheel.transform);
    }

    private void CalculateSuspensionForce(Transform tireTransform)
    {
        RaycastHit hit;
        if (Physics.Raycast(tireTransform.position, -transform.up, out hit, suspensionRayHeight))
        {
            Vector3 springDir = tireTransform.up;
            Vector3 tirePosition = tireTransform.position;
            Vector3 tireWorldVel = carRigidBody.GetPointVelocity(tirePosition);

            float offset = suspensionRestHeight - hit.distance;

            float vel = Vector3.Dot(springDir, tireWorldVel);
            float force = (offset * springStrength) - (vel * springDamper);

            carRigidBody.AddForceAtPosition(springDir * force, tirePosition);
        }
    }
    #endregion

    public void UseBoost()
    {
        if (GameManager.instance.fuel > 0)
        {
            GameManager.instance.fuel -= Time.deltaTime * 10f;
            CalculateAccelForce(rearRightWheel.transform, boostMultiplier*motorForce, 2);
            CalculateAccelForce(rearLeftWheel.transform, boostMultiplier*motorForce, 2);
        }
    }

    public bool IsUpsideDown()
    {
        return Vector3.Dot(transform.up, Vector3.up) < 0.0f;
    }
    
    private void EngineSound()
    {
        var currentSpeed = carRigidBody.velocity.magnitude;
        pitchFromCar = currentSpeed / 50f;

        if (currentSpeed < minSpeed)
        {
            carAudio.pitch = minPitch;
        }

        if (currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            carAudio.pitch = minPitch + pitchFromCar;
        }

        if (currentSpeed > maxSpeed)
        {
            carAudio.pitch = maxPitch;
        }
    }

    public void ResetCar()
    {
        carTopSpeed = 30f;
        motorForce = 75f;
        tireGripFactor = .05f;
        boostMultiplier = 2;
        if (carRigidBody == null)
            carRigidBody = GetComponent<Rigidbody>();
        carRigidBody.velocity = Vector3.zero;
        transform.position = resetPosition;
    }

    public void UpgradeSpeed(float multiplier)
    {
        carTopSpeed *= multiplier;
    }

    public void UpgradeAcceleration(float multiplier)
    {
        motorForce *= multiplier;
    }

    public void UpgradeHandling(float multiplier)
    {
        tireGripFactor += .05f;
    }
    
    public void UpgradeBoost(float pickedCardMultiplier)
    {
        boostMultiplier *= pickedCardMultiplier;
    }
    
    public void OnMove(InputAction.CallbackContext value)
    {
        Vector2 inputVector = value.ReadValue<Vector2>();
        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;
    }

    public void OnFire(InputAction.CallbackContext value)
    {
        boostInput = value.ReadValueAsButton();
    }
    
    public float GetCurrentSpeed() => currentSpeed;

    
}
