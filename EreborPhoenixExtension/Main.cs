using EreborPhoenixExtension.GUI;
using EreborPhoenixExtension.Libs;
using EreborPhoenixExtension.Libs.Extensions;
using EreborPhoenixExtension.Libs.Runes;
using EreborPhoenixExtension.Libs.Skills.Mining;
using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace EreborPhoenixExtension
{

    public class Main
    {
        System.Timers.Timer t;
        private int X = World.Player.X;
        private int Y=World.Player.Y;
        public Erebor EreborInstance;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        public GameWindowSize GWS;
        public Settings Settings;

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);



        private static Main instance;

        private Main() {

            XmlSerializeHelper<GameWindowSize> gw = new XmlSerializeHelper<GameWindowSize>();
            if (!gw.Load("WindowSize", out GWS))
            {
                GWS = new GameWindowSize(); //TODO save proc null ?
                GWS.Width = 800;
                GWS.Height = 600;

            }
            GWS.Patch();

            t = new System.Timers.Timer(500);
            t.Elapsed += T_Elapsed;
            t.Start();
            ShowWindowAsync(Client.HWND, SW_SHOWNORMAL);
            ShowWindowAsync(Client.HWND, SW_SHOWMAXIMIZED);
            Erebor.SetForegroundWindow(Client.HWND);

            World.Player.RequestStatus(100);
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(World.Player.Name!=null)
            {
                
                t.Stop();
                Initialize();
            }
        }

        public static Main Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Main();
                }
                return instance;
            }
        }






        private void Initialize()
        {
            XmlSerializeHelper<Settings> sett = new XmlSerializeHelper<Settings>();
            

  
            new Thread(setEQ).Start();

            if (!sett.Load(World.Player.Name, out Settings))
                Settings = new Settings();

            World.Player.Changed += Player_Changed;
            Instance.Settings.Ev.hiddenChange += Ev_hiddenChange;
            Instance.Settings.Ev.hitsChanged += Ev_hitsChanged;
            Instance.Settings.AHeal.PatientHurted += AHeal_PatientHurted;


            Instance.EreborInstance.Changed += EreborInstance_Changed;
            UO.PrintInformation("Loading done");


            #region Init GUI
            Instance.EreborInstance.Food = Instance.Settings.Lot.Food;
            Instance.EreborInstance.Leather = Instance.Settings.Lot.Leather;
            Instance.EreborInstance.Bolts = Instance.Settings.Lot.Bolts;
            Instance.EreborInstance.Extend1 = Instance.Settings.Lot.Extend1;
            Instance.EreborInstance.Extend2 = Instance.Settings.Lot.Extend2;
            Instance.EreborInstance.Lot = Instance.Settings.Lot.DoLot;
            Instance.EreborInstance.Feathers = Instance.Settings.Lot.Feathers;
            Instance.EreborInstance.Gems = Instance.Settings.Lot.Gems;
            Instance.EreborInstance.Regeants = Instance.Settings.Lot.Reageants;
            Instance.EreborInstance.CorpsesHide = Instance.Settings.Lot.HideCorpses;
            Instance.EreborInstance.AutoMorf = Instance.Settings.Amorf.Amorf;
            Instance.EreborInstance.HitBandage = Instance.Settings.HitBandage;
            Instance.EreborInstance.HitTrack = Instance.Settings.HitTrack;
            Instance.EreborInstance.AutoArrow = Instance.Settings.Spells.AutoArrow;
            Instance.EreborInstance.AutoDrink = Instance.Settings.AutoDrink;
            Instance.EreborInstance.StoodUps = Instance.Settings.PrintAnim;

            #endregion

        }
        #region GUI Function
        private void EreborInstance_Changed(object sender, EventChangedArgs e)
        {
            //((Erebor)sender).BeginInvoke(new MethodInvoker(EreborInstance.tt));
            int tmp;
            string tmps;
            bool tmpb;
            SetForegroundWindow(Client.HWND);
            if (!e.btnName.Equals(string.Empty))
            {
                switch (e.btnName)
                {
                    case "btn_0":
                        switch (e.SelectedTabID)
                        {
                            // Runes
                            case 0:
                                foreach (Rune r in Instance.Settings.RuneTree.Runes.Where(run => run.Id.ToString() == Instance.EreborInstance.SelectedRuneID))
                                {
                                    Instance.Settings.RuneTree.findRune(r);
                                    r.RecallSvitek();
                                }
                                break;
                            // Equip Sets
                            case 1:
                                tmp = Instance.EreborInstance.SelectedEquip;
                                if (tmp >= 0)
                                {
                                    Instance.Settings.EquipSet.equipy[tmp].DressOnly();
                                }
                                break;
                        }

                        break;
                    case "btn_1":
                        switch (e.SelectedTabID)
                        {
                            // Runes
                            case 0:
                                foreach (Rune r in Instance.Settings.RuneTree.Runes.Where(run => run.Id.ToString() == Instance.EreborInstance.SelectedRuneID))
                                {
                                    Instance.Settings.RuneTree.findRune(r);
                                    r.Recall();
                                }
                                break;
                            // Equip Sets
                            case 1:
                                tmp = Instance.EreborInstance.SelectedEquip;
                                if (tmp >= 0)
                                {
                                    UO.PrintInformation("Zamer odkladaci batoh");
                                    Instance.Settings.EquipSet.equipy[tmp].Dress(new UOItem(UIManager.TargetObject()));
                                }
                                break;
                        }
                        break;
                    case "btn_2":
                        switch (e.SelectedTabID)
                        {
                            // Runes
                            case 0:
                                foreach (Rune r in Instance.Settings.RuneTree.Runes.Where(run => run.Id.ToString() == Instance.EreborInstance.SelectedRuneID))
                                {
                                    Instance.Settings.RuneTree.findRune(r);
                                    r.Gate();
                                }
                                break;
                            // Equip Sets
                            case 1:
                                tmp = Instance.EreborInstance.SelectedEquip;
                                if (tmp >= 0)
                                {
                                    instance.Settings.EquipSet.Remove(tmp);
                                    instance.Settings.EquipSet.fillListBox(instance.EreborInstance.EquipList);
                                }
                                break;
                            // Weapons
                            case 2:
                                tmp = Instance.EreborInstance.SelectedWeapon;
                                if (tmp >= 0)
                                {
                                    instance.Settings.Weapons.Remove(tmp);
                                    instance.Settings.Weapons.fillListBox(instance.EreborInstance.WeaponList);
                                }
                                break;
                            // Healed
                            case 3:
                                tmp = Instance.EreborInstance.SelectedHealed;
                                if (tmp >= 0)
                                {
                                    instance.Settings.AHeal.Remove(tmp);
                                    instance.Settings.AHeal.fillListBox(instance.EreborInstance.HealList);
                                }
                                break;
                            // TrackIgnore
                            case 4:
                                tmp = Instance.EreborInstance.SelectedIgnored;
                                if (tmp >= 0)
                                {
                                    instance.Settings.Track.Remove(tmp);
                                    instance.Settings.Track.fillListBox(instance.EreborInstance.TrackIgnoreList);
                                }
                                break;
                            case 5:
                                break;

                        }
                        break;
                    case "btn_3":
                        switch (e.SelectedTabID)
                        {
                            // Runes
                            case 0:
                                Instance.Settings.RuneTree.GetRunes();
                                Instance.Settings.RuneTree.FillTreeView(Instance.EreborInstance.RuneView);
                                break;
                            // Equip Sets
                            case 1:
                                UO.PrintWarning("Zamer bagl s equipem");
                                instance.Settings.EquipSet.Add();
                                instance.Settings.EquipSet.fillListBox(instance.EreborInstance.EquipList);
                                break;
                            // Weapons
                            case 2:
                                UO.PrintWarning("Zamer zbran a stit");
                                instance.Settings.Weapons.Add();
                                instance.Settings.Weapons.fillListBox(instance.EreborInstance.WeaponList);
                                break;
                            // Healed
                            case 3:
                                UO.PrintWarning("Zamer koho lecit");
                                instance.Settings.AHeal.Add();
                                instance.Settings.AHeal.fillListBox(instance.EreborInstance.HealList);
                                break;
                            // TrackIgnore
                            case 4:
                                UO.PrintWarning("Zamer koho ignorovat pri Trackingu");
                                instance.Settings.Track.Add();
                                instance.Settings.Track.fillListBox(instance.EreborInstance.TrackIgnoreList);
                                break;
                            case 5:
                                break;

                        }
                        break;
                    case "btn_4":
                        switch (e.SelectedTabID)
                        {
                            // Runes
                            case 0:
                                Instance.Settings.RuneTree.FillTreeView(Instance.EreborInstance.RuneView);
                                break;
                            // Equip Sets
                            case 1:
                                instance.Settings.EquipSet.fillListBox(instance.EreborInstance.EquipList);
                                break;
                            // Weapons
                            case 2:
                                instance.Settings.Weapons.fillListBox(instance.EreborInstance.WeaponList);
                                break;
                            // Healed
                            case 3:
                                instance.Settings.AHeal.fillListBox(instance.EreborInstance.HealList);
                                break;
                            // TrackIgnore
                            case 4:
                                instance.Settings.Track.fillListBox(instance.EreborInstance.TrackIgnoreList);

                                break;
                            case 5:
                                break;

                        }
                        break;
                    case "btn_AddHotkeys":
                        Instance.Settings.HotKeys.Add();
                        break;
                    case "btn_ClearHotkeys":
                        Instance.Settings.HotKeys.Clear();
                        break;
                    case "btn_Extend1":
                        UO.PrintWarning("Zamer Item");
                        Instance.Settings.Lot.extend1_type = new UOItem(UIManager.TargetObject()).Graphic;
                        Instance.EreborInstance.Extend1Type_Text = Instance.Settings.Lot.extend1_type.ToString();
                        break;
                    case "btn_Extend2":
                        UO.PrintWarning("Zamer Item");
                        Instance.Settings.Lot.extend1_type = new UOItem(UIManager.TargetObject()).Graphic;
                        Instance.EreborInstance.Extend2Type_Text = Instance.Settings.Lot.extend2_type.ToString();
                        break;
                    case "btn_SetBag":
                        UO.PrintWarning("Zamer Lot Baglik");
                        Instance.Settings.Lot.LotBag = new UOItem(UIManager.TargetObject());
                        break;
                    case "btn_SetCarv":
                        UO.PrintWarning("Zamer Nuz");
                        Instance.Settings.Lot.CarvTool = new UOItem(UIManager.TargetObject());
                        break;
                    case "btn_Pois":
                        UO.PrintInformation("Zamer Poison");
                        Instance.Settings.Poisoning.PoisonBottle = new UOItem(UIManager.TargetObject());
                        UOItem pois = new UOItem(Instance.Settings.Poisoning.PoisonBottle);
                        pois.Click();
                        UO.Wait(150);
                        Instance.EreborInstance.PoisType = pois.Name;
                        break;

                }
            }
            if(!e.TextValue.Equals(string.Empty))
            {
                tmps = Instance.EreborInstance.GoldLimit ?? "0";
                //tmps= Regex.Match(tmps, @"\d+").Value;
                Instance.Settings.GoldLimit = ushort.Parse(tmps);

                tmps = Instance.EreborInstance.GwWidth ?? "800";
                //tmps = Regex.Match(tmps, @"\d+").Value;
                Instance.GWS.Width = ushort.Parse(tmps);

                tmps = Instance.EreborInstance.GwHeight ?? "600";
                //tmps = Regex.Match(tmps, @"\d+").Value;
                Instance.GWS.Height = ushort.Parse(tmps);

                tmps = Instance.EreborInstance.HidDelay ?? "2800";
               // tmps = Regex.Match(tmps, @"\d+").Value;
                Instance.Settings.hidDelay = ushort.Parse(tmps);

                tmps = Instance.EreborInstance.Hits2Pot ?? "30";
              //  tmps = Regex.Match(tmps, @"\d+").Value;
                Instance.Settings.criticalHits = ushort.Parse(tmps);

                tmps = Instance.EreborInstance.MinHp ?? "80";
               // tmps = Regex.Match(tmps, @"\d+").Value;
                Instance.Settings.minHP = ushort.Parse(tmps);

                tmps = Instance.EreborInstance.VoodooObet ?? "40";
               // tmps = Regex.Match(tmps, @"\d+").Value;
                Instance.Settings.VoodooManaLimit = ushort.Parse(tmps);
            }


            // Sync Property with controls

            //Instance.Settings.Lot.Food = Instance.EreborInstance.Food;
            //Instance.Settings.Lot.Leather = Instance.EreborInstance.Leather;
            //Instance.Settings.Lot.Bolts = Instance.EreborInstance.Bolts;
            //Instance.Settings.Lot.Extend1 = Instance.EreborInstance.Extend1;
            //Instance.Settings.Lot.Extend2 = Instance.EreborInstance.Extend2;
            //Instance.Settings.Lot.DoLot = Instance.EreborInstance.Lot;
            //Instance.Settings.Lot.Feathers = Instance.EreborInstance.Feathers;
            //Instance.Settings.Lot.Gems = Instance.EreborInstance.Gems;
            //Instance.Settings.Lot.Reageants = Instance.EreborInstance.Regeants;
            //Instance.Settings.Lot.HideCorpses = Instance.EreborInstance.CorpsesHide;
            //Instance.Settings.Amorf.Amorf = Instance.EreborInstance.AutoMorf;
            //Instance.Settings.HitBandage= Instance.EreborInstance.HitBandage;
            //Instance.Settings.HitTrack= Instance.EreborInstance.HitTrack;
            //Instance.Settings.Spells.AutoArrow= Instance.EreborInstance.AutoArrow;
            //Instance.Settings.AutoDrink= Instance.EreborInstance.AutoDrink;
            //Instance.Settings.PrintAnim= Instance.EreborInstance.StoodUps;


            tmpb = Instance.EreborInstance.Food; if (tmpb != Instance.Settings.Lot.Food) Instance.Settings.Lot.Food = tmpb;
            tmpb = Instance.EreborInstance.Leather; if (tmpb != Instance.Settings.Lot.Leather) Instance.Settings.Lot.Leather = tmpb;
            tmpb = Instance.EreborInstance.Bolts; if (tmpb != Instance.Settings.Lot.Bolts) Instance.Settings.Lot.Bolts = tmpb;
            tmpb = Instance.EreborInstance.Extend1; if (tmpb != Instance.Settings.Lot.Extend1) Instance.Settings.Lot.Extend1 = tmpb;
            tmpb = Instance.EreborInstance.Extend2; if (tmpb != Instance.Settings.Lot.Extend2) Instance.Settings.Lot.Extend2 = tmpb;
            tmpb = Instance.EreborInstance.Lot; if (tmpb != Instance.Settings.Lot.DoLot) Instance.Settings.Lot.DoLot = tmpb;
            tmpb = Instance.EreborInstance.Feathers; if (tmpb != Instance.Settings.Lot.Feathers) Instance.Settings.Lot.Feathers = tmpb;
            tmpb = Instance.EreborInstance.Gems; if (tmpb != Instance.Settings.Lot.Gems) Instance.Settings.Lot.Gems = tmpb;
            tmpb = Instance.EreborInstance.Regeants; if (tmpb != Instance.Settings.Lot.Reageants) Instance.Settings.Lot.Reageants = tmpb;
            tmpb = Instance.EreborInstance.CorpsesHide; if (tmpb != Instance.Settings.Lot.HideCorpses) Instance.Settings.Lot.HideCorpses = tmpb;
            tmpb = Instance.EreborInstance.AutoMorf; if (tmpb != Instance.Settings.Amorf.Amorf) Instance.Settings.Amorf.Amorf = tmpb;
            tmpb = Instance.EreborInstance.HitBandage; if (tmpb != Instance.Settings.HitBandage) Instance.Settings.HitBandage = tmpb;
            tmpb = Instance.EreborInstance.HitTrack; if (tmpb != Instance.Settings.HitTrack) Instance.Settings.HitTrack = tmpb;
            tmpb = Instance.EreborInstance.AutoArrow; if (tmpb != Instance.Settings.Spells.AutoArrow) Instance.Settings.Spells.AutoArrow = tmpb;
            tmpb = Instance.EreborInstance.AutoDrink; if (tmpb != Instance.Settings.AutoDrink) Instance.Settings.AutoDrink = tmpb;
            tmpb = Instance.EreborInstance.StoodUps; if (tmpb != Instance.Settings.PrintAnim) Instance.Settings.PrintAnim = tmpb;




        }
        #endregion

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
        public string PrintState()
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

        private void setEQ()
        {
            UO.Wait(1000);
            World.Player.Click();
            UO.Wait(1000);
            World.Player.WaitTarget();
            UO.Say(".setequip15");

        }


        

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);







    }
}
