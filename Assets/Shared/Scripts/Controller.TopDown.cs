using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        private static readonly Vector3 TopDownFallbackRight = Vector3.right;
        private static readonly Vector3 TopDownFallbackForward = Vector3.forward;

        [Header("Top Down")]
        [SerializeField] private CinemachineCamera topDownCamera;
        [SerializeField, Min(0f)] private float topDownMovementSpeedMultiplier = 1f;
        [SerializeField, Min(0f)] private float topDownTurnSpeedMultiplier = 1f;

        private static Vector3 GetTopDownMovementDirection(
            Vector2 movementInput,
            Vector3 movementRight,
            Vector3 movementForward)
        {
            return movementRight * movementInput.x + movementForward * movementInput.y;
        }

        private static bool TryGetTopDownMovementBasis(
            Camera gameplayCamera,
            out Vector3 movementRight,
            out Vector3 movementForward)
        {
            movementRight = Vector3.ProjectOnPlane(gameplayCamera.transform.right, Vector3.up);
            movementForward = Vector3.ProjectOnPlane(gameplayCamera.transform.up, Vector3.up);

            movementRight = movementRight.sqrMagnitude > MeaningfulMovementInputSquared
                ? movementRight.normalized
                : TopDownFallbackRight;

            movementForward = movementForward.sqrMagnitude > MeaningfulMovementInputSquared
                ? movementForward.normalized
                : TopDownFallbackForward;

            return true;
        }

        private static bool HasValidTopDownSprintDirection(Vector2 movementInput)
        {
            return movementInput.sqrMagnitude > MeaningfulMovementInputSquared;
        }

        private static bool TopDownUsesLookInput() => false;
        private static bool TopDownRendersPlayer() => true;
        private static bool TopDownFacesMovement() => true;
    }
}
