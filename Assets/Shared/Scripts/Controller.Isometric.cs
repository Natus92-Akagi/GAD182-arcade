using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        [Header("Isometric")]
        [SerializeField] private CinemachineCamera isometricCamera;
        [SerializeField, Min(0f)] private float isometricMovementSpeedMultiplier = 1f;
        [SerializeField, Min(0f)] private float isometricTurnSpeedMultiplier = 1f;

        private static Vector3 GetIsometricMovementDirection(
            Vector2 movementInput,
            Vector3 movementRight,
            Vector3 movementForward)
        {
            return movementRight * movementInput.x + movementForward * movementInput.y;
        }

        private static bool HasValidIsometricSprintDirection(Vector2 movementInput)
        {
            return movementInput.y > MeaningfulMovementInput;
        }

        private static bool IsometricUsesLookInput() => true;
        private static bool IsometricRendersPlayer() => true;
        private static bool IsometricFacesMovement() => true;
    }
}
