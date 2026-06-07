using Sol.Grab;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public partial class FppController
    {
        private const float MinimumInputDeltaTime = 0.0001f;

        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson,
            TopDown,
            Isometric,
            Platformer
        }

        [Header("Camera")]
        [SerializeField] private CameraMode cameraMode = CameraMode.FirstPerson;
        [SerializeField] private Camera outputCamera;
        [SerializeField] private CinemachineBrain cameraBrain;
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField] private CinemachineCamera thirdPersonCamera;
        [SerializeField] private CinemachineCamera topDownCamera;
        [SerializeField] private CinemachineCamera isometricCamera;
        [SerializeField] private CinemachineCamera platformerCamera;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float gamepadLookSpeed = 180f;

        private CameraMode appliedCameraMode;

        private void InitializeCamera()
        {
            CinemachineInputAxisController[] cameraInputControllers =
                GetComponentsInChildren<CinemachineInputAxisController>(true);
            foreach (CinemachineInputAxisController inputController in cameraInputControllers)
                inputController.ReadControlValueOverride = ReadCameraInput;

            appliedCameraMode = cameraMode;
            ApplyCameraMode();
        }

        private void EnableCamera()
        {
            SetCursorLocked(UsesLookInput());
        }

        private void DisableCamera()
        {
            SetCursorLocked(false);
        }

        private void UpdateCameraMode()
        {
            if (appliedCameraMode != cameraMode)
            {
                appliedCameraMode = cameraMode;
                ApplyCameraMode();
            }

            if (UsesLookInput() && Keyboard.current?.escapeKey.wasPressedThisFrame == true)
                SetCursorLocked(Cursor.lockState != CursorLockMode.Locked);
        }

        private void ApplyCameraMode()
        {
            SetCameraActive(firstPersonCamera, cameraMode == CameraMode.FirstPerson);
            SetCameraActive(thirdPersonCamera, cameraMode == CameraMode.ThirdPerson);
            SetCameraActive(topDownCamera, cameraMode == CameraMode.TopDown);
            SetCameraActive(isometricCamera, cameraMode == CameraMode.Isometric);
            SetCameraActive(platformerCamera, cameraMode == CameraMode.Platformer);

            SetCursorLocked(UsesLookInput());
            cameraBrain?.ResetState();
        }

        private float ReadCameraInput(
            InputAction lookAction,
            IInputAxisOwner.AxisDescriptor.Hints axisHint,
            Object inputContext,
            CinemachineInputAxisController.Reader.ControlValueReader defaultReader)
        {
            if (!UsesLookInput() ||
                Cursor.lockState != CursorLockMode.Locked ||
                IsGrabRotationActive())
            {
                return 0f;
            }

            float lookInput = defaultReader(lookAction, axisHint, inputContext, null);
            if (lookAction.activeControl?.device is Pointer)
            {
                float inputDeltaTime = Mathf.Max(Time.deltaTime, MinimumInputDeltaTime);
                return lookInput * mouseSensitivity / inputDeltaTime;
            }

            return lookInput * gamepadLookSpeed;
        }

        private Camera GetGameplayCamera()
        {
            return Camera.main != null ? Camera.main : outputCamera;
        }

        private bool UsesLookInput()
        {
            return cameraMode is CameraMode.FirstPerson or CameraMode.ThirdPerson or CameraMode.Isometric;
        }

        private static bool IsGrabRotationActive()
        {
            return GrabManager.Instance != null &&
                   GrabManager.Instance.HeldObject != null &&
                   GrabManager.Instance.rotationMode;
        }

        private static void SetCameraActive(CinemachineCamera cinemachineCamera, bool shouldBeActive)
        {
            if (cinemachineCamera != null)
                cinemachineCamera.gameObject.SetActive(shouldBeActive);
        }

        private static void SetCursorLocked(bool shouldLockCursor)
        {
            Cursor.lockState = shouldLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !shouldLockCursor;
        }
    }
}
