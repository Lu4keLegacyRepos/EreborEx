using System;

namespace EreborPhoenixExtension.Libs.Healing
{
    public class HurtedPatientArgs : EventArgs
    {
        public Patient pati { get; set; }
        public bool selfHurted { get; set; }
        public bool crystalOff { get; set; }

    }
}
