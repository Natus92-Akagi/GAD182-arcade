using UnityEngine;

namespace Player
{
    public partial class Controller
    {
        private void UpdateMovement()
        {
            isGrounded = verticalSpeed <= 0f && CheckGrounded();

            if (AllowsJumping())
            {
                UpdateJumpTimers();
                ApplyJump();
            }
            else
            {
                ClearJumpTimers();
            }

            Vector2 movementInput = Vector2.ClampMagnitude(inputActions.Player.Move.ReadValue<Vector2>(), 1f);
            bool isSprinting = inputActions.Player.Sprint.IsPressed() && HasValidSprintDirection(movementInput);
            float baseMovementSpeed = isSprinting ? sprintSpeed : walkSpeed;

            GetMovementBasis(out Vector3 movementRight, out Vector3 movementForward);
            Vector3 desiredMovementDirection = GetDesiredMovementDirection(
                movementInput,
                movementRight,
                movementForward);
            Vector3 desiredMovementVelocity =
                desiredMovementDirection * baseMovementSpeed * GetMovementSpeedMultiplier();

            if (FacesMovement() && desiredMovementDirection.sqrMagnitude > MeaningfulMovementInputSquared)
            {
                Quaternion movementFacingRotation = Quaternion.LookRotation(desiredMovementDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    movementFacingRotation,
                    characterTurnSpeed * GetTurnSpeedMultiplier() * Time.deltaTime);
            }

            float velocityChangeRate = isGrounded
                ? (movementInput.sqrMagnitude > MeaningfulMovementInputSquared ? acceleration : deceleration)
                : airAcceleration;

            horizontalMovementVelocity = Vector3.MoveTowards(
                horizontalMovementVelocity,
                desiredMovementVelocity,
                velocityChangeRate * Time.deltaTime);

            if (isGrounded && verticalSpeed < 0f)
                verticalSpeed = groundedVerticalSpeed;

            float gravityMultiplier = verticalSpeed < 0f
                ? fallGravityMultiplier
                : GetRisingGravityMultiplier();

            verticalSpeed += gravity * gravityMultiplier * Time.deltaTime;
            characterController.Move((horizontalMovementVelocity + Vector3.up * verticalSpeed) * Time.deltaTime);
        }

        private bool CheckGrounded()
        {
            float groundCheckRadius = characterController.radius * groundCheckRadiusScale;
            Vector3 groundCheckPosition = characterController.bounds.center +
                                          Vector3.down * (characterController.bounds.extents.y -
                                                          groundCheckRadius +
                                                          groundCheckDistance);

            return Physics.CheckSphere(
                groundCheckPosition,
                groundCheckRadius,
                groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void GetMovementBasis(out Vector3 movementRight, out Vector3 movementForward)
        {
            Camera gameplayCamera = GetGameplayCamera();
            if (gameplayCamera == null)
            {
                movementRight = transform.right;
                movementForward = transform.forward;
                return;
            }

            if (TryGetModeMovementBasis(gameplayCamera, out movementRight, out movementForward))
                return;

            movementRight = Vector3.ProjectOnPlane(gameplayCamera.transform.right, Vector3.up).normalized;
            movementForward = Vector3.ProjectOnPlane(gameplayCamera.transform.forward, Vector3.up).normalized;

            if (movementForward.sqrMagnitude < MeaningfulMovementInputSquared)
                movementForward = cameraMovementFallbackHeading * Vector3.forward;

            if (movementRight.sqrMagnitude < MeaningfulMovementInputSquared)
                movementRight = cameraMovementFallbackHeading * Vector3.right;
        }

        private bool TryGetModeMovementBasis(
            Camera gameplayCamera,
            out Vector3 movementRight,
            out Vector3 movementForward)
        {
            if (cameraMode == CameraMode.TopDown)
                return TryGetTopDownMovementBasis(gameplayCamera, out movementRight, out movementForward);

            movementRight = Vector3.zero;
            movementForward = Vector3.zero;
            return false;
        }

        private bool AllowsJumping()
        {
            return cameraMode != CameraMode.TopDown;
        }

        private float GetRisingGravityMultiplier()
        {
            return verticalSpeed > 0f && AllowsJumping() && !inputActions.Player.Jump.IsPressed()
                ? lowJumpGravityMultiplier
                : 1f;
        }

        private bool HasValidSprintDirection(Vector2 movementInput)
        {
            return cameraMode switch
            {
                CameraMode.FirstPerson => HasValidFirstPersonSprintDirection(movementInput),
                CameraMode.ThirdPerson => HasValidThirdPersonSprintDirection(movementInput),
                CameraMode.TopDown => HasValidTopDownSprintDirection(movementInput),
                CameraMode.Isometric => HasValidIsometricSprintDirection(movementInput),
                CameraMode.Platformer => HasValidPlatformerSprintDirection(movementInput),
                _ => false
            };
        }

        private Vector3 GetDesiredMovementDirection(
            Vector2 movementInput,
            Vector3 movementRight,
            Vector3 movementForward)
        {
            return cameraMode switch
            {
                CameraMode.FirstPerson => GetFirstPersonMovementDirection(movementInput, movementRight, movementForward),
                CameraMode.ThirdPerson => GetThirdPersonMovementDirection(movementInput, movementRight, movementForward),
                CameraMode.TopDown => GetTopDownMovementDirection(movementInput, movementRight, movementForward),
                CameraMode.Isometric => GetIsometricMovementDirection(movementInput, movementRight, movementForward),
                CameraMode.Platformer => GetPlatformerMovementDirection(movementInput, movementRight),
                _ => Vector3.zero
            };
        }

        private float GetMovementSpeedMultiplier()
        {
            return cameraMode switch
            {
                CameraMode.FirstPerson => firstPersonMovementSpeedMultiplier,
                CameraMode.ThirdPerson => thirdPersonMovementSpeedMultiplier,
                CameraMode.TopDown => topDownMovementSpeedMultiplier,
                CameraMode.Isometric => isometricMovementSpeedMultiplier,
                CameraMode.Platformer => platformerMovementSpeedMultiplier,
                _ => 1f
            };
        }

        private float GetTurnSpeedMultiplier()
        {
            return cameraMode switch
            {
                CameraMode.FirstPerson => firstPersonTurnSpeedMultiplier,
                CameraMode.ThirdPerson => thirdPersonTurnSpeedMultiplier,
                CameraMode.TopDown => topDownTurnSpeedMultiplier,
                CameraMode.Isometric => isometricTurnSpeedMultiplier,
                CameraMode.Platformer => platformerTurnSpeedMultiplier,
                _ => 1f
            };
        }

        private bool FacesMovement()
        {
            return cameraMode switch
            {
                CameraMode.FirstPerson => FirstPersonFacesMovement(),
                CameraMode.ThirdPerson => ThirdPersonFacesMovement(),
                CameraMode.TopDown => TopDownFacesMovement(),
                CameraMode.Isometric => IsometricFacesMovement(),
                CameraMode.Platformer => PlatformerFacesMovement(),
                _ => true
            };
        }
    }
}
