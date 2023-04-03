using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class WallJumpState : BaseState {
		private bool _falling = false;

		private const string State = "WallJumpState";
		public override void Enter() {
			StateChange.stateUpdate?.Invoke(State);
			
			_falling = false;
			Vector3 verticalVelocity = Vector3.ProjectOnPlane(Char._velocity, Vector3.up);

			RaycastHit rightHit = RayCast(Char, Char.transform.right);
			RaycastHit leftHit = RayCast(Char, -Char.transform.right);

			Vector3 redirectedVelocity = Char._velocity;
			Vector3 wallProjectionVector;
			if (rightHit.collider) {
				wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, rightHit.normal);
				redirectedVelocity = RedirectVelocity(wallProjectionVector, rightHit.normal);
			}
			else if (leftHit.collider) {
				wallProjectionVector = Vector3.ProjectOnPlane(verticalVelocity, leftHit.normal);
				redirectedVelocity = RedirectVelocity(wallProjectionVector, leftHit.normal);
			}

			redirectedVelocity += redirectedVelocity.normalized * 1.0f;
			redirectedVelocity.y = Char._jumpForce;;
			Char._velocity = redirectedVelocity;
		}

		private Vector3 RedirectVelocity(Vector3 velocity, Vector3 normal) {

			Vector3 direction = velocity.normalized;
			float magnitude = velocity.magnitude;

			return magnitude * Vector3.Slerp(direction, normal, .5f);
		}

		public override void Run() {
            
			Char.AirControl();

			float gravityMovement = -Char._defaultGravity * 2.0f * Time.deltaTime;
			Char._velocity.y += gravityMovement;
			
			Char.ApplyAirFriction();
			
			if (!Char.HoldingJump || Char._velocity.y < float.Epsilon)
				_falling = true;

			if (_falling)
				stateMachine.TransitionTo<AirState>();

			// if (WallRunState.Requirement(Player))
			// 	stateMachine.TransitionTo<WallRunState>();

			if (Char.Grounded && Char._velocity.y < float.Epsilon)
				stateMachine.TransitionTo<MoveState>();
		}

		private static RaycastHit RayCast(CharController @char, Vector3 direction) {
			Ray ray = new Ray( @char._point2Transform.position/*Player.transform.position + Player._point2*/, direction);
			Physics.Raycast(ray, out var hit, @char._colliderRadius + .5f, @char._collisionMask);
			return hit;
		}

		public override void Exit() { }
	}
