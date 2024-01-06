using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity
{
    public class PlayerController : MonoBehaviour
    {
        public enum MovementSpeedMode
        {
            Walk, Run, Slow
        }

        private Transform m_CameraTransform = null;
        private Transform m_Transform = null;
        private CapsuleCollider m_CapsuleCollider = null;
        private Rigidbody m_Rigidbody = null;
        private InputActionMap m_MovementActionMap = null;
        private Vector2 m_LeftStickValue;
        private Vector2 m_RightStickValue;
        private MovementSpeedMode m_MovementSpeedMode = MovementSpeedMode.Walk;
        private bool m_Paused = false;
        private bool m_IsGrounded = false;
        private bool m_IsFlying = false;

        #region Editor Fields

        [Header("Movement Settings")]
        public float slowSpeed = 3;
        public float normalSpeed = 5;
        public float fastSpeed = 10;
        public float flightSpeedMultiplier = 3;
        public float airborneForceMultiplier = 5;
        public float lookSensitivity = 1.0f;
        public float mouseSensitivity = 0.25f;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;

        #endregion

        #region Public Fields

        public bool isFlying
        {
            get => m_IsFlying;
            set
            {
                m_IsFlying = value;

                if (!m_IsFlying)
                {
                    m_Rigidbody.useGravity = true;
                }
                else
                {
                    m_Rigidbody.useGravity = false;
                }
            }
        }

        public bool Paused => m_Paused;

        #endregion

        private void Start()
        {
            m_Transform = GetComponent<Transform>();
            m_CameraTransform = Camera.main.GetComponent<Transform>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_Rigidbody = GetComponent<Rigidbody>();

#if !UNITY_STANDALONE && !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
#endif

            var m_MovementActionMap = InputManager.GetActionMap("Movement");
            m_MovementActionMap.Enable();

            // Run mode.
            m_MovementActionMap["Run"].started += (c) => SetMovementType(MovementSpeedMode.Run);
            m_MovementActionMap["Run"].canceled += (c) => SetMovementType(MovementSpeedMode.Walk);

            // Fly mode.
            m_MovementActionMap["Fly"].started += (c) => isFlying = !isFlying;

            // Jump.
            m_MovementActionMap["Jump"].started += (c) => Jump();

            // Crouch
            m_MovementActionMap["Crouch"].started += (c) => Crouch(true);
            m_MovementActionMap["Crouch"].canceled += (c) => Crouch(false);

            m_MovementActionMap["LeftAxis"].performed += c => m_LeftStickValue = c.ReadValue<Vector2>();
            m_MovementActionMap["LeftAxis"].canceled += c => m_LeftStickValue = Vector2.zero;

            m_MovementActionMap["RightAxis"].performed += c => m_RightStickValue = c.ReadValue<Vector2>();
            m_MovementActionMap["RightAxis"].canceled += c => m_RightStickValue = Vector2.zero;

            // Setup the camera
            var config = GameSettings.Get();
        }

        private void OnEnable() => m_MovementActionMap?.Enable();
        private void OnDisable() => m_MovementActionMap?.Disable();

        private void Update()
        {
            if (m_Paused)
            {
                return;
            }

            RotatePlayer();
        }

        private void FixedUpdate()
        {
            m_IsGrounded = CalculateIsGrounded();

            if (m_IsGrounded || isFlying)
            {
                SetVelocity();
            }
            else if (!m_IsGrounded || !isFlying)
            {
                ApplyAirborneForce();
            }
        }

        public void SetMovementType(MovementSpeedMode mode)
        {
            if (mode == MovementSpeedMode.Slow && m_MovementSpeedMode == MovementSpeedMode.Slow)
            {
                m_MovementSpeedMode = MovementSpeedMode.Walk;
            }
            else if (mode == MovementSpeedMode.Run && m_MovementSpeedMode != MovementSpeedMode.Slow)
            {
                m_MovementSpeedMode = MovementSpeedMode.Run;
            }
            else
            {
                m_MovementSpeedMode = MovementSpeedMode.Walk;
            }
        }

        private void Crouch(bool crouch)
        {
            if (crouch)
            {
                SetMovementType(MovementSpeedMode.Slow);
            }
            else
            {
                SetMovementType(MovementSpeedMode.Walk);
            }
        }

        private void Jump()
        {
            if (m_IsGrounded && !m_IsFlying)
            {
                var newVelocity = m_Rigidbody.velocity;
                newVelocity.y = 5;

                m_Rigidbody.velocity = newVelocity;
            }
        }

        private void RotatePlayer()
        {
            var eulerAngles = new Vector3(m_CameraTransform.localEulerAngles.x, m_Transform.localEulerAngles.y, 0);

            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
            {
                eulerAngles.x = eulerAngles.x - 360;
            }

            var deltaMouse = lookSensitivity * m_RightStickValue;

            eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
            eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

               m_CameraTransform.localEulerAngles = new Vector3(eulerAngles.x, 0, 0);

            m_Transform.localEulerAngles = new Vector3(0, eulerAngles.y, 0);
        }

        private void SetVelocity()
        {
            Vector3 velocity;

            if (!isFlying)
            {
                velocity = m_Transform.TransformVector(CalculateLocalVelocity());
                velocity.y = m_Rigidbody.velocity.y;
            }
            else
            {
                velocity = m_CameraTransform.TransformVector(CalculateLocalVelocity());
            }

            m_Rigidbody.velocity = velocity;
        }

        private void ApplyAirborneForce()
        {
            var forceDirection = m_Transform.TransformVector(CalculateLocalMovementDirection());
            forceDirection.y = 0;
            forceDirection.Normalize();

            var force = airborneForceMultiplier * m_Rigidbody.mass * forceDirection;

            m_Rigidbody.AddForce(force);
        }

        private Vector3 CalculateLocalMovementDirection()
        {
            // Calculate the local movement direction.
            var leftAxis = m_LeftStickValue;// m_LeftAxisAction.ReadValue<Vector2>();

            var direction = new Vector3(leftAxis.x, 0.0f, leftAxis.y);
            return direction.normalized;
        }

        private float CalculateSpeed()
        {
            var speed = normalSpeed;

            if (m_MovementSpeedMode == MovementSpeedMode.Run)
            {
                speed = fastSpeed;
            }
            else if (m_MovementSpeedMode == MovementSpeedMode.Slow)
            {
                speed = slowSpeed;
            }

            if (isFlying)
            {
                speed *= flightSpeedMultiplier;
            }

            return speed;
        }

        private Vector3 CalculateLocalVelocity()
        {
            return CalculateSpeed() * CalculateLocalMovementDirection();
        }

        private bool CalculateIsGrounded()
        {
            var playerCenter = m_Transform.position + m_CapsuleCollider.center;
            var castedSphereRadius = 0.8f * m_CapsuleCollider.radius;
            var sphereCastDistance = (m_CapsuleCollider.height / 2);

            return Physics.SphereCast(new Ray(playerCenter, -m_Transform.up), castedSphereRadius, sphereCastDistance);
        }
    }
}