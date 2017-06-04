using EreborPhoenixExtension.Libs;
using EreborPhoenixExtension.Libs.Extensions;
using EreborPhoenixExtension.Libs.Skills.Mining;
using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EreborPhoenixExtension
{
    public class Commands
    {



        #region Commands


        [Command]
        public void Record(string name)
        {
            if (Main.Instance.Settings.Mining == null)
            {
                XmlSerializeHelper<Mine> ss = new XmlSerializeHelper<Mine>();
                if (!ss.Load("Mining", out Main.Instance.Settings.Mining))
                    Main.Instance.Settings.Mining = new Mine();
            }
            Main.Instance.Settings.Mining.AddMap(name);
            new XmlSerializeHelper<Mine>().Save("Mining", Main.Instance.Settings.Mining);



        }
        [Command]
        public void mine()
        {
            //if (Main.Instance.Settings.Mining == null)
            //{
            //    XmlSerializeHelper<Mine> ss = new XmlSerializeHelper<Mine>();
            //    if (!ss.Load("Mining", out Main.Instance.Settings.Mining))
            //        Main.Instance.Settings.Mining = new Mine();
            //}
            Main.Instance.Settings.Mining.Work();
        }

        [Command]
        public void unload()
        {
            //if (Main.Instance.Settings.Mining == null)
            //{
            //    XmlSerializeHelper<Mine> ss = new XmlSerializeHelper<Mine>();
            //    if (!ss.Load("Mining", out Main.Instance.Settings.Mining))
            //        Main.Instance.Settings.Mining = new Mine();
            //}
            Main.Instance.Settings.Mining.Unload();
        }

        [Command]
        public void save()
        {
            XmlSerializeHelper<GameWIndoSize_DATA> gws = new XmlSerializeHelper<GameWIndoSize_DATA>();
            XmlSerializeHelper<Settings> sett = new XmlSerializeHelper<Settings>();
            XmlSerializeHelper<Mine> min = new XmlSerializeHelper<Mine>();

            gws.Save("WindowSize", Main.Instance.GWS_DATA);
            sett.Save(World.Player.Name, Main.Instance.Settings);
            if (Main.Instance.Settings.Mining != null) min.Save("Mining",( Main.Instance.Settings.Mining));
            UO.PrintInformation("Saved");
        }
        [Command]
        public void wall(bool energy)
        {
            int t = 10;
            Core.UnregisterServerMessageCallback(0x1C, Main.Instance.Settings.onSpellFizz);
            Core.RegisterServerMessageCallback(0x1C, Main.Instance.Settings.onSpellFizz);
            StaticTarget st = UIManager.Target();
            UO.WaitTargetTile(st.X, st.Y, st.Z, st.Graphic);
            if (energy)
            {
                UO.Cast(StandardSpell.EnergyField);
                while (t > 0)
                {
                    UO.Wait(500);
                    if (Main.Instance.Settings.Casting.SpellFizz)
                    {
                        Main.Instance.Settings.Casting.SpellFizz = false;
                        Core.UnregisterServerMessageCallback(0x1C, Main.Instance.Settings.onSpellFizz);
                        return;
                    }
                    t--;
                }
                Main.Instance.Settings.wallCnt.Add(st.X, st.Y, WallCounter.WallTime.Energy);
            }
            else
            {
                UO.Cast(StandardSpell.WallofStone);
                while (t > 0)
                {
                    UO.Wait(500);
                    if (Main.Instance.Settings.Casting.SpellFizz)
                    {
                        Main.Instance.Settings.Casting.SpellFizz = false;
                        Core.UnregisterServerMessageCallback(0x1C, Main.Instance.Settings.onSpellFizz);
                        return;
                    }
                    t--;
                }
                Main.Instance.Settings.wallCnt.Add(st.X, st.Y, WallCounter.WallTime.Stone);
            }
            Core.UnregisterServerMessageCallback(0x1C, Main.Instance.Settings.onSpellFizz);

        }

        [Command, BlockMultipleExecutions]
        public void kill()
        {
            Main.Instance.Settings.OtherAbilites.kill();
        }
        [Command, BlockMultipleExecutions]
        public void friend()
        {
            Main.Instance.Settings.OtherAbilites.frnd();
        }
        [Command]
        public void mesure(string command)
        {
            UO.Say(command);
            DateTime tt = DateTime.Now;
            while (World.Player.Hits == World.Player.MaxHits)
            {
                UO.Wait(50);
            }
            UO.Print((DateTime.Now - tt).TotalMilliseconds);
        }
        [Command]
        public void autoSacrafire()
        {
            if (Main.Instance.Settings.Casting.autoSacrafire)
            {
                UO.PrintInformation("Auto obet OFF");
                Main.Instance.Settings.Casting.autoSacrafire = false;
            }
            else
            {
                UO.PrintInformation("Auto obet ON");
                Main.Instance.Settings.Casting.autoSacrafire = true;
            }
        }
        [Command]
        public void obet()
        {
            Main.Instance.Settings.VooDoo.Sacrafire(Main.Instance.Settings.AHeal.bandage);
        }

        [Command]
        public void voodoo()
        {
            if (Main.Instance.Settings.VooDoo.VoDo) Main.Instance.Settings.VooDoo.VoDo = false;
            else Main.Instance.Settings.VooDoo.VoDo = true;
        }
        [Command]
        public void switchhotkeys()
        {
            Main.Instance.Settings.HotKeys.swHotk();
        }
        [Command]
        public void reactiv()
        {
            Main.Instance.Settings.Spells.ReactiveArmor(World.Player);
        }
        [Command]
        public void arrowself(bool b)
        {
            Main.Instance.Settings.Spells.arrowSelf(b);
        }
        [Command]
        public void potion(string type)
        {
            Main.Instance.Settings.OtherAbilites.potion(type);
        }
        [Command]
        public void potion(string type, int delay)
        {
            Main.Instance.Settings.OtherAbilites.potion(type, delay);
        }
        [Command]
        public void attacklast()
        {
            Main.Instance.Settings.OtherAbilites.attackLast();
        }

        [Command]
        public void war()
        {
            Main.Instance.Settings.OtherAbilites.war();
        }
        [Command, BlockMultipleExecutions]
        public void probo()
        {
            Main.Instance.Settings.HitBandage = false;
            Main.Instance.Settings.OtherAbilites.probo(Main.Instance.Settings.HiddenTime);
            Main.Instance.Settings.HitBandage = true;
        }
        [Command, BlockMultipleExecutions]
        public void kudla()
        {
            Main.Instance.Settings.OtherAbilites.kudla();
        }

        [Command]
        public void Morf(ushort to)
        {
            Main.Instance.Settings.Amorf.MorfTo(to);
        }
        [Command]
        public void OnOff()
        {
            if (Main.Instance.Settings.AHeal.OnOff) Main.Instance.Settings.AHeal.OnOff = false;
            else
            {
                Main.Instance.Settings.AHeal.OnOff = true;
                if (Main.Instance.Settings.ActualCharacter == Character.EreborClass.Null)
                {
                    Character.ClassKnown += Character_ClassKnown;
                    GetClass Gc = new GetClass(Character.GetClass);
                    Gc.BeginInvoke(null, null);
                }
                UO.Print(Main.Instance.Settings.AHeal.HealedPlayers.Count);
            }
        }

        private void Character_ClassKnown(object sender, Character.ClassReturnArgs e)
        {
            Character.ClassKnown -= Character_ClassKnown;
            Main.Instance.Settings.ActualCharacter = e.ActualClass;
            switch (Main.Instance.Settings.ActualCharacter)
            {
                case Character.EreborClass.Priest:
                    Main.Instance.Settings.CrystalCmd = ".enlightment";
                    Main.Instance.Settings.HealCmd = ".heal";
                    Main.Instance.Settings.AHeal.PatientHPLimit = 87;
                    break;
                case Character.EreborClass.Shaman:
                    Main.Instance.Settings.CrystalCmd = ".improvement";
                    Main.Instance.Settings.HealCmd = ".samheal";
                    Main.Instance.Settings.AHeal.PatientHPLimit = 90;
                    break;

            }
            UO.Print(Main.Instance.Settings.CrystalCmd);
        }

        [Command]
        public void bandage()
        {
            if (Main.Instance.Settings.ActualCharacter == Character.EreborClass.Null)
            {
                Character.ClassKnown += Character_ClassKnown;
                GetClass Gc = new GetClass(Character.GetClass);
                Gc.BeginInvoke(null, null);
            }
            Main.Instance.Settings.AHeal.bandage();
            Main.Instance.Settings.Weapons.ActualWeapon.Equip();
        }


        [Command, BlockMultipleExecutions]
        public void pois()
        {
            Main.Instance.Settings.Poisoning.pois();
        }

        [Command]
        public void nhcast(string s, Serial t)
        {
            if (Main.Instance.Settings.arrowSelfProgress) return;
            bool tmp = false;
            if (Main.Instance.Settings.AHeal.OnOff)
            {
                Main.Instance.Settings.AHeal.OnOff = false;
                tmp = true;
            }


            Main.Instance.Settings.Casting.ccast(s, t, Main.Instance.Settings.AHeal.bandage, this.obet);
            UO.Wait(Main.Instance.Settings.Casting.SpellsDelays[s]);
            if (tmp == true && Main.Instance.Settings.AHeal.OnOff == false) Main.Instance.Settings.AHeal.OnOff = true;
        }
        [Command]
        public void ccast(string s)
        {
            if (Main.Instance.Settings.arrowSelfProgress) return;
            Main.Instance.Settings.Casting.ccast(s, Main.Instance.Settings.AHeal.bandage, this.obet);
        }
        [Command]
        public void ccast(string s, Serial t)
        {
            if (Main.Instance.Settings.arrowSelfProgress) return;
            if (Aliases.LastAttack != t) UO.Attack(t);
            Main.Instance.Settings.Casting.ccast(s, t, Main.Instance.Settings.AHeal.bandage, this.obet);
        }
        [Command]
        public void autocast(bool b)
        {
            Main.Instance.Settings.Casting.autocast(b, Main.Instance.Settings.AHeal.bandage, this.obet);
        }
        [Command]
        public void hidoff()
        {
            Main.Instance.Settings.Hiding.hidoff();
        }
        [Command, BlockMultipleExecutions]
        public void hid()
        {
            Main.Instance.Settings.Hiding.hiding(true);

        }
        [Command("exp")]
        public void getExp()
        {
            UO.Print(Main.Instance.Settings.exp);
        }
        [Command]
        public void inv()
        {
            Main.Instance.Settings.Spells.inv(Main.Instance.Settings.Casting.ccast, Main.Instance.Settings.AHeal.bandage, Main.Instance.Settings.Casting.SpellsDelays["Invis"]);
        }

        [Command("switch")]
        public void swit()
        {
            Main.Instance.Settings.Weapons.SwitchWeapons();
        }
        [Command, BlockMultipleExecutions]
        public void music(bool b)
        {
            Main.Instance.Settings.Peace_Entic.music(b);
        }

        [Command]
        public void kuch()
        {
            Main.Instance.Settings.Lot.Carving();
            Main.Instance.Settings.Weapons.ActualWeapon.Equip();
        }

        [Command]
        public void track()
        {
            Main.Instance.Settings.Track.Track();
        }
        [Command]
        public void track(int choice)
        {
            Main.Instance.Settings.Track.Track(choice);
        }
        [Command]
        public void track(string var)
        {
            Main.Instance.Settings.Track.Track(var);
        }
        [Command]
        public void track(int choice, bool war)
        {
            Main.Instance.Settings.Track.Track(choice, war);
        }
        [Command]
        public void add(string name)
        {
            Main.Instance.Settings.Track.Add(name);

        }
        [Command, BlockMultipleExecutions]
        public void autopetheal()
        {
            Main.Instance.Settings.Vete.autoVet();
        }

        [Command, BlockMultipleExecutions]
        public void petheal()
        {
            Main.Instance.Settings.Vete.Vet();
        }
        [Command]
        public void harm()
        {
            UO.Attack(Aliases.GetObject("laststatus"));
            UO.Cast(StandardSpell.Harm, Aliases.GetObject("laststatus"));
        }
        #endregion
    }
}
