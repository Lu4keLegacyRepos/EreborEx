using Phoenix.WorldData;
using System;
using System.Xml.Serialization;

namespace EreborPhoenixExtension.Libs.Healing
{
    [Serializable]
    public class Patient
    {
        [XmlIgnore]
        public UOCharacter chara;
        public uint character {
            get
            {
                return chara.Serial;
            }
            set
            {
                chara = new UOCharacter(value);
            }
        }
        public int equip { get; set; }


    }
}
