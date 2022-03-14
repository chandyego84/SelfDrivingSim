using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // inputs
    private float m_horizontalInput;
    private float m_verticalInput;
    private float m_steeringAngle;

    // position of wheel colldiers
    public WheelCollider frontDriverW, frontPassengerW;
    public WheelCollider rearDriverW, rearPassengerW;
    public Transform frontDriverT, frontPassengerT;
    public Transform rearDriverT, rearPassengerT;
    public float maxSteerAngle = 30;
    public float motorForce = 500;

    public void GetInput() {
        m_horizontalInput = Input.GetAxis("Horizontal");
        m_verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer() {
        m_steeringAngle = maxSteerAngle * m_horizontalInput;
        frontDriverW.steerAngle = m_steeringAngle;
        frontPassengerW.steerAngle = m_steeringAngle;
    }

    private void Accelerate() {
        frontDriverW.motorTorque = m_verticalInput * motorForce;
        frontPassengerW.motorTorque = m_verticalInput * motorForce;
    }

    // update position of wheels and rotation
    private void UpdateWheelPoses() {
        UpdateWheelPose(frontDriverW, frontDriverT);
        UpdateWheelPose(frontPassengerW, frontPassengerT);
        UpdateWheelPose(rearDriverW, rearDriverT);
        UpdateWheelPose(rearPassengerW, rearPassengerT);
    }

    // helper for updating wheelposes
    private void UpdateWheelPose(WheelCollider _collider, Transform _transform) {
        // init position and rotation vals
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        // changes values to vector3 and quaternion
        _collider.GetWorldPose(out _pos, out _quat);

        // update position and rotation
        _transform.position = _pos;
        _transform.rotation = _quat;
    }

    private void FixedUpdate() {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

}
