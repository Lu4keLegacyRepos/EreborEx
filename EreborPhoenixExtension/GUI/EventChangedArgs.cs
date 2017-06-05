using System;

namespace EreborPhoenixExtension.GUI
{
    public class EventChangedArgs: EventArgs
    {
        public string btnName { get; set; }
        public int SelectedTabID { get; set; }

        public string TextValue { get; set; }

    }
}
