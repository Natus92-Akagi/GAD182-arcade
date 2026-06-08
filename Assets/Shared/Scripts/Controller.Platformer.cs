using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        [Header("Platformer")]
        [SerializeField] private CinemachineCamera platformerCamera;
        [SerializeField, Min(0f)] private float platformerMovementSpeedMultiplier = 1f;
        [SerializeField, Min(0f)] private float platformerTurnSpeedMultiplier = 1f;

        private static Vector3 GetPlatformerMovementDirection(Vector2 movementInput, Vector3 movementRight)
        {
            return movementRight * movementInput.x;
        }

        private static bool HasValidPlatformerSprintDirection(Vector2 movementInput)
        {
            return Mathf.Abs(movementInput.x) > MeaningfulMovementInput;
        }

        private static bool PlatformerUsesLookInput() => false;
        private static bool PlatformerRendersPlayer() => true;
        private static bool PlatformerFacesMovement() => true;
    }
}
