using System;

namespace EreborPhoenixExtension.Libs.Events
{
    public class HiddenChangeArgs : EventArgs
    {

        private bool HiddenState;

        public HiddenChangeArgs(bool hiddenState)
        {
            this.HiddenState = hiddenState;
        }

        public bool hiddenState { get { return HiddenState; } }
    }
}
