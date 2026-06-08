using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        [Header("Jump")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float lowJumpGravityMultiplier = 2.5f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;

        private float coyoteTimeRemaining;
        private float jumpBufferTimeRemaining;

        private void UpdateJumpTimers()
        {
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
            ClearJumpTimers();
            isGrounded = false;
        }

        private void ClearJumpTimers()
        {
            coyoteTimeRemaining = 0f;
            jumpBufferTimeRemaining = 0f;
        }
    }
}
