
    public abstract class BaseState : State {
        
        public CharController owner;
        private CharController _char;

        protected CharController Char => owner;
    }
