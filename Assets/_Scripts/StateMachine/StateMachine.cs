using System.Collections.Generic;
using System;


    public class StateMachine {

        private BaseState _currentState;
        private BaseState _queuedState;
        private Dictionary<Type, BaseState> _states = new Dictionary<Type, BaseState>();

        public StateMachine(CharController owner, List<BaseState> states) {

            foreach (BaseState state in states) {
                BaseState instance = state;
                instance.owner = owner;
                instance.stateMachine = this;
                _states.Add(instance.GetType(), instance);

                _currentState ??= instance;
            }

            _queuedState = _currentState;
            _currentState?.Enter();
        }

        public void Run() {
            if (_currentState != _queuedState) {
                _currentState.Exit();
                _currentState = _queuedState;
                _currentState.Enter();
            }

            _currentState.Run();
        }

        public void TransitionTo<T>() where T : State {
            _queuedState = _states[typeof(T)];
        }
    }
