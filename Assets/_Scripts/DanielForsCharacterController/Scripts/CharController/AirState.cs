using UnityEngine;

	public class AirState : BaseState {

		private const float AntiFloatForce = 25.0f;

		private const string State = "AirState";
		public override void Enter() {
			
			StateChange.stateUpdate?.Invoke(State);
		}

		public override void Run() {

			Char.AirControl();

			AddGravityForce();

			Char.ApplyAirFriction();

			if (Char.Grounded)
				stateMachine.TransitionTo<MoveState>();

			if (Char.Jumped) // coyote time jump
				stateMachine.TransitionTo<JumpState>();

			if (WallRunState.Requirement(Char))
				stateMachine.TransitionTo<WallRunState>();
		}

		private void AddGravityForce() {

			float gravityMovement =
				-Char._defaultGravity * Char._fallGravityMultiplier * Time.deltaTime;
			
			CounteractFloat();
			
			Char._velocity.y += gravityMovement;

			void CounteractFloat() {
				if (Char._velocity.y > 0)
					gravityMovement -= AntiFloatForce * Time.deltaTime;
			}
		}

		public override void Exit() { }
	}
