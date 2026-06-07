using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public partial class FppController : MonoBehaviour
    {
        private const int PlayerLayerIndex = 3;
        private const int AllLayersExceptPlayer = ~(1 << PlayerLayerIndex);
        private const float MeaningfulMovementInput = 0.1f;
        private const float MeaningfulMovementInputSquared = MeaningfulMovementInput * MeaningfulMovementInput;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float acceleration = 35f;
        [SerializeField] private float deceleration = 45f;
        [SerializeField] private float airAcceleration = 12f;
        [SerializeField] private float characterTurnSpeed = 720f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float fallGravityMultiplier = 1.8f;
        [SerializeField] private float lowJumpGravityMultiplier = 2.5f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;

        [Header("Grounding")]
        [SerializeField] private LayerMask groundLayers = AllLayersExceptPlayer;
        [SerializeField] private float groundCheckDistance = 0.08f;
        [SerializeField, Range(0.5f, 1f)] private float groundCheckRadiusScale = 0.9f;
        [SerializeField] private float groundedVerticalSpeed = -2f;

        private CharacterController characterController;
        private InputSystem_Actions inputActions;
        private Vector3 horizontalMovementVelocity;
        private Quaternion cameraMovementFallbackHeading;
        private float verticalSpeed;
        private float coyoteTimeRemaining;
        private float jumpBufferTimeRemaining;
        private bool isGrounded;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputActions = new InputSystem_Actions();
            cameraMovementFallbackHeading = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

            InitializeCamera();

            isGrounded = CheckGrounded();
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
            EnableCamera();
        }

        private void OnDisable()
        {
            inputActions.Player.Disable();
            DisableCamera();
        }

        private void OnDestroy()
        {
            inputActions.Dispose();
        }

        private void Update()
        {
            UpdateCameraMode();
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            isGrounded = verticalSpeed <= 0f && CheckGrounded();

            if (AllowsJumping())
            {
                UpdateJumpTimers();
                ApplyJump();
            }
            else
            {
                // Changing modes must not leave a stored jump ready for Platformer.
                coyoteTimeRemaining = 0f;
                jumpBufferTimeRemaining = 0f;
            }

            Vector2 movementInput = Vector2.ClampMagnitude(inputActions.Player.Move.ReadValue<Vector2>(), 1f);
            bool isSprinting = inputActions.Player.Sprint.IsPressed() && HasValidSprintDirection(movementInput);
            float movementSpeed = isSprinting ? sprintSpeed : walkSpeed;

            Vector3 desiredMovementDirection = GetDesiredMovementDirection(movementInput);
            Vector3 desiredMovementVelocity = desiredMovementDirection * movementSpeed;

            if (desiredMovementDirection.sqrMagnitude > MeaningfulMovementInputSquared)
            {
                Quaternion movementFacingRotation = Quaternion.LookRotation(desiredMovementDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    movementFacingRotation,
                    characterTurnSpeed * Time.deltaTime);
            }

            float velocityChangeRate = isGrounded
                ? (movementInput.sqrMagnitude > MeaningfulMovementInputSquared ? acceleration : deceleration)
                : airAcceleration;

            horizontalMovementVelocity = Vector3.MoveTowards(
                horizontalMovementVelocity,
                desiredMovementVelocity,
                velocityChangeRate * Time.deltaTime);

            if (isGrounded && verticalSpeed < 0f)
                verticalSpeed = groundedVerticalSpeed;

            // Modes without jumping still apply gravity so ledge falls finish naturally.
            float gravityMultiplier = verticalSpeed < 0f
                ? fallGravityMultiplier
                : verticalSpeed > 0f && AllowsJumping() && !inputActions.Player.Jump.IsPressed()
                    ? lowJumpGravityMultiplier
                    : 1f;

            verticalSpeed += gravity * gravityMultiplier * Time.deltaTime;
            characterController.Move((horizontalMovementVelocity + Vector3.up * verticalSpeed) * Time.deltaTime);

        }

        private void UpdateJumpTimers()
        {
            // These short timers forgive a slightly late ledge jump or slightly early landing jump.
            coyoteTimeRemaining = isGrounded
                ? coyoteTime
                : Mathf.Max(0f, coyoteTimeRemaining - Time.deltaTime);

            jumpBufferTimeRemaining = inputActions.Player.Jump.WasPressedThisFrame()
                ? jumpBufferTime
                : Mathf.Max(0f, jumpBufferTimeRemaining - Time.deltaTime);
        }

        private void ApplyJump()
        {
            if (jumpBufferTimeRemaining <= 0f || coyoteTimeRemaining <= 0f)
                return;

            verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimeRemaining = 0f;
            coyoteTimeRemaining = 0f;
            isGrounded = false;
        }

        private bool CheckGrounded()
        {
            float groundCheckRadius = characterController.radius * groundCheckRadiusScale;
            Vector3 groundCheckPosition = characterController.bounds.center +
                                          Vector3.down * (characterController.bounds.extents.y -
                                                          groundCheckRadius +
                                                          groundCheckDistance);

            return Physics.CheckSphere(
                groundCheckPosition,
                groundCheckRadius,
                groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void GetMovementBasis(out Vector3 movementRight, out Vector3 movementForward)
        {
            Camera gameplayCamera = GetGameplayCamera();
            if (gameplayCamera == null)
            {
                movementRight = transform.right;
                movementForward = transform.forward;
                return;
            }

            // Flatten the output camera axes so looking vertically never creates vertical movement.
            movementRight = Vector3.ProjectOnPlane(gameplayCamera.transform.right, Vector3.up).normalized;
            movementForward = Vector3.ProjectOnPlane(gameplayCamera.transform.forward, Vector3.up).normalized;

            if (movementForward.sqrMagnitude < MeaningfulMovementInputSquared)
                movementForward = cameraMovementFallbackHeading * Vector3.forward;

            if (movementRight.sqrMagnitude < MeaningfulMovementInputSquared)
                movementRight = cameraMovementFallbackHeading * Vector3.right;
        }

        private bool AllowsJumping()
        {
            return cameraMode == CameraMode.Platformer;
        }

        private bool HasValidSprintDirection(Vector2 movementInput)
        {
            return cameraMode switch
            {
                CameraMode.Platformer => Mathf.Abs(movementInput.x) > MeaningfulMovementInput,
                CameraMode.TopDown => movementInput.sqrMagnitude > MeaningfulMovementInputSquared,
                _ => movementInput.y > MeaningfulMovementInput
            };
        }

        private Vector3 GetDesiredMovementDirection(Vector2 movementInput)
        {
            GetMovementBasis(out Vector3 movementRight, out Vector3 movementForward);

            return cameraMode == CameraMode.Platformer
                ? movementRight * movementInput.x
                : movementRight * movementInput.x + movementForward * movementInput.y;
        }
    }
}
