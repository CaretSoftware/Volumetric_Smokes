using UnityEngine;

    public class MoveState : BaseState {

        //private float inAirTime;
        //private float transitionToAirStateTime = .1f;
        
        private const string State = "MoveState";
        public override void Enter() {
            StateChange.stateUpdate?.Invoke(State);
            Char._jumpedOnce = false;
            //inAirTime = 0.0f;
        }

        public override void Run() {

            // if (Char._inputMovement.magnitude > float.Epsilon)
            //     Char.Accelerate(Char._inputMovement);
            // else
            //     Char.Decelerate();
            
            StepUp();
            
            Char.HandleVelocity();

            if (Vector3.Angle(Char.GroundNormal, Vector3.up) < 40)
                ApplyStaticFriction();
            else
                AddGravityForce();

            
            if (Char.Jumped)
                stateMachine.TransitionTo<JumpState>();

            if (!Char.Grounded)
                stateMachine.TransitionTo<AirState>();
            //     inAirTime += Time.deltaTime;
            // if (inAirTime >= transitionToAirstateTime)
        }

        private void StepUp() {

            Vector3 stepHeight = Vector3.up * .3f;
            Vector3 velocity = Vector3.ProjectOnPlane(Char._velocity, Vector3.up) * Time.deltaTime;
            Vector3 direction = velocity.normalized;
            float maxDistance = velocity.magnitude + Char._skinWidth;
            
            if (Physics.CapsuleCast(
                    Char._point1Transform.position,
                    Char._point2Transform.position, 
                    Char._colliderRadius, 
                    direction, 
                    out RaycastHit lowHit,
                    maxDistance,
                    Char._collisionMask) &&
                Char._velocity.y < float.Epsilon &&
                // Vector3.Dot(lowHit.normal, Vector3.up) < .6f &&
                !Physics.CapsuleCast(
                    Char._point1Transform.position + stepHeight,
                    Char._point2Transform.position + stepHeight, 
                    Char._colliderRadius, 
                    direction, 
                    maxDistance + Char._colliderRadius,
                    Char._collisionMask)) {
                
                Vector3 maxMagnitude = Vector3.ClampMagnitude(direction * Char._colliderRadius, Char._velocity.magnitude);
                Physics.CapsuleCast(
                    Char._point1Transform.position + stepHeight + maxMagnitude,
                    Char._point2Transform.position + stepHeight + maxMagnitude,//direction * Char._colliderRadius,
                    Char._colliderRadius,
                    Vector3.down,
                    out RaycastHit hit, 
                    float.MaxValue, 
                    Char._collisionMask);
                
                Char.transform.position += (stepHeight - hit.distance * Vector3.up) * 1.0f;
            }
        }

        private void ApplyStaticFriction() {

            // if (Char._velocity.magnitude < 
            //     Char.normalForce.magnitude * Char._staticFrictionCoefficient) {
            //     Char._velocity = Vector3.zero;
            // } else {
            //     Char._velocity -= Char._velocity.normalized * Char.normalForce.magnitude *
            //                 Char._kineticFrictionCoefficient;
            // }
            
            if (Vector3.ProjectOnPlane(Char._velocity, Vector3.up).magnitude <
                Char.normalForce.magnitude * Char._staticFrictionCoefficient) {
                
                // float verticalVelocity = Char._velocity.y;
                Char._velocity = Vector3.zero;
                // Char._velocity.y = verticalVelocity;
            }
        }

        private void AddGravityForce() {

            float gravityMovement = -Char._defaultGravity * Time.deltaTime;
            Char._velocity.y += gravityMovement;
        }

        public override void Exit() { }
    }
