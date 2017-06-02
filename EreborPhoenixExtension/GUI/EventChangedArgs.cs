using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EreborPhoenixExtension.GUI
{
    public class EventChangedArgs: EventArgs
    {
        public string btnName { get; set; }
        public int SelectedTabID { get; set; }

        public string TextValue { get; set; }

    }
}
