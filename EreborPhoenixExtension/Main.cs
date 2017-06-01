using EreborPhoenixExtension.GUI;
using EreborPhoenixExtension.Libs;
using EreborPhoenixExtension.Libs.Extensions;
using EreborPhoenixExtension.Libs.Skills.Mining;
using Phoenix;
using Phoenix.WorldData;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EreborPhoenixExtension
{
    public delegate void Load();
    [RuntimeObject]
    public class Main
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        public static Main Instance;
        private int GWHeight;
        private int GWWidth;
        public Settings Settings;
        private readonly GameWindowSize patch;


        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        public Main()
        {
            Instance = this;
            Instance.Settings = (new Settings().Deserialize("WindowSize"));
            Instance.Settings.SetWindow(out GWWidth,out GWHeight);
            try
            {
                patch = new GameWindowSize(GWWidth, GWHeight);
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            Core.LoginComplete += Core_LoginComplete;
            Core.Disconnected += Core_Disconnected;
            
            
        }

        private void Core_Disconnected(object sender, EventArgs e)
        {
            Core.Disconnected -= Core_Disconnected;
            save();
            Instance.Settings = null;
            Core.LoginComplete += Core_LoginComplete;
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onBandageDone);
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onCrystal);
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onExp);
            Core.UnregisterServerMessageCallback(0xa1, Instance.Settings.onHpChanged);
            Core.UnregisterServerMessageCallback(0x11, Instance.Settings.OnNextTarget);
            World.Player.Changed -= Player_Changed;
            Instance.Settings.Ev.hiddenChange -= Ev_hiddenChange;
            Instance.Settings.Ev.hitsChanged -= Ev_hitsChanged;
            //Instance.Settings.AHeal.PatientHurted -= AHeal_PatientHurted;

            Instance = null;

        }

        private void Core_LoginComplete(object sender, EventArgs e)
        {
            Core.LoginComplete -= Core_LoginComplete;
            ShowWindowAsync(Client.HWND, SW_SHOWNORMAL);
            ShowWindowAsync(Client.HWND, SW_SHOWMAXIMIZED);
            Erebor.SetForegroundWindow(Client.HWND);
            if (Instance == null)
            {
                Instance = new Main();
                return;
            }



            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onBandageDone);
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onCrystal);
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onExp);
            Core.UnregisterServerMessageCallback(0xa1, Instance.Settings.onHpChanged);
            Core.UnregisterServerMessageCallback(0x11, Instance.Settings.OnNextTarget);
            Core.RegisterServerMessageCallback(0x1C, Instance.Settings.onBandageDone);
            Core.RegisterServerMessageCallback(0x1C, Instance.Settings.onCrystal);
            Core.RegisterServerMessageCallback(0x1C, Instance.Settings.onExp);
            Core.RegisterServerMessageCallback(0xa1, Instance.Settings.onHpChanged);
            Core.RegisterServerMessageCallback(0x11, Instance.Settings.OnNextTarget);




            //nacist GUI
            World.Player.RequestStatus(100);
            World.Player.Click();


            UO.PrintInformation("Loading done");
            new Thread(setEQ).Start();


            World.Player.Changed += Player_Changed;
            Instance.Settings.Ev.hiddenChange += Ev_hiddenChange;
            Instance.Settings.Ev.hitsChanged += Ev_hitsChanged;
            Instance.Settings.AHeal.PatientHurted += AHeal_PatientHurted;
            Instance.Settings.Set(World.Player.Name);
            Erebor.Instance.BeginInvoke(new CheckAll(Erebor.Instance.CheckAll));
        }
        private void Load()
        {
            while (World.Player.Name == null) UO.Wait(200);
            Instance.Settings = new Settings().Deserialize(World.Player.Name.ToString());//  Instance.Settings.Deserialize(World.Player.Name.ToString());
            Erebor.Instance.BeginInvoke(new CheckAll(Erebor.Instance.CheckAll));
        }
        private void RuntimeCore_UnregisteringAssembly(object sender, Phoenix.Runtime.UnregisteringAssemblyEventArgs e)
        {
            Debug.WriteLine("Ukladam");
            Instance.save();
        }

        private void setEQ()
        {
            UO.Wait(1000);
            World.Player.Click();
            UO.Wait(1000);
            World.Player.WaitTarget();
            UO.Say(".setequip15");

        }
        private void AHeal_PatientHurted(object sender, Libs.Healing.HurtedPatientArgs e)
        {
            Instance.Settings.AHeal.PatientHurted -= AHeal_PatientHurted;
            if (e.pati != null)
            {
                Instance.Settings.AHeal.bandage(e.pati);
            }
            if (e.selfHurted)
            {
                Instance.Settings.AHeal.bandage();
            }
            if (e.crystalOff)
            {
                Instance.Settings.CrystallCnt++;
                if (Instance.Settings.CrystallCnt >= 5 && Instance.Settings.crystalState)
                {

                    Instance.Settings.CrystallCnt = 0;
                    UO.Say(Instance.Settings.CrystalCmd);
                    Instance.Settings.AHeal.GetStatuses();
                    Instance.Settings.Weapons.ActualWeapon.Equip();
                }
            }
            Instance.Settings.AHeal.PatientHurted += AHeal_PatientHurted;
        }

        private void Ev_hitsChanged(object sender, Libs.Events.HitsChangedArgs e)
        {
            if (!e.gain) Instance.Settings.Hiding.getHit = true;
            ushort[] color = new ushort[4];
            color[0] = 0x0026;//red
            color[2] = 0x0175;//green
            color[1] = 0x099;//yellow
            color[3] = 0x0FAB;//fialova - enemy;
            if (e.amount > 3)
            {
                World.Player.Print(e.poison ? color[2] : e.gain ? color[1] : color[0], "[{0} HP]  {1}{2}", World.Player.Hits, e.gain ? "+" : "-", e.amount);
            }
        }

        private void Ev_hiddenChange(object sender, Libs.Events.HiddenChangeArgs e)
        {
            if (!e.hiddenState)
            {
                Instance.Settings.Hiding.hidoff();
                UO.Say(".resync");
                UO.PrintError("resync");
                UO.Wait(200);
            }
            else Instance.Settings.HiddenTime = DateTime.Now;
        }

        private void Player_Changed(object sender, ObjectChangedEventArgs e)
        {
            World.Player.Changed -= Player_Changed;
            if (World.Player.Hits <= Instance.Settings.criticalHits && World.Player.Hits >= 1 && !Instance.Settings.OtherAbilites.potionDelay && Instance.Settings.AutoDrink)
            {

                UO.Say(",potion heal");
            }
            string tmp = PrintState();
            if (tmp != Client.Text)
                Client.Text = tmp;
            if (new UOItem(Instance.Settings.Lot.LotBag).AllItems.FindType(0x0eed).Amount > Instance.Settings.GoldLimit && Instance.Settings.GoldLimit != 0)
            {
                UO.Say(".mesec");

            }
            UO.Wait(100);
            World.Player.Changed += Player_Changed;

        }


        public static string PrintState()
        {
            UOPlayer p = World.Player;
            string temp = "";
            temp = "HP: " + p.Hits + "/" + p.MaxHits //11
                + "  ||  Mana: " + p.Mana + "/" + p.MaxMana//19
                + "  ||  Stamina: " + p.Stamina + "/" + p.MaxStamina//22
                + "  ||  Sila: " + p.Strenght //15
                + "  ||  Stealth: " + p.Skills["Stealth"].RealValue / 10 //18
                + "  ||  Weight: " + p.Weight + "/" + p.MaxWeight//21
                + "  ||  Armor: " + p.Armor //16
                + "  ||  Gold: " + p.Gold;//20  =109-->110+110=220 124-->125+125=290

            if (World.GetCharacter(Aliases.GetObject("laststatus")).Distance < 20 && World.GetCharacter(Aliases.GetObject("laststatus")).Hits > -1)
            {
                if (temp.Length < 145)
                {
                    for (int i = 0; i < 145 - temp.Length; i++)
                    {
                        temp += " ";
                    }
                    temp += "                              ";//40 znaku
                    temp += "                              ";//40 znaku
                }
                temp += World.GetCharacter(Aliases.GetObject("laststatus")).Name
                    + ": "
                    + World.GetCharacter(Aliases.GetObject("laststatus")).Hits
                    + "/"
                    + World.GetCharacter(Aliases.GetObject("laststatus")).MaxHits
                    + "   Distance: "
                    + World.GetCharacter(Aliases.GetObject("laststatus")).Distance;

            }
            return temp;
        }
        #region Commands


        [Command]
        public void Record(string name)
        {
            if(Instance.Settings.Mining ==null)
            {
                Instance.Settings.Mining = (new Mine().Deserialize());

            }

            Instance.Settings.Mining.AddMap(name);
            Instance.Settings.Mining.Serialize();


        }
        [Command]
        public void mine()
        {
            if (Instance.Settings.Mining == null)
            {
                Instance.Settings.Mining = (new Mine().Deserialize());

            }
           // Instance.Settings.Mining.SelectMap();// TODO odstranit 
            Instance.Settings.Mining.Work();
        }


        [Command]
        public void save()
        {
            Instance.Settings.Serialize(World.Player.Name.ToString());
            Instance.Settings.Serialize("WindowSize");
            UO.PrintInformation("Saved");
        }
        [Command]
        public void wall(bool energy)
        {
            int t = 10;
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onSpellFizz);
            Core.RegisterServerMessageCallback(0x1C, Instance.Settings.onSpellFizz);
            StaticTarget st = UIManager.Target();
            UO.WaitTargetTile(st.X, st.Y, st.Z, st.Graphic);
            if (energy)
            {
                UO.Cast(StandardSpell.EnergyField);
                while (t > 0)
                {
                    UO.Wait(500);
                    if (Instance.Settings.Casting.SpellFizz)
                    {
                        Instance.Settings.Casting.SpellFizz = false;
                        Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onSpellFizz);
                        return;
                    }
                    t--;
                }
                Instance.Settings.wallCnt.Add(st.X, st.Y, WallCounter.WallTime.Energy);
            }
            else
            {
                UO.Cast(StandardSpell.WallofStone);
                while (t > 0)
                {
                    UO.Wait(500);
                    if (Instance.Settings.Casting.SpellFizz)
                    {
                        Instance.Settings.Casting.SpellFizz = false;
                        Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onSpellFizz);
                        return;
                    }
                    t--;
                }
                Instance.Settings.wallCnt.Add(st.X, st.Y, WallCounter.WallTime.Stone);
            }
            Core.UnregisterServerMessageCallback(0x1C, Instance.Settings.onSpellFizz);

        }

        [Command, BlockMultipleExecutions]
        public void kill()
        {
            Instance.Settings.OtherAbilites.kill();
        }
        [Command, BlockMultipleExecutions]
        public void friend()
        {
            Instance.Settings.OtherAbilites.frnd();
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
            if (Instance.Settings.Casting.autoSacrafire)
            {
                UO.PrintInformation("Auto obet OFF");
                Instance.Settings.Casting.autoSacrafire = false;
            }
            else
            {
                UO.PrintInformation("Auto obet ON");
                Instance.Settings.Casting.autoSacrafire = true;
            }
        }
        [Command]
        public void obet()
        {
            Instance.Settings.VooDoo.Sacrafire(Instance.Settings.AHeal.bandage);
        }

        [Command]
        public void voodoo()
        {
            if (Instance.Settings.VooDoo.VoDo) Instance.Settings.VooDoo.VoDo = false;
            else Instance.Settings.VooDoo.VoDo = true;
        }
        [Command]
        public void switchhotkeys()
        {
            Instance.Settings.HotKeys.swHotk();
        }
        [Command]
        public void reactiv()
        {
            Instance.Settings.Spells.ReactiveArmor(World.Player);
        }
        [Command]
        public void arrowself(bool b)
        {
            Instance.Settings.Spells.arrowSelf(b);
        }
        [Command]
        public void potion(string type)
        {
            Instance.Settings.OtherAbilites.potion(type);
        }
        [Command]
        public void potion(string type, int delay)
        {
            Instance.Settings.OtherAbilites.potion(type, delay);
        }
        [Command]
        public void attacklast()
        {
            Instance.Settings.OtherAbilites.attackLast();
        }

        [Command]
        public void war()
        {
            Instance.Settings.OtherAbilites.war();
        }
        [Command, BlockMultipleExecutions]
        public void probo()
        {
            Instance.Settings.HitBandage = false;
            Instance.Settings.OtherAbilites.probo(Instance.Settings.HiddenTime);
            Instance.Settings.HitBandage = true;
        }
        [Command, BlockMultipleExecutions]
        public void kudla()
        {
            Instance.Settings.OtherAbilites.kudla();
        }

        [Command]
        public void Morf(ushort to)
        {
            Instance.Settings.Amorf.MorfTo(to);
        }
        [Command]
        public void OnOff()
        {
            if (Instance.Settings.AHeal.OnOff) Instance.Settings.AHeal.OnOff = false;
            else
            {
                Instance.Settings.AHeal.OnOff = true;
                if (Instance.Settings.ActualCharacter == Character.EreborClass.Null)
                {
                    Character.ClassKnown += Character_ClassKnown;
                    GetClass Gc = new GetClass(Character.GetClass);
                    Gc.BeginInvoke(null, null);
                }
                UO.Print(Instance.Settings.AHeal.HealedPlayers.Count);
            }
        }

        private void Character_ClassKnown(object sender, Character.ClassReturnArgs e)
        {
            Character.ClassKnown -= Character_ClassKnown;
            Instance.Settings.ActualCharacter = e.ActualClass;
            switch (Instance.Settings.ActualCharacter)
            {
                case Character.EreborClass.Priest:
                    Instance.Settings.CrystalCmd = ".enlightment";
                    Instance.Settings.HealCmd = ".heal";
                    Instance.Settings.AHeal.PatientHPLimit = 87;
                    break;
                case Character.EreborClass.Shaman:
                    Instance.Settings.CrystalCmd = ".improvement";
                    Instance.Settings.HealCmd = ".samheal";
                    Instance.Settings.AHeal.PatientHPLimit = 90;
                    break;

            }
            UO.Print(Instance.Settings.CrystalCmd);
        }

        [Command]
        public void bandage()
        {
            if (Instance.Settings.ActualCharacter == Character.EreborClass.Null)
            {
                Character.ClassKnown += Character_ClassKnown;
                GetClass Gc = new GetClass(Character.GetClass);
                Gc.BeginInvoke(null, null);
            }
            Instance.Settings.AHeal.bandage();
            Instance.Settings.Weapons.ActualWeapon.Equip();
        }


        [Command, BlockMultipleExecutions]
        public void pois()
        {
            Instance.Settings.Poisoning.pois();
        }

        [Command]
        public void nhcast(string s, Serial t)
        {
            if (Instance.Settings.arrowSelfProgress) return;
            bool tmp = false;
            if (Instance.Settings.AHeal.OnOff)
            {
                Instance.Settings.AHeal.OnOff = false;
                tmp = true;
            }


            Instance.Settings.Casting.ccast(s, t, Instance.Settings.AHeal.bandage, this.obet);
            UO.Wait(Instance.Settings.Casting.SpellsDelays[s]);
            if (tmp == true && Instance.Settings.AHeal.OnOff == false) Instance.Settings.AHeal.OnOff = true;
        }
        [Command]
        public void ccast(string s)
        {
            if (Instance.Settings.arrowSelfProgress) return;
            Instance.Settings.Casting.ccast(s, Instance.Settings.AHeal.bandage, this.obet);
        }
        [Command]
        public void ccast(string s, Serial t)
        {
            if (Instance.Settings.arrowSelfProgress) return;
            if (Aliases.LastAttack != t) UO.Attack(t);
            Instance.Settings.Casting.ccast(s, t, Instance.Settings.AHeal.bandage, this.obet);
        }
        [Command]
        public void autocast(bool b)
        {
            Instance.Settings.Casting.autocast(b, Instance.Settings.AHeal.bandage, this.obet);
        }
        [Command]
        public void hidoff()
        {
            Instance.Settings.Hiding.hidoff();
        }
        [Command, BlockMultipleExecutions]
        public void hid()
        {
            Instance.Settings.Hiding.hiding(true);

        }
        [Command("exp")]
        public void getExp()
        {
            UO.Print(Instance.Settings.exp);
        }
        [Command]
        public void inv()
        {
            Instance.Settings.Spells.inv(Instance.Settings.Casting.ccast, Instance.Settings.AHeal.bandage, Instance.Settings.Casting.SpellsDelays["Invis"]);
        }

        [Command("switch")]
        public void swit()
        {
            Instance.Settings.Weapons.SwitchWeapons();
        }
        [Command, BlockMultipleExecutions]
        public void music(bool b)
        {
            Instance.Settings.Peace_Entic.music(b);
        }

        [Command]
        public void kuch()
        {
            Instance.Settings.Lot.Carving();
            Instance.Settings.Weapons.ActualWeapon.Equip();
        }

        [Command]
        public void track()
        {
            Instance.Settings.Track.Track();
        }
        [Command]
        public void track(int choice)
        {
            Instance.Settings.Track.Track(choice);
        }
        [Command]
        public void track(string var)
        {
            Instance.Settings.Track.Track(var);
        }
        [Command]
        public void track(int choice, bool war)
        {
            Instance.Settings.Track.Track(choice, war);
        }
        [Command]
        public void add(string name)
        {
            Instance.Settings.Track.Add(name);

        }
        [Command, BlockMultipleExecutions]
        public void autopetheal()
        {
            Instance.Settings.Vete.autoVet();
        }

        [Command, BlockMultipleExecutions]
        public void petheal()
        {
            Instance.Settings.Vete.Vet();
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
