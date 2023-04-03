using UnityEngine;

    public class JumpState : BaseState {
        private bool _falling = false;

        private const string State = "JumpState";
        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            _falling = false;
            Char._jumpedOnce = true;
            Char.airTime = 0;
            Char._velocity.y = Char._jumpForce;
        }

        public override void Run() {

            Char.AirControl();

            Vector3 gravityMovement = Char._defaultGravity * Time.deltaTime * Vector3.down;

            Char._velocity += gravityMovement;

            Char.ApplyAirFriction();

            if (!Char.HoldingJump || Char._velocity.y < float.Epsilon)
                _falling = true;

            if (_falling)
                stateMachine.TransitionTo<AirState>();

            if (WallRunState.Requirement(Char))
                stateMachine.TransitionTo<WallRunState>();

            if (Char.Grounded && Char._velocity.y < float.Epsilon)
                stateMachine.TransitionTo<MoveState>();
        }

        public override void Exit() { }
    }
