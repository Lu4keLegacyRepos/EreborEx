using EreborPhoenixExtension.Libs;
using EreborPhoenixExtension.Libs.Abilites;
using EreborPhoenixExtension.Libs.EquipSet;
using EreborPhoenixExtension.Libs.Events;
using EreborPhoenixExtension.Libs.Healing;
using EreborPhoenixExtension.Libs.Magic;
using EreborPhoenixExtension.Libs.Runes;
using EreborPhoenixExtension.Libs.Skills;
using EreborPhoenixExtension.Libs.Skills.Mining;
using EreborPhoenixExtension.Libs.Weapons;
using Phoenix;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace EreborPhoenixExtension
{
    [Serializable]
    public class Settings
    {
        public event EventHandler<EventArgs> OnStoodUp; 
        #region Fields

        private string path = Core.Directory + @"\Profiles\XML\";
        private string[] crystalOnCalls = new string[] { "Nyni jsi schopen rychleji lecit bandazemi.", "Nyni jsi schopen lecit lepe.", "Nyni ti muze byt navracena spotrebovana magenergie.", "Nyni jsi schopen kouzlit za mene many." };
        private string[] bandageDoneCalls = new string[] { " byl uspesne osetren", "Leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.", "Nevidis na cil.", "Nevidis cil." };
        private List<string> onHitCalls = new List<string>() { "Tvuj cil krvaci.", "Skvely zasah!", "Kriticky zasah!", "Vysavas staminu", "Vysavas zivoty!" };
        [XmlIgnore]
        public DateTime HiddenTime;
        [XmlIgnore]
        public int exp;
        private bool printAnim;
        private bool hitBandage;
        private bool hitTrack;
        private bool autoDrink;
        public bool arrowSelfProgress=false, musicProgress=false, crystalState = false, BandageDone = true;
        public string CrystalCmd, HealCmd;

        [XmlIgnore]
        public Character.EreborClass ActualCharacter= Character.EreborClass.Null;
        [XmlIgnore]
        public SwitchabeHotkeys HotKeys;
        [XmlIgnore]
        public Casting Casting;
        [XmlIgnore]
        public WallCounter wallCnt;
        [XmlIgnore]
        public Other OtherAbilites;
        [XmlIgnore]
        public Voodoo VooDoo;
        [XmlIgnore]
        public Spells Spells;
        [XmlIgnore]
        public AutoMorf Amorf;
        [XmlIgnore]
        public Hiding Hiding;
        [XmlIgnore]
        public Peacemaking_Enticement Peace_Entic;
        public Tracking Track;
        [XmlIgnore]
        public Veterinary Vete;
        Targeting targ;
        Provocation Provocation;
        [XmlIgnore]
        public Handler Ev;
        [XmlIgnore]
        public Mine Mining;

        #endregion

        #region Properties
        public Poisoning Poisoning { get; set; }
        public EquipSet EquipSet { get; set; }
        public Lot Lot { get; set; }
        public AutoHeal AHeal { get; set; }
        public RuneTree RuneTree { get; set; }
        public WeaponsHandle Weapons { get; set; }
        public uint minHP { get; set; }
        public uint VoodooManaLimit { get; set; }
        public uint hidDelay { get; set; }
        public bool HitBandage
        {
            get { return hitBandage; }
            set
            {
                if (value)
                {
                    Core.RegisterServerMessageCallback(0x1C, OnHitBandage);
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitBandage);
                }
                hitBandage = value;
            }
        }
        public bool HitTrack
        {
            get { return hitTrack; }
            set
            {
                if (value)
                {
                    Core.RegisterServerMessageCallback(0x1C, OnHitTrack);
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitTrack);
                }
                hitTrack = value;
            }
        }
        public uint criticalHits { get;  set; }
        public bool AutoDrink
        {
            get { return autoDrink; }
            set
            {
                autoDrink = value;
            }
        }
        public ushort GoldLimit { get;  set; }
        public bool PrintAnim
        {
            get
            {
                return printAnim;
            }
            set
            {
                if (value)
                    Core.RegisterServerMessageCallback(0x6E, onNaprah);
                else
                    Core.UnregisterServerMessageCallback(0x6E, onNaprah);
                printAnim = value;
            }
        }

        public bool AutoMorf
        {
            get
            {
                return Amorf.Amorf;
            }

            set
            {
                Amorf.Amorf = value;
            }
        }

        public bool AutoArrow
        {
            get
            {
                return Spells.AutoArrow;
            }

            set
            {
                Spells.AutoArrow = value;
            }
        }

        public List<string> Hotkeys
        {
            get
            {
                return HotKeys.swHotkeys;
                
            }
            set
            {
                HotKeys.swHotkeys = value;
            }
        } 
        public uint PoisBottle
        {
            get { return Poisoning.PoisonBottle; }
            set { Poisoning.PoisonBottle = value; }
        }
        public string PoisName
        {
            get {
                return Poisoning.PoisonName;
            }
            set
            {
                UOItem tmp = new UOItem(Poisoning.PoisonBottle);
                tmp.Click();
                UO.Wait(200);
                Poisoning.PoisonName = value;
            }
        }

        public bool SkipCopper
        {
            get
            {
                return Mining.SkipCopper;
            }

            set
            {
                Mining.SkipCopper = value;
            }
        }

        public bool SkipIron
        {
            get
            {
                return Mining.SkipIron;
            }

            set
            {
                Mining.SkipIron = value;
            }
        }

        public bool SkipSilicon
        {
            get
            {
                return Mining.SkipSilicon;
            }

            set
            {
                Mining.SkipSilicon = value;
            }
        }

        public bool SkipVerite
        {
            get
            {
                return Mining.SkipVerite;
            }

            set
            {
                Mining.SkipVerite = value;
            }
        }

        public bool DropCopper
        {
            get
            {
                return Mining.DropCopper;
            }

            set
            {
                Mining.DropCopper = value;
            }
        }

        public bool DropIron
        {
            get
            {
                return Mining.DropIron;
            }

            set
            {
                Mining.DropIron = value;
            }
        }

        public bool DropSilicon
        {
            get
            {
                return Mining.DropSilicon;
            }

            set
            {
                Mining.DropSilicon = value;
            }
        }

        public bool DropVerite
        {
            get
            {
                return Mining.DropVerite;
            }

            set
            {
                Mining.DropVerite = value;
            }
        }

        public bool Crystal
        {
            get
            {
                return Mining.CrystalEnabled;
            }

            set
            {
                Mining.CrystalEnabled = value;
            }
        }

        public bool AutoStockUp
        {
            get
            {
                return Mining.AutoStockUp;
            }

            set
            {
                Mining.AutoStockUp = value;
            }
        }

        public bool AutoRemoveObstacles
        {
            get
            {
                return Mining.RemoveObstacles;
            }

            set
            {
                Mining.RemoveObstacles = value;
            }
        }

        public bool UseBank
        {
            get
            {
                return Mining.UseBank;
            }

            set
            {
                Mining.UseBank = value;
            }
        }

        public int MaxObs
        {
            get
            {
                return Mining.MaxObsidian;
            }

            set
            {
                Mining.MaxObsidian = value;
            }
        }

        public int MaxAda
        {
            get
            {
                return Mining.MaxAdamantium;
            }

            set
            {
                Mining.MaxAdamantium = value;
            }
        }

        public bool RessurectInProgress { get; set; }





        #endregion


        public Settings()
        {
            Debug.WriteLine("SETTINGS !!");


            Provocation = new Provocation();
            HotKeys = new SwitchabeHotkeys();
            Casting = new Casting();
            wallCnt = new WallCounter();
            VooDoo = new Voodoo();
            Spells = new Spells(this);
            Amorf = new AutoMorf();
            Weapons = new WeaponsHandle();
            Hiding = new Hiding(this);
            OtherAbilites = new Other(Hiding.hidoff, Weapons);
            Poisoning = new Poisoning();
            Peace_Entic = new Peacemaking_Enticement(this);
            Lot = new Lot();
            Track = new Tracking();
            Vete = new Veterinary(Weapons);
            targ = new Targeting();
            EquipSet = new EquipSet();
            RuneTree = new RuneTree();
            AHeal = new AutoHeal();
            Ev = new Handler();
            Mining = new Mine();

            AHeal.PatientHurted += AHeal_PatientHurted;
        }

        private void AHeal_PatientHurted(object sender, HurtedPatientArgs e)
        {
            AHeal.PatientHurted -= AHeal_PatientHurted;
            if (e.pati != null)
            {
                AHeal.bandage(e.pati);
            }
            if (e.selfHurted)
            {
                AHeal.bandage();
            }
            if (e.crystalOff)
            {
                CrystallCnt++;
                if (CrystallCnt >= 5 && crystalState)
                {

                   CrystallCnt = 0;
                    UO.Say(CrystalCmd);
                    AHeal.GetStatuses();
                    Weapons.ActualWeapon.Equip();
                }
            }
            AHeal.PatientHurted += AHeal_PatientHurted;
        }



        #region CallbackResults
        [ServerMessageHandler(0x1C)]
        public CallbackResult onCrystal(byte[] data, CallbackResult prevResult)//0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            if (packet.Text.Contains("Jsi zpatky v normalnim stavu."))
            {
                Main.Instance.Settings.crystalState = false;

                return CallbackResult.Normal;
            }
            foreach (string s in crystalOnCalls)
            {
                if (packet.Text.Contains(s))
                {
                    Main.Instance.Settings.crystalState = true;
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }


        [ServerMessageHandler(0x1C)]
        public CallbackResult onExp(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains(" zkusenosti."))
            {

                //string[] numbers = Regex.Split(packet.Text, @"\D+");
                string number = Regex.Match(packet.Text, @"-?\d+").Value;
                if (!string.IsNullOrEmpty(number))
                {
                    Main.Instance.Settings.exp += int.Parse(number);

                }
            }
            return CallbackResult.Normal;
        }
        [ServerMessageHandler(0x11)]
        public CallbackResult OnNextTarget(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            if (reader.ReadByte() != 0x11) throw new Exception("Invalid packet passed to OnNextTarget method.");
            ushort blockSize = reader.ReadUInt16();
            uint serial = reader.ReadUInt32();
            if (serial == Aliases.Self)//|| inList.Contains(serial))
            {
                return CallbackResult.Normal;
            }
            Aliases.SetObject("laststatus", serial);
            UOCharacter cil = World.GetCharacter(serial);
            if (cil.MaxHits == -1)
            {
                cil.RequestStatus(50);
                return CallbackResult.Normal;
            }
            else
            {
                ushort color = 0;
                string not = cil.Notoriety.ToString();
                switch (not)
                {

                    case "Criminal":
                        color = 0x0026;
                        break;

                    case "Enemy":
                        color = 0x0031;
                        break;

                    case "Guild":
                        color = 0x0B50;
                        break;

                    case "Innocent":
                        color = 0x0058;
                        break;

                    case "Murderer":
                        color = 0x0026;
                        break;

                    case "Neutral":
                        color = 0x03BC;
                        break;
                    case "Unknown":
                        color = 0x03BC;
                        break;
                    default:
                        color = Phoenix.Env.DefaultInfoColor;
                        break;
                }
                UO.Print(color, "{0} : {1}/{2} ({3})", cil.Name, cil.Hits, cil.MaxHits, cil.Distance);
                return CallbackResult.Normal;
            }
        }


       public CallbackResult OnHitBandage(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in onHitCalls)
            {
                if (packet.Text.Contains(s) && World.Player.Hits < World.Player.MaxHits - 10 && !World.Player.Hidden)
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitBandage);
                    bandage asBanage = new bandage(AHeal.bandage);
                    asBanage.BeginInvoke(null, null);
                    Main.Instance.Settings.Weapons.ActualWeapon.Equip();
                    //Equip eq = new Equip(Weapons.ActualWeapon.Equip);
                    //eq.BeginInvoke(null, null);
                    UO.Wait(100);
                    Core.RegisterServerMessageCallback(0x1C, OnHitBandage);
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }

        public CallbackResult OnHitTrack(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in onHitCalls)
            {

                if (packet.Text.Contains(s))
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitTrack);
                    UO.Say(",track 2 true");
                    Core.RegisterServerMessageCallback(0x1C, OnHitTrack);
                    return CallbackResult.Normal;
                }

            }
            return CallbackResult.Normal;
        }


        public CallbackResult onNaprah(byte[] data, CallbackResult prev)
        {

            PacketReader p = new PacketReader(data);
            p.Skip(1);
            uint serial = p.ReadUInt32();
            ushort action = p.ReadUInt16();
            if (action == 26 && serial == World.Player.Serial && new UOCharacter(Aliases.GetObject("laststatus")).Distance < 3)
            {
                UO.Print(SpeechFont.Bold, 0x0076, "Naprah na " + new UOCharacter(Aliases.GetObject("laststatus")).Name);
                OnStoodUp?.Invoke(this, new EventArgs());
            }

            return CallbackResult.Normal;
        }
        [ServerMessageHandler(0xa1)]
        public CallbackResult onHpChanged(byte[] data, CallbackResult prevResult)//0xa1
        {
            UOCharacter character = new UOCharacter(Phoenix.ByteConverter.BigEndian.ToUInt32(data, 1));
            if (character.Serial == World.Player.Serial) return CallbackResult.Normal;
            ushort maxHits = 100; // Nejvyssi HITS bez nakouzleni
            ushort hits = Phoenix.ByteConverter.BigEndian.ToUInt16(data, 7);
            ushort[] color = new ushort[4];
            color[0] = 0x0026;//red
            color[2] = 0x0175;//green
            color[1] = 0x099;//yellow
            color[3] = 0x0FAB;//fialova - enemy;
            int col = 0;

            if (character.Hits - hits < -4 || character.Hits - hits > 4)
            {
                if (character.Hits > hits)
                {
                    if (character.Poisoned) col = 2;
                    else col = 0;
                }
                else
                {
                    if (character.Poisoned) col = 2;
                    else col = 1;
                }

                if ((character.Model == 0x0190 || character.Model == 0x0191))
                {
                    character.Print(color[col], "{2} [{0} HP] {1}", ((maxHits / 100) * hits), (hits - character.Hits), character.Name);
                }


                if (character.Serial == Aliases.GetObject("laststatus"))
                    character.Print(color[3], "[{0} HP] {1}", ((maxHits / 100) * hits), (hits - character.Hits));

            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x1C)]
        public CallbackResult onBandageDone(byte[] data, CallbackResult prevResult)//0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in bandageDoneCalls)
            {
                if (packet.Text.Contains(s))
                {
                    //UO.Wait(100);
                    Main.Instance.Settings.BandageDone = true;
                }
            }
            return CallbackResult.Normal;
        }


        public CallbackResult onSpellFizz(byte[] data, CallbackResult prev)
        {
            AsciiSpeech asc = new AsciiSpeech(data);
            if (asc.Text.ToLower().Contains("kouzlo se nezdarilo.")) Casting.SpellFizz = true;
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x4F)]
        public CallbackResult OnSunLight(byte[] data, CallbackResult prevResult)
        {
            if (prevResult < CallbackResult.Sent)
            {
                if (data[1] > 20)//max 31-tma
                {
                    byte[] newData = new byte[2];
                    newData[0] = 0x4F;
                    newData[1] = (byte)19;
                    Core.SendToClient(newData);

                    // UO.Print(0x015C, "Light level fixed.");
                    return CallbackResult.Sent;
                }
            }
            return CallbackResult.Normal;
        }
        private int x = 1;
        internal int CrystallCnt=0;

        [ServerMessageHandler(0x22)]
        public CallbackResult OnWalkRequestSucceeded(byte[] data, CallbackResult prevResult)
        {
            if (World.Player.Hidden)
            {
                if (prevResult < CallbackResult.Sent)
                {
                    if (x % 5 == 0) UO.Print(0x011C, "Stealth : {0}", x);
                    x++;
                }
            }
            else
            {
                x = 1;
            }
            return CallbackResult.Normal;
        }
        [ServerMessageHandler(0x65)]//weather
        public CallbackResult Filter(byte[] data, CallbackResult prevResult) { return CallbackResult.Eat; }

        #endregion
    }
}
