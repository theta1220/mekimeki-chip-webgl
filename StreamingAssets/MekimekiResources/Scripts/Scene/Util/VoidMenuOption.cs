    using System;

    public class VoidMenuOption : IMenuOption
    {
        private string _name;
        public string Name => _name;
        public Action Action;

        public int Dummy;
        public object Reference
        {
            get { return null; }
            set { Dummy = (int)value; }
        }
        
        public VoidMenuOption(string name, Action action)
        {
            _name = name;
            Action = action;
        }

        public string BuildText()
        {
            return _name;
        }

        public void Invoke(int dir)
        {
            if (dir != 0)
            {
                return;
            }
            Action.Invoke();
        }
    }
