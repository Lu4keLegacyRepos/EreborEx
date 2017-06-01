using System;

namespace EreborPhoenixExtension.Libs.Events
{
    public class HitsChangedArgs : EventArgs
    {

        private bool Gain;
        private short Amount;
        private bool Poison;

        public HitsChangedArgs(bool gain, short amount, bool poison)
        {
            Gain = gain;
            Amount = amount;
            Poison = poison;
        }

        public bool gain { get { return Gain; } }
        public bool poison { get { return Poison; } }
        public short amount { get { return Amount; } }
    }
}
