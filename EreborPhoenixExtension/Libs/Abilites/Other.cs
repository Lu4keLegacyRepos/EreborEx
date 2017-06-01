using EreborPhoenixExtension.Libs.Weapons;
using Phoenix;

using Phoenix.WorldData;
using System;

namespace EreborPhoenixExtension.Libs.Abilites
{
    public class Other
    {

        public bool potionDelay;
        Action makeStepOFF;
        private WeaponSet ActualWeapon;

        public Other(Action makeStepOff,  WeaponsHandle weap)
        {
            ActualWeapon = weap.ActualWeapon;
            makeStepOFF = makeStepOff;
        }

        public void war()
        {
            if (World.Player.Warmode)
            {
                UO.Warmode(false);
                UO.PrintInformation("Warmode off");
            }

            else
            {
                UO.Warmode(true);
                UO.PrintInformation("Warmode on");
            }
        }



        public void attackLast()
        {
            UO.Attack(Aliases.GetObject("laststatus"));
        }

        public void potion(string type)
        {
            potion(type, 22);
        }

        public void potion(string type, int delaySec)
        {
            if (potionDelay) UO.PrintError("NEMUZES PIT!!");
            else
            {
                DateTime startTime = DateTime.Now;
                UO.Say(".potion" + type);
                potionDelay = true;
                while (DateTime.Now - startTime < TimeSpan.FromSeconds(delaySec))
                {
                    if (World.Player.Dead) break;
                    UO.Wait(500);
                }
                UO.PrintInformation("Muzes Pit !!!");
                potionDelay = false;

            }
        }

        public void probo(DateTime HiddenTime)
        {
            UOCharacter target = new UOCharacter(Aliases.GetObject("laststatus"));
            bool first = true;
            while (World.Player.Hidden)
            {

                UO.Wait(100);
                if (!target.Serial.Equals(Aliases.GetObject("laststatus")))
                    target = new UOCharacter(Aliases.GetObject("laststatus"));
                if (DateTime.Now - HiddenTime < TimeSpan.FromSeconds(3)) continue;
                if (first)
                {
                    UO.PrintError("Muzes Bodat!");
                    first = false;
                }
                if (target.Distance < 2)
                {
                    target.WaitTarget();
                    UO.Say(".usehand");
                    UO.Wait(200);
                }

            }
            makeStepOFF();
            UO.Say(",hid");

        }


        public void kudla()
        {
            UOCharacter cil = new UOCharacter(Aliases.GetObject("laststatus"));
            if (cil.Distance > 6)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            UO.Say(".throw");
            ActualWeapon.Equip();
            if (Journal.WaitForText(true, 1000, "Nemas zadny cil.", "Nevidis na cil"))
            {
                UO.PrintInformation("HAZ!!");
                return;
            }
            UO.Wait(1000);
            UO.PrintInformation("3");
            UO.Wait(1000);
            UO.PrintInformation("2");
            UO.Wait(1000);
            UO.PrintInformation("1");
            UO.Wait(1000);
            UO.PrintInformation("HAZ!!");
            World.Player.Print("Hazej!!!");

        }

        public void frnd()
        {
            foreach (UOCharacter sum in World.Characters)
            {
                if (sum.Renamable || sum.Notoriety == Notoriety.Innocent)
                {
                    UO.Say("all friend");
                    UO.WaitTargetObject(sum.Serial);
                    UO.Wait(100);
                }
                UO.Wait(100);
            }
        }


        public void kill()
        {
            int wait = 100;
            foreach (UOCharacter enemy in World.Characters)
            {
                if (enemy.Renamable) continue;
                if (enemy.Distance > 11) continue;
                if (enemy.Notoriety == Notoriety.Enemy || enemy.Notoriety == Notoriety.Murderer)
                {
                    UO.Say("all kill");
                    UO.WaitTargetObject(enemy.Serial);
                    UO.Wait(wait = wait + 25);
                }
                UO.Wait(100);
            }
        }

    }
}
