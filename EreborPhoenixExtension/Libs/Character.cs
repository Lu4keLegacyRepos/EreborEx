using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EreborPhoenixExtension.Libs
{
    public delegate void GetClass();
    public static class Character
    {
        public static event EventHandler<ClassReturnArgs> ClassKnown;
        private static Dictionary<string, int> classes = new Dictionary<string, int>() { { "priest", 1 }, { "shaman" ,2},
            { "mag",3 }, { "necro",4 }, { "thief",5 }, { "craft",8 }, { "war",6 }, { "ranger",7 } };
        public static EreborClass ActualClass=0;
        public enum EreborClass
        {
            Null=0,
            Priest=1,
            Shaman=2,
            Mag=3,
            Necromant=4,
            Thief=5,
            Warrior=6,
            Ranger=7,
            Kraft=8
        }

        public static void GetClass()
        {
            Core.RegisterServerMessageCallback(0x1C, onWhoami);
            int x = World.Player.X;
            int y = World.Player.Y;
            while (x == World.Player.X && y == World.Player.Y) UO.Wait(100);
            UO.Say(".whoami");
            UO.Wait(300);
            Core.UnregisterServerMessageCallback(0x1C, onWhoami);
            if (ActualClass > 0)
            {
                ClassKnown?.Invoke(null, new ClassReturnArgs() { ActualClass= ActualClass });
            }
            else MessageBox.Show("Neznámé povolání");

        }


        public class ClassReturnArgs: EventArgs
        {
            public EreborClass ActualClass { get; set; }
        }

        static CallbackResult onWhoami(byte[] data, CallbackResult prevResult)//0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in classes.Keys)
            {
                if (packet.Text.Contains(s))
                {
                    ActualClass = (EreborClass)classes[s];
                    return CallbackResult.Eat;
                }
            }
            return CallbackResult.Normal;
        }
    }
}
