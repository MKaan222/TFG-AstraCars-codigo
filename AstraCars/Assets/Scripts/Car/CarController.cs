using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    public float maxSpeed = 10;
    public float motorForce = 100f;
    public float brakeForce = 50f;
    public float maxSteeringAngle = 30f;

    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    public Transform frontLeftTransform;
    public Transform frontRightTransform;
    public Transform rearLeftTransform;
    public Transform rearRightTransform;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBraking;
    public PlayerData playerData;
    private float respawnCooldown = 0.5f;
    private float lastRespawnTime = 0f;


    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.5f, 0); // Prueba valores entre -0.5 y -1.5 en Y
    }

    private void Update()
    {


        if (playerData != null)
        {
            horizontalInput = InputManager.GetHorizontal(playerData);
            // Solo permitimos acelerar si hay gasolina
            verticalInput = playerData.GetGasolina() > 0 ? 1f : 0f;
            // Respawn
            if (InputManager.GetRespawn(playerData) && Time.time - lastRespawnTime > respawnCooldown)
            {
                RespawnToSpawnPoint();
                lastRespawnTime = Time.time;
            }
        }
        HandelMotor();
        HandelSteering();
        UpdateWheels();

    }



    private void HandelMotor()
    {

        if (GetComponent<Rigidbody>().velocity.magnitude > maxSpeed)
        {
            GetComponent<Rigidbody>().velocity = maxSpeed * GetComponent<Rigidbody>().velocity.normalized;
        }

        frontLeftWheel.motorTorque = verticalInput * motorForce;
        frontRightWheel.motorTorque = verticalInput * motorForce;

        currentBrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        frontLeftWheel.brakeTorque = currentBrakeForce;
        frontRightWheel.brakeTorque = currentBrakeForce;
        rearLeftWheel.brakeTorque = currentBrakeForce;
        rearRightWheel.brakeTorque = currentBrakeForce;
    }

    private void HandelSteering()
    {
        currentSteerAngle = maxSteeringAngle * horizontalInput;
        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;

    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftTransform);
        UpdateSingleWheel(frontRightWheel, frontRightTransform);
        UpdateSingleWheel(rearLeftWheel, rearLeftTransform);
        UpdateSingleWheel(rearRightWheel, rearRightTransform);
    }
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = newMaxSpeed;
    }
    public void SetMaxSteeringAngle(float newMaxSteeringAngle)
    {
        maxSteeringAngle = newMaxSteeringAngle;
    }

    private void RespawnToSpawnPoint()
    {
        if (playerData != null && playerData.lastRespawnPoint != null)
        {
            transform.position = playerData.lastRespawnPoint.position;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.rotation = playerData.lastRespawnPoint.rotation;
        }
    }

}