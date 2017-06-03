using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Linq;

namespace EreborPhoenixExtension.Libs.Magic
{
    public delegate void arrowSelf(bool attacklast);

    public class Spells
    {
        private readonly string[] onParaCalls = new string[] { "Nohama ti projela silna bolest, citis, ze se nemuzes hybat.", " crying awfully." };
        private bool autoArrow;
        private Settings ch;
        public Spells(Settings cha)
        {
            ch = cha;
        }
        public void ReactiveArmor( UOCharacter target)
        {
            DateTime start;
            UO.Warmode(false);
            UO.Cast(StandardSpell.ReactiveArmor, target);
            if (!Journal.WaitForText(true, 2000, "Kouzlo se nezdarilo."))
            {
                start = DateTime.Now;
            }
            else return;
            while (DateTime.Now - start < TimeSpan.FromSeconds(8)) UO.Wait(100);
            UO.PrintError("Reactiv vyprsel");
        }

        public void inv(Action<string,Serial,Action,Action> ccast, Action bandage, int InvDelay)
        {
            int tmp = 8;
            UO.Warmode(false);
            if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
            ccast("Invis", Aliases.Self,bandage,null);

            if (Journal.WaitForText(true, InvDelay / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(3);
            if (Journal.WaitForText(true, InvDelay / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(2);
            if (!Journal.WaitForText(true, InvDelay/3, "Kouzlo se nezdarilo."))
            {
                World.Player.Print(1);
                if (clearPerimeter(2))
                {
                    DateTime start = DateTime.Now;
                    UO.PrintInformation("Blokuju krok na 1 s");
                    Core.UnregisterClientMessageCallback(0x02, blockStep);
                    Core.RegisterClientMessageCallback(0x02, blockStep);
                    while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(1000))
                    {
                        UO.Wait(50);
                        tmp--;
                        if (!clearPerimeter(2))
                        {
                            Core.UnregisterClientMessageCallback(0x02, blockStep);
                            return;
                        }
                        if (tmp < 0 && !World.Player.Hidden)
                        {
                            Core.UnregisterClientMessageCallback(0x02, blockStep);
                            return;
                        }
                    }
                    Core.UnregisterClientMessageCallback(0x02, blockStep);
                    if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
                }
            }
        }
        private bool clearPerimeter(int perimeterSize)
        {
            foreach (UOCharacter ch in World.Characters.Where(cha=>cha.Distance<perimeterSize && cha.Notoriety>Notoriety.Neutral))
            {
                    return false;
            }
            return true;
        }

        public void arrowSelf(bool attacklast)
        {
            ch.arrowSelfProgress = true;
            UO.Warmode(false);
            UO.Cast(StandardSpell.Harm, World.Player.Serial);
            UO.Wait(1600);
            if (attacklast)
            {
                UO.Warmode(true);
                UO.Attack(Aliases.GetObject("laststatus").IsValid ? Aliases.GetObject("laststatus") : 0x00);
            }
            ch.arrowSelfProgress = false;
        }
        public bool AutoArrow { get
            {
                return autoArrow;
            }
            set
            {
                if(value)
                {
                    Core.RegisterServerMessageCallback(0x1C, onParaCall);
                }
                else Core.UnregisterServerMessageCallback(0x1C, onParaCall);
                autoArrow = value;
                }
        }




        private CallbackResult onParaCall(byte[] data, CallbackResult prevResult)
        {
            //0x1C
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in onParaCalls)
            {
                if (packet.Text.Contains(s))
                {
                    arrowSelf sa = new arrowSelf(arrowSelf);
                    sa.BeginInvoke(false,null,null);
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }

        CallbackResult blockStep(byte[] data, CallbackResult prev)//0x02 clientReq
        {
            UO.Say(".resync");
            return CallbackResult.Eat;
        }


    }
}
