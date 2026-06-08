using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public partial class Controller : MonoBehaviour
    {
        private const int PlayerLayerIndex = 3;
        private const int PlayerLayerMask = 1 << PlayerLayerIndex;
        private const int AllLayersExceptPlayer = ~PlayerLayerMask;
        private const float MeaningfulMovementInput = 0.1f;
        private const float MeaningfulMovementInputSquared = MeaningfulMovementInput * MeaningfulMovementInput;

        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson,
            TopDown,
            Isometric,
            Platformer
        }

        [Header("Mode")]
        [SerializeField] private CameraMode cameraMode = CameraMode.FirstPerson;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float acceleration = 35f;
        [SerializeField] private float deceleration = 45f;
        [SerializeField] private float airAcceleration = 12f;
        [SerializeField] private float characterTurnSpeed = 720f;

        [Header("Gravity")]
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float fallGravityMultiplier = 1.8f;

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
    }
}
