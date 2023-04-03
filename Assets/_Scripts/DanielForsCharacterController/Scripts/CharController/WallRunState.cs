using UnityEngine;

	public class WallRunState : BaseState {
		private const float AntiFloatForce = 25.0f;
		private static float wallRunMagnitudeThreshold = 4.0f;
		private Vector3 _wallNormal;
		private const string State = "WallRunState";

		public static bool Requirement(CharController @char) {

			if (@char.Grounded || Vector3.Dot(@char._velocity, @char.transform.forward) < 0.0f)
				return false;

			Vector3 verticalVelocity = Vector3.ProjectOnPlane(@char._velocity, Vector3.up);

			RaycastHit rightHit = RayCast(@char, @char.transform.right);
			if (rightHit.collider) {
				Vector3 wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, rightHit.normal);
				return wallProjectionVector.magnitude > wallRunMagnitudeThreshold;
			}

			RaycastHit leftHit = RayCast(@char, -@char.transform.right);
			if (leftHit.collider) {
				Vector3 wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, leftHit.normal);
				return wallProjectionVector.magnitude > wallRunMagnitudeThreshold;
			}

			return false;
		}

		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			RaycastHit rightHit = RayCast(Char, Char.transform.right);
			RaycastHit leftHit = RayCast(Char, -Char.transform.right);

			if (rightHit.collider)
				_wallNormal = rightHit.normal;
			else if (leftHit.collider)
				_wallNormal = leftHit.normal;
		}

		public override void Run() {
			// Move @char along vector of wall surface normal
			// project characters _velocity on wall normal, multiply with initial wall running speed
			// redirect characters _velocity into run? add upwards velocity?
			// decrease speed with time

			// Player.AirControl();

			AddGravityForce();

			// Player.ApplyAirFriction();

			// if (Player.pressedJump && Player._velocity.y > threshold) 
			// 	stateMachine.TransitionTo<JumpState>();

			if (Char.PressedJump)
				stateMachine.TransitionTo<WallJumpState>();

			if (!RayCast(Char, Char.transform.right).collider && !RayCast(Char, -Char.transform.right).collider)
				stateMachine.TransitionTo<AirState>();

			if (Char.Grounded)
				stateMachine.TransitionTo<MoveState>();
		}

		private void AddGravityForce() {

			Vector3 gravityMovement = Char._defaultGravity * Time.deltaTime * Vector3.down;
			if (Char._velocity.y > 0.0f)
				gravityMovement *= .75f;
			else {
				Char._velocity += Char._velocity.y * -_wallNormal * Time.deltaTime;
			}

			//CounteractFloat();
			Char._velocity += gravityMovement;
		}

		private static RaycastHit RayCast(CharController @char, Vector3 direction) {
			Ray ray = new Ray(@char._point2Transform.position/*Player.transform.position + Player._point2*/, direction);
			Physics.Raycast(ray, out var hit, @char._colliderRadius + .5f, @char._collisionMask);
			return hit;
		}

		public override void Exit() { }
	}
