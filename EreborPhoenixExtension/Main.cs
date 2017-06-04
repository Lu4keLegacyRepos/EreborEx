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

        readonly GameWindowSize GWS;
        public GameWIndoSize_DATA GWS_DATA;
        public Settings Settings;

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);



        private static Main instance;

        private Main() {
            XmlSerializeHelper<GameWIndoSize_DATA> gw = new XmlSerializeHelper<GameWIndoSize_DATA>();
            gw.Load("WindowSize", out GWS_DATA);

            GWS = new GameWindowSize(GWS_DATA); //TODO save proc null ?
            

            t = new System.Timers.Timer(500);
            t.Elapsed += T_Elapsed;
            t.Start();
            ShowWindowAsync(Client.HWND, SW_SHOWNORMAL);
            ShowWindowAsync(Client.HWND, SW_SHOWMAXIMIZED);
            Erebor.SetForegroundWindow(Client.HWND);


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
            World.Player.RequestStatus(100);
            XmlSerializeHelper<Settings> sett = new XmlSerializeHelper<Settings>();
            new Thread(setEQ).Start();

            if (!sett.Load(World.Player.Name, out Instance.Settings))
                Instance.Settings = new Settings();

            XmlSerializeHelper<Mine> ss = new XmlSerializeHelper<Mine>();
            if (!ss.Load("Mining", out Instance.Settings.Mining))
                Instance.Settings.Mining = new Mine();

            World.Player.Changed += Player_Changed;
            Instance.Settings.Ev.hiddenChange += Ev_hiddenChange;
            Instance.Settings.Ev.hitsChanged += Ev_hitsChanged;
            Instance.Settings.AHeal.PatientHurted += AHeal_PatientHurted;





            #region Init GUI
            Instance.EreborInstance.Invoke(new MethodInvoker(delegate
            {
                Instance.EreborInstance.Food = Instance.Settings.Lot.Food;
                Instance.EreborInstance.Leather = Instance.Settings.Lot.Leather;
                Instance.EreborInstance.Bolts = Instance.Settings.Lot.Bolts;
                Instance.EreborInstance.Extend1Type_Text = Instance.Settings.Lot.Extend1_type.ToString();
                Instance.EreborInstance.Extend2Type_Text = Instance.Settings.Lot.Extend2_type.ToString();
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
                Instance.EreborInstance.PoisType = Instance.Settings.PoisName;


                Instance.EreborInstance.GoldLimit = Instance.Settings.GoldLimit.ToString();
                Instance.EreborInstance.GwWidth = Instance.GWS_DATA.Width.ToString();
                Instance.EreborInstance.GwHeight = Instance.GWS_DATA.Height.ToString();
                Instance.EreborInstance.HidDelay = Instance.Settings.hidDelay.ToString();
                Instance.EreborInstance.Hits2Pot = Instance.Settings.criticalHits.ToString();
                Instance.EreborInstance.MinHp = Instance.Settings.minHP.ToString();
                Instance.EreborInstance.VoodooObet = Instance.Settings.VoodooManaLimit.ToString();



                Instance.EreborInstance.SkipCopper = Instance.Settings.SkipCopper;
                Instance.EreborInstance.SkipIron = Instance.Settings.SkipIron;
                Instance.EreborInstance.SkipSilicon = Instance.Settings.SkipSilicon;
                Instance.EreborInstance.SkipVerite = Instance.Settings.SkipVerite;
                Instance.EreborInstance.DropCopper = Instance.Settings.DropCopper;
                Instance.EreborInstance.DropIron = Instance.Settings.DropIron;
                Instance.EreborInstance.DropSilicon = Instance.Settings.DropSilicon;
                Instance.EreborInstance.DropVerite = Instance.Settings.DropVerite;

                Instance.EreborInstance.Crystal = Instance.Settings.Crystal;
                Instance.EreborInstance.AutoStockUp = Instance.Settings.AutoStockUp;
                Instance.EreborInstance.AutoRemoveObstacles = Instance.Settings.AutoRemoveObstacles;
                Instance.EreborInstance.UseBank = Instance.Settings.UseBank;

                Instance.EreborInstance.MaxObs = Instance.Settings.MaxObs;
                Instance.EreborInstance.MaxAda = Instance.Settings.MaxAda;


            }));
            RefreshLists();

            UO.Wait(100);
            Instance.EreborInstance.Changed += EreborInstance_Changed;
            UO.PrintInformation("Loading done");


            #endregion

        }
        #region GUI Function
        private void EreborInstance_Changed(object sender, EventChangedArgs e)
        {
            int tmp;
            string tmps;
            if (sender is Button)
            {
                switch (e.btnName)
                {
                    case "btn_0":
                        SetForegroundWindow(Client.HWND);
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
                        SetForegroundWindow(Client.HWND);
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
                                SetForegroundWindow(Client.HWND);
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
                                    Instance.Settings.EquipSet.Remove(tmp);
                                    Instance.Settings.EquipSet.fillListBox(Instance.EreborInstance.EquipList);
                                }
                                break;
                            // Weapons
                            case 2:
                                tmp = Instance.EreborInstance.SelectedWeapon;
                                if (tmp >= 0)
                                {
                                    Instance.Settings.Weapons.Remove(tmp);
                                    Instance.Settings.Weapons.fillListBox(Instance.EreborInstance.WeaponList);
                                }
                                break;
                            // Healed
                            case 3:
                                tmp = Instance.EreborInstance.SelectedHealed;
                                if (tmp >= 0)
                                {
                                    Instance.Settings.AHeal.Remove(tmp);
                                    Instance.Settings.AHeal.fillListBox(Instance.EreborInstance.HealList);
                                }
                                break;
                            // TrackIgnore
                            case 4:
                                tmp = Instance.EreborInstance.SelectedIgnored;
                                if (tmp >= 0)
                                {
                                    Instance.Settings.Track.Remove(tmp);
                                    Instance.Settings.Track.fillListBox(Instance.EreborInstance.TrackIgnoreList);
                                }
                                break;
                            case 5:
                                break;

                        }
                        break;
                    case "btn_3":
                        SetForegroundWindow(Client.HWND);
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
                                Instance.Settings.EquipSet.Add();
                                Instance.Settings.EquipSet.fillListBox(Instance.EreborInstance.EquipList);
                                break;
                            // Weapons
                            case 2:
                                UO.PrintWarning("Zamer zbran a stit");
                                Instance.Settings.Weapons.Add();
                                Instance.Settings.Weapons.fillListBox(Instance.EreborInstance.WeaponList);
                                break;
                            // Healed
                            case 3:
                                UO.PrintWarning("Zamer koho lecit");
                                Instance.Settings.AHeal.Add();
                                Instance.Settings.AHeal.fillListBox(Instance.EreborInstance.HealList);
                                break;
                            // TrackIgnore
                            case 4:
                                UO.PrintWarning("Zamer koho ignorovat pri Trackingu");
                                Instance.Settings.Track.Add();
                                Instance.Settings.Track.fillListBox(Instance.EreborInstance.TrackIgnoreList);
                                break;
                            // Mining
                            case 5:
                                UO.PrintInformation("Zvolen dul {0}", Instance.Settings.Mining.Maps[Instance.EreborInstance.SelectedMine].Name);
                                Instance.Settings.Mining.ActualMapIndex = Instance.EreborInstance.SelectedMine;
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
                                Instance.Settings.EquipSet.fillListBox(Instance.EreborInstance.EquipList);
                                break;
                            // Weapons
                            case 2:
                                Instance.Settings.Weapons.fillListBox(Instance.EreborInstance.WeaponList);
                                break;
                            // Healed
                            case 3:
                                Instance.Settings.AHeal.fillListBox(Instance.EreborInstance.HealList);
                                break;
                            // TrackIgnore
                            case 4:
                                Instance.Settings.Track.fillListBox(Instance.EreborInstance.TrackIgnoreList);

                                break;
                            // Mining
                            case 5:
                                Instance.Settings.Mining.fillListBox(Instance.EreborInstance.MiningList);
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
                        SetForegroundWindow(Client.HWND);
                        UO.PrintWarning("Zamer Item");
                        Instance.Settings.Lot.Extend1_type = new UOItem(UIManager.TargetObject()).Graphic;
                        Instance.EreborInstance.Invoke(new MethodInvoker(delegate
                        {
                            Instance.EreborInstance.Extend1Type_Text = Instance.Settings.Lot.Extend1_type.ToString();
                        }));
                        break;
                    case "btn_Extend2":
                        SetForegroundWindow(Client.HWND);
                        UO.PrintWarning("Zamer Item");
                        Instance.Settings.Lot.Extend2_type = new UOItem(UIManager.TargetObject()).Graphic;
                        Instance.EreborInstance.Invoke(new MethodInvoker(delegate
                        {
                            Instance.EreborInstance.Extend2Type_Text = Instance.Settings.Lot.Extend2_type.ToString();
                        }));
                        break;
                    case "btn_SetBag":
                        SetForegroundWindow(Client.HWND);
                        UO.PrintWarning("Zamer Lot Baglik");
                        Instance.Settings.Lot.LotBag = new UOItem(UIManager.TargetObject());
                        break;
                    case "btn_SetCarv":
                        SetForegroundWindow(Client.HWND);
                        UO.PrintWarning("Zamer Nuz");
                        Instance.Settings.Lot.CarvTool = new UOItem(UIManager.TargetObject());
                        break;
                    case "btn_Pois":
                        SetForegroundWindow(Client.HWND);
                        UO.PrintInformation("Zamer Poison");
                        Instance.Settings.Poisoning.PoisonBottle = new UOItem(UIManager.TargetObject());
                        UOItem pois = new UOItem(Instance.Settings.Poisoning.PoisonBottle);
                        pois.Click();
                        UO.Wait(150);
                        Instance.EreborInstance.Invoke(new MethodInvoker(delegate
                        {
                            Instance.EreborInstance.PoisType = pois.Name;
                        }));
                        break;

                    case "btn_setRunesPos":
                        UO.PrintError("Aktualni poloha ulozena pro Portovani do dolu");
                        Instance.Settings.Mining.RunePosition = new System.Drawing.Point(World.Player.X, World.Player.Y);
                        break;

                    case "btn_setBankPos":
                        UO.PrintError("Aktualni poloha ulozena pro Vykladaní a spotrebu steaku ze zeme");
                        Instance.Settings.Mining.HousePosition = new System.Drawing.Point(World.Player.X, World.Player.Y);
                        break;

                    case "btn_setDoor":
                        UO.PrintError("Zamer leve zavrene dvere");
                        UOItem tmpd = new UOItem(UIManager.TargetObject());
                        Instance.Settings.Mining.DoorLeft = tmpd ?? new UOItem(0x0);
                        Instance.Settings.Mining.DoorLeftClosedGraphic = tmpd.Graphic;
                        UO.PrintInformation("Zamer prave zavrene dvere");
                        tmpd = new UOItem(UIManager.TargetObject());
                        Instance.Settings.Mining.DoorRight = tmpd ?? new UOItem(0x0);
                        Instance.Settings.Mining.DoorRightClosedGraphic = tmpd.Graphic;
                        break;

                    case "btn_setOreBox":
                        UO.PrintInformation("Zamer Kontejner na Ore");
                        Instance.Settings.Mining.DoorRight =new UOItem(UIManager.TargetObject());
                        break;

                    case "btn_setGemBox":
                        UO.PrintInformation("Zamer Kontejner na Kameny");
                        Instance.Settings.Mining.DoorRight = new UOItem(UIManager.TargetObject());
                        break;

                }
            }
            if (sender is TextBox)
            {

                switch (((TextBox)sender).Name)
                {
                    case "tb_GoldLimit":
                        tmps = Instance.EreborInstance.GoldLimit ?? "0";
                        tmps= Regex.Match(tmps, @"\d+").Value;
                        Instance.Settings.GoldLimit = ushort.Parse(tmps);
                        break;
                    case "tb_GwWidth":
                        tmps = Instance.EreborInstance.GwWidth ?? "800";
                        tmps = Regex.Match(tmps, @"\d+").Value;
                        Instance.GWS_DATA.Width = ushort.Parse(tmps);
                        break;
                    case "tb_GwHeight":

                        tmps = Instance.EreborInstance.GwHeight ?? "600";
                        tmps = Regex.Match(tmps, @"\d+").Value;
                        Instance.GWS_DATA.Height = ushort.Parse(tmps);
                        break;
                    case "tb_HidDelay":
                        tmps = Instance.EreborInstance.HidDelay ?? "2800";
                         tmps = Regex.Match(tmps, @"\d+").Value;
                        Instance.Settings.hidDelay = ushort.Parse(tmps);
                        break;
                    case "tb_Hits2Pot":
                        tmps = Instance.EreborInstance.Hits2Pot ?? "30";
                          tmps = Regex.Match(tmps, @"\d+").Value;
                        Instance.Settings.criticalHits = ushort.Parse(tmps);
                        break;
                    case "tb_MinHpBandage":
                        tmps = Instance.EreborInstance.MinHp ?? "80";
                         tmps = Regex.Match(tmps, @"\d+").Value;
                        Instance.Settings.minHP = ushort.Parse(tmps);
                        break;
                    case "tb_Obet":
                        tmps = Instance.EreborInstance.VoodooObet ?? "40";
                         tmps = Regex.Match(tmps, @"\d+").Value;
                        Instance.Settings.VoodooManaLimit = ushort.Parse(tmps);
                        break;
                }
        }


            // Sync Property with controls

            Instance.EreborInstance.Invoke(new MethodInvoker(delegate
            {
                if (Instance.EreborInstance.Food != Instance.Settings.Lot.Food) Instance.Settings.Lot.Food = Instance.EreborInstance.Food;
                if (Instance.EreborInstance.Leather != Instance.Settings.Lot.Leather) Instance.Settings.Lot.Leather = Instance.EreborInstance.Leather;
                if (Instance.EreborInstance.Bolts != Instance.Settings.Lot.Bolts) Instance.Settings.Lot.Bolts = Instance.EreborInstance.Bolts;
                if (Instance.EreborInstance.Extend1 != Instance.Settings.Lot.Extend1) Instance.Settings.Lot.Extend1 = Instance.EreborInstance.Extend1;
                if (Instance.EreborInstance.Extend2 != Instance.Settings.Lot.Extend2) Instance.Settings.Lot.Extend2 = Instance.EreborInstance.Extend2;
                if (Instance.EreborInstance.Lot != Instance.Settings.Lot.DoLot) Instance.Settings.Lot.DoLot = Instance.EreborInstance.Lot;
                if (Instance.EreborInstance.Feathers != Instance.Settings.Lot.Feathers) Instance.Settings.Lot.Feathers = Instance.EreborInstance.Feathers;
                if (Instance.EreborInstance.Gems != Instance.Settings.Lot.Gems) Instance.Settings.Lot.Gems = Instance.EreborInstance.Gems;
                if (Instance.EreborInstance.Regeants != Instance.Settings.Lot.Reageants) Instance.Settings.Lot.Reageants = Instance.EreborInstance.Regeants;
                if (Instance.EreborInstance.CorpsesHide != Instance.Settings.Lot.HideCorpses) Instance.Settings.Lot.HideCorpses = Instance.EreborInstance.CorpsesHide;
                if (Instance.EreborInstance.AutoMorf != Instance.Settings.Amorf.Amorf) Instance.Settings.Amorf.Amorf = Instance.EreborInstance.AutoMorf;
                if (Instance.EreborInstance.HitBandage != Instance.Settings.HitBandage) Instance.Settings.HitBandage = Instance.EreborInstance.HitBandage;
                if (Instance.EreborInstance.HitTrack != Instance.Settings.HitTrack) Instance.Settings.HitTrack = Instance.EreborInstance.HitTrack;
                if (Instance.EreborInstance.AutoArrow != Instance.Settings.Spells.AutoArrow) Instance.Settings.Spells.AutoArrow = Instance.EreborInstance.AutoArrow;
                if (Instance.EreborInstance.AutoDrink != Instance.Settings.AutoDrink) Instance.Settings.AutoDrink = Instance.EreborInstance.AutoDrink;
                if (Instance.EreborInstance.StoodUps != Instance.Settings.PrintAnim) Instance.Settings.PrintAnim = Instance.EreborInstance.StoodUps;

                if (Instance.EreborInstance.Extend1Type_Text != Instance.Settings.Lot.Extend1_type.ToString())
                    Instance.Settings.Lot.Extend1_type = ushort.Parse(Instance.EreborInstance.Extend1Type_Text);
                if (Instance.EreborInstance.Extend2Type_Text != Instance.Settings.Lot.Extend2_type.ToString())
                    Instance.Settings.Lot.Extend2_type = ushort.Parse(Instance.EreborInstance.Extend2Type_Text);

                if (Instance.EreborInstance.GoldLimit != Instance.Settings.GoldLimit.ToString())
                    Instance.Settings.GoldLimit = ushort.Parse(Instance.EreborInstance.GoldLimit);
                if (Instance.EreborInstance.GwWidth != Instance.GWS_DATA.Width.ToString())
                    Instance.GWS_DATA.Width = ushort.Parse(Instance.EreborInstance.GwWidth);
                if (Instance.EreborInstance.GwHeight != Instance.GWS_DATA.Height.ToString())
                    Instance.GWS_DATA.Height = ushort.Parse(Instance.EreborInstance.GwHeight);
                if (Instance.EreborInstance.HidDelay != Instance.Settings.hidDelay.ToString())
                    Instance.Settings.hidDelay = ushort.Parse(Instance.EreborInstance.HidDelay);
                if (Instance.EreborInstance.Hits2Pot != Instance.Settings.criticalHits.ToString())
                    Instance.Settings.criticalHits = ushort.Parse(Instance.EreborInstance.Hits2Pot);
                if (Instance.EreborInstance.MinHp != Instance.Settings.minHP.ToString())
                    Instance.Settings.minHP = ushort.Parse(Instance.EreborInstance.MinHp);
                if (Instance.EreborInstance.VoodooObet != Instance.Settings.VoodooManaLimit.ToString())
                    Instance.Settings.VoodooManaLimit = ushort.Parse(Instance.EreborInstance.VoodooObet);

                if (Instance.EreborInstance.SkipCopper != Instance.Settings.SkipCopper)
                    Instance.Settings.SkipCopper = Instance.EreborInstance.SkipCopper;
                if (Instance.EreborInstance.SkipIron != Instance.Settings.SkipIron)
                    Instance.Settings.SkipIron = Instance.EreborInstance.SkipIron;
                if (Instance.EreborInstance.SkipSilicon != Instance.Settings.SkipSilicon)
                    Instance.Settings.SkipSilicon = Instance.EreborInstance.SkipSilicon;
                if (Instance.EreborInstance.SkipVerite != Instance.Settings.SkipVerite)
                    Instance.Settings.SkipVerite = Instance.EreborInstance.SkipVerite;
                if (Instance.EreborInstance.DropCopper != Instance.Settings.DropCopper)
                    Instance.Settings.DropCopper = Instance.EreborInstance.DropCopper;
                if (Instance.EreborInstance.DropIron != Instance.Settings.DropIron)
                    Instance.Settings.DropIron = Instance.EreborInstance.DropIron;
                if (Instance.EreborInstance.DropSilicon != Instance.Settings.DropSilicon)
                    Instance.Settings.DropSilicon = Instance.EreborInstance.DropSilicon;
                if (Instance.EreborInstance.DropVerite != Instance.Settings.DropVerite)
                    Instance.Settings.DropVerite = Instance.EreborInstance.DropVerite;
                if (Instance.EreborInstance.Crystal != Instance.Settings.Crystal)
                    Instance.Settings.Crystal = Instance.EreborInstance.Crystal;
                if (Instance.EreborInstance.AutoStockUp != Instance.Settings.AutoStockUp)
                    Instance.Settings.AutoStockUp = Instance.EreborInstance.AutoStockUp;
                if (Instance.EreborInstance.AutoRemoveObstacles != Instance.Settings.AutoRemoveObstacles)
                    Instance.Settings.AutoRemoveObstacles = Instance.EreborInstance.AutoRemoveObstacles;
                if (Instance.EreborInstance.UseBank != Instance.Settings.UseBank)
                    Instance.Settings.UseBank = Instance.EreborInstance.UseBank;
                if (Instance.EreborInstance.MaxObs != Instance.Settings.MaxObs)
                    Instance.Settings.MaxObs = Instance.EreborInstance.MaxObs;
                if (Instance.EreborInstance.MaxAda != Instance.Settings.MaxAda)
                    Instance.Settings.MaxAda = Instance.EreborInstance.MaxAda;


            }));

            RefreshLists();

            XmlSerializeHelper<Settings> sett = new XmlSerializeHelper<Settings>();
            sett.Save(World.Player.Name, Instance.Settings);

            XmlSerializeHelper<Mine> min = new XmlSerializeHelper<Mine>();
            min.Save("Mining", Instance.Settings.Mining);

            XmlSerializeHelper<GameWIndoSize_DATA> gws = new XmlSerializeHelper<GameWIndoSize_DATA>();
            gws.Save("WindowSize", Instance.GWS_DATA);
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



        private void RefreshLists()
        {
            Instance.EreborInstance.Invoke(new MethodInvoker(delegate
            {
                Instance.Settings.RuneTree.FillTreeView(Instance.EreborInstance.RuneView);

                Instance.Settings.EquipSet.fillListBox(Instance.EreborInstance.EquipList);

                Instance.Settings.Weapons.fillListBox(Instance.EreborInstance.WeaponList);

                Instance.Settings.AHeal.fillListBox(Instance.EreborInstance.HealList);

                Instance.Settings.Track.fillListBox(Instance.EreborInstance.TrackIgnoreList);

                Instance.Settings.Mining.fillListBox(Instance.EreborInstance.MiningList);
            }));
        }
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);







    }
}
