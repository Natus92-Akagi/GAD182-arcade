using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FppController : MonoBehaviour
    {
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -20f;

        private CharacterController controller;
        private InputSystem_Actions actions;
        private Vector3 verticalVelocity;
        private float cameraPitch;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            actions.Player.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            actions.Player.Disable();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnDestroy()
        {
            actions.Dispose();
        }

        private void Update()
        {
            Look();
            Move();
        }

        private void Look()
        {
            Vector2 look = actions.Player.Look.ReadValue<Vector2>() * mouseSensitivity;

            transform.Rotate(Vector3.up * look.x);

            cameraPitch -= look.y;
            cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);
            cameraRoot.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        private void Move()
        {
            Vector2 input = actions.Player.Move.ReadValue<Vector2>();

            float speed = actions.Player.Sprint.IsPressed() ? sprintSpeed : walkSpeed;

            Vector3 move =
                transform.right * input.x +
                transform.forward * input.y;

            controller.Move(move * speed * Time.deltaTime);

            if (controller.isGrounded && verticalVelocity.y < 0f)
                verticalVelocity.y = -2f;

            if (controller.isGrounded && actions.Player.Jump.WasPressedThisFrame())
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity.y += gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);
        }
    }
}