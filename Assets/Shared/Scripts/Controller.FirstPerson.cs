using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        private const float FirstPersonPanAxisCenter = 0f;

        [Header("First Person")]
        [SerializeField] private CinemachineCamera firstPersonCamera;
        [SerializeField, Min(0f)] private float firstPersonMovementSpeedMultiplier = 1f;
        [SerializeField, Min(0f)] private float firstPersonTurnSpeedMultiplier = 1f;

        private static Vector3 GetFirstPersonMovementDirection(
            Vector2 movementInput,
            Vector3 movementRight,
            Vector3 movementForward)
        {
            return movementRight * movementInput.x + movementForward * movementInput.y;
        }

        private static bool HasValidFirstPersonSprintDirection(Vector2 movementInput)
        {
            return movementInput.y > MeaningfulMovementInput;
        }

        private void ApplyFirstPersonYaw(float yawDeltaDegrees)
        {
            transform.Rotate(Vector3.up, yawDeltaDegrees, Space.World);
            cameraMovementFallbackHeading = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }

        private void ResetFirstPersonCamera()
        {
            if (firstPersonCamera == null ||
                firstPersonCamera.TryGetComponent(out CinemachinePanTilt firstPersonPanTilt) == false)
            {
                return;
            }

            firstPersonPanTilt.PanAxis.Value = FirstPersonPanAxisCenter;
            firstPersonPanTilt.PanAxis.Center = FirstPersonPanAxisCenter;
        }

        private static bool FirstPersonUsesLookInput() => true;
        private static bool FirstPersonRendersPlayer() => false;
        private static bool FirstPersonFacesMovement() => false;
    }
}
