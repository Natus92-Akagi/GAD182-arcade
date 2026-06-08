using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        [Header("Third Person")]
        [SerializeField] private CinemachineCamera thirdPersonCamera;
        [SerializeField, Min(0f)] private float thirdPersonMovementSpeedMultiplier = 1f;
        [SerializeField, Min(0f)] private float thirdPersonTurnSpeedMultiplier = 1f;

        private static Vector3 GetThirdPersonMovementDirection(
            Vector2 movementInput,
            Vector3 movementRight,
            Vector3 movementForward)
        {
            return movementRight * movementInput.x + movementForward * movementInput.y;
        }

        private static bool HasValidThirdPersonSprintDirection(Vector2 movementInput)
        {
            return movementInput.y > MeaningfulMovementInput;
        }

        private static bool ThirdPersonUsesLookInput() => true;
        private static bool ThirdPersonRendersPlayer() => true;
        private static bool ThirdPersonFacesMovement() => true;
    }
}
