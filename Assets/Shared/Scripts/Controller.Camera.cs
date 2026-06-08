using Sol.Grab;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public partial class Controller
    {
        private const float MinimumInputDeltaTime = 0.0001f;        //the minimum time that must pass between input updates

        [Header("Camera")]
        [SerializeField] private Camera outputCamera;               //the culling mask of the output camera will be modified to show/hide the player based on the camera mode, so this should be assigned to the camera used for rendering gameplay (often the main camera, but not necessarily)
        [SerializeField] private CinemachineBrain cameraBrain;      //the cinemachine brain, handles coordination of multiple cinemachine cameras and blending between them.

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 0.12f;    //mouse sensitivity multiplier, higher values make input more sensitive. This is only applied to mouse, gamepad input is not affected.
        [SerializeField] private float gamepadLookSpeed = 180f;     //gamepade sensitivity multiplier in degrees per second, higher values make input more sensitive. This is only applied to gamepad, mouse input is not affected.

        private CameraMode appliedCameraMode;                       //the currently applied camera mode, used to detect when the camera mode changes so that the new mode can be applied
        private int outputCameraCullingMaskWithPlayer;              //the culling mask that includes the player layer, for quickly switching between showing and hiding the player without needing to recalculate the full culling mask

        private void InitializeCamera()                             //initializes the camera system, setting up references and ensuring the correct camera mode is active at the start
        {
            if (outputCamera == null)
                outputCamera = Camera.main;                                     //set output camera to main camera if not assigned, since the main camera is often used for gameplay rendering. If there is no main camera, outputCamera will remain null and the system will still function, just without player layer visibility control.

            if (outputCamera != null)
                outputCameraCullingMaskWithPlayer = outputCamera.cullingMask | PlayerLayerMask;   //calculate the culling mask that includes the player layer, so we can easily switch between showing and hiding the player by toggling the player layer bit

            CinemachineInputAxisController[] cameraInputControllers =                                       //
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

            ResetActiveCameraMode();
            UpdatePlayerLayerVisibility();
            SetCursorLocked(UsesLookInput());
            cameraBrain?.ResetState();
        }

        private void UpdatePlayerLayerVisibility()
        {
            if (outputCamera == null)
                return;

            outputCamera.cullingMask = RendersPlayer()
                ? outputCameraCullingMaskWithPlayer
                : outputCameraCullingMaskWithPlayer & ~PlayerLayerMask;
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

            float lookInput = ReadScaledCameraInput(lookAction, axisHint, inputContext, defaultReader);
            if (cameraMode == CameraMode.FirstPerson &&
                axisHint == IInputAxisOwner.AxisDescriptor.Hints.X)
            {
                ApplyFirstPersonYaw(lookInput * Time.deltaTime);
                return 0f;
            }

            return lookInput;
        }

        private float ReadScaledCameraInput(
            InputAction lookAction,
            IInputAxisOwner.AxisDescriptor.Hints axisHint,
            Object inputContext,
            CinemachineInputAxisController.Reader.ControlValueReader defaultReader)
        {
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
            return cameraMode switch
            {
                CameraMode.FirstPerson => FirstPersonUsesLookInput(),
                CameraMode.ThirdPerson => ThirdPersonUsesLookInput(),
                CameraMode.TopDown => TopDownUsesLookInput(),
                CameraMode.Isometric => IsometricUsesLookInput(),
                CameraMode.Platformer => PlatformerUsesLookInput(),
                _ => false
            };
        }

        private bool RendersPlayer()
        {
            return cameraMode switch
            {
                CameraMode.FirstPerson => FirstPersonRendersPlayer(),
                CameraMode.ThirdPerson => ThirdPersonRendersPlayer(),
                CameraMode.TopDown => TopDownRendersPlayer(),
                CameraMode.Isometric => IsometricRendersPlayer(),
                CameraMode.Platformer => PlatformerRendersPlayer(),
                _ => true
            };
        }

        private void ResetActiveCameraMode()
        {
            if (cameraMode == CameraMode.FirstPerson)
                ResetFirstPersonCamera();
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
