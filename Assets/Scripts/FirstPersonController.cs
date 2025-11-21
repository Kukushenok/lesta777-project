using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class FirstPersonController : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset _inputActionAsset;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _moveSpeed = 6f;

        [SerializeField]
        private float _acceleration = 50f;

        [SerializeField]
        private float _maxSpeed = 6f;

        [SerializeField]
        private float _jumpForce = 5f;

        [SerializeField]
        private float _groundCheckRadius = 0.35f;

        [SerializeField]
        private float _groundCheckDistance = 0.1f;

        [SerializeField]
        private LayerMask _groundLayerMask = ~0;

        [SerializeField]
        private float _lookSensitivity = 1.5f;

        [SerializeField]
        private float _minPitch = -85f;

        [SerializeField]
        private float _maxPitch = 85f;

        private Rigidbody _rigidbody;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;

        private float _pitch;
        private Vector2 _moveInput;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            if (_camera == null)
            {
                _camera = GetComponentInChildren<Camera>();
            }

            if (_inputActionAsset == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("InputActionAsset not assigned on FirstPersonController; attempting to find in project is skipped at runtime. Assign the asset in inspector.", this);
#else
                Debug.LogWarning("InputActionAsset not assigned on FirstPersonController.", this);
#endif
            }
            else
            {
                _moveAction = _inputActionAsset.FindAction("Move");
                _lookAction = _inputActionAsset.FindAction("Look");
                _jumpAction = _inputActionAsset.FindAction("Jump");

                if (_moveAction == null)
                {
                    Debug.LogError("Move action not found in InputActionAsset (expected name: \"Move\").", this);
                }

                if (_lookAction == null)
                {
                    Debug.LogError("Look action not found in InputActionAsset (expected name: \"Look\").", this);
                }

                if (_jumpAction == null)
                {
                    Debug.LogError("Jump action not found in InputActionAsset (expected name: \"Jump\").", this);
                }
            }

            // Initialize pitch from current camera rotation
            if (_camera != null)
            {
                _pitch = _camera.transform.localEulerAngles.x;

                // Convert angle to signed -180..180
                if (_pitch > 180f)
                {
                    _pitch -= 360f;
                }
            }

            // Lock cursor for FPS control
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            if (_moveAction != null)
            {
                _moveAction.performed += OnMovePerformed;
                _moveAction.canceled += OnMoveCanceled;
                _moveAction.Enable();
            }

            _lookAction?.Enable();

            if (_jumpAction != null)
            {
                _jumpAction.performed += OnJumpPerformed;
                _jumpAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMovePerformed;
                _moveAction.canceled -= OnMoveCanceled;
                _moveAction.Disable();
            }

            _lookAction?.Disable();

            if (_jumpAction != null)
            {
                _jumpAction.performed -= OnJumpPerformed;
                _jumpAction.Disable();
            }
        }

        private void LateUpdate()
        {
            HandleLook();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleLook()
        {
            if (_lookAction == null || _camera == null)
            {
                return;
            }
            var scale = 2.0f / (Screen.width + Screen.height);
            Vector2 delta = _lookAction.ReadValue<Vector2>();
            Vector2 lookDelta = _lookSensitivity * scale * delta;

            transform.Rotate(Vector3.up, lookDelta.x);

            _pitch -= lookDelta.y;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            Vector3 cameraLocalEuler = _camera.transform.localEulerAngles;
            cameraLocalEuler.x = _pitch;
            _camera.transform.localEulerAngles = cameraLocalEuler;
        }
        private void HandleMovement()
        {
            if (_moveAction == null)
            {
                return;
            }

            // Build move direction relative to player yaw (ignore vertical component)
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 desiredVelocity = ((forward * _moveInput.y) + (right * _moveInput.x)) * _moveSpeed;

            Vector3 currentVelocity = _rigidbody.linearVelocity;
            var currentHorizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

            Vector3 velocityChange = desiredVelocity - currentHorizontal;

            // Apply acceleration force (hybrid style)
            _rigidbody.AddForce(velocityChange * _acceleration, ForceMode.Acceleration);

            // Manual clamp of horizontal speed
            var horizontalSpeed = currentHorizontal.magnitude;
            if (horizontalSpeed > _maxSpeed)
            {
                Vector3 clampedHorizontal = currentHorizontal.normalized * _maxSpeed;
                var newVelocity = new Vector3(clampedHorizontal.x, currentVelocity.y, clampedHorizontal.z);
                _rigidbody.linearVelocity = newVelocity;
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            }
        }

        private bool IsGrounded()
        {
            // SphereCast origin slightly above transform.position so cast starts inside the capsule collider area
            Vector3 origin = transform.position + (Vector3.up * (_groundCheckRadius + 0.01f));
            Vector3 direction = Vector3.down;
            var maxDistance = _groundCheckDistance + _groundCheckRadius;
            return Physics.SphereCast(origin, _groundCheckRadius, direction, out _, maxDistance, _groundLayerMask, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Allows runtime sensitivity adjustment.
        /// Public method is allowed by your rules (no public fields).
        /// </summary>
        public void SetLookSensitivity(float sensitivity)
        {
            _lookSensitivity = sensitivity;
        }

        /// <summary>
        /// Optional helper to change jump force at runtime.
        /// </summary>
        public void SetJumpForce(float jumpForce)
        {
            _jumpForce = jumpForce;
        }
    }
}
