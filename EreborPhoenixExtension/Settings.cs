using EreborPhoenixExtension;
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
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EreborPhoenixExtension
{
    [Serializable]
    public class Settings
    {
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
        public int GWHeight { get; set; }
        public int GWWidth { get; set; }
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


        public void Set(string filename)
        {
            Settings tmp = null;
            tmp.Deserialize(filename);
            if (tmp == null) return;
            Poisoning.PoisonBottle = tmp.Poisoning.PoisonBottle;
            EquipSet.equipy = tmp.EquipSet.equipy;
            Lot.Bolts = tmp.Lot.Bolts;
            Lot.CarvTool = tmp.Lot.CarvTool;
            Lot.DoLot = tmp.Lot.DoLot;
            Lot.Feathers = tmp.Lot.Feathers;
            Lot.Food = tmp.Lot.Food;
            Lot.Gems = tmp.Lot.Gems;
            Lot.HideCorpses = tmp.Lot.HideCorpses;
            Lot.Leather = tmp.Lot.Leather;
            Lot.LotBag = tmp.Lot.LotBag;
            Lot.Reageants = tmp.Lot.Reageants;
            Lot.Extend1 = tmp.Lot.Extend1;
            Lot.extend1_type = tmp.Lot.extend1_type;
            Lot.Extend2 = tmp.Lot.Extend2;
            Lot.extend2_type = tmp.Lot.extend2_type;
            AHeal.avaibleEquips = tmp.AHeal.avaibleEquips;
            AHeal.HealedPlayers = tmp.AHeal.HealedPlayers;
            AHeal.PatientHPLimit = tmp.AHeal.PatientHPLimit;
            RuneTree.Runes = tmp.RuneTree.Runes;
            Weapons.weapons = tmp.Weapons.weapons;
            Weapons.ActualWeapon = tmp.Weapons.ActualWeapon;
            minHP = tmp.minHP;
            VoodooManaLimit = tmp.VoodooManaLimit;
            hidDelay = tmp.hidDelay;
            HitBandage = tmp.HitBandage;
            HitTrack = tmp.HitTrack;
            criticalHits = tmp.criticalHits;
            AutoDrink = tmp.AutoDrink;
            GoldLimit = tmp.GoldLimit;
            PrintAnim = tmp.PrintAnim;

        }

        public Settings()
        {
            Debug.WriteLine("SETTINGS !!");

            Mining = new Mine();
            XmlSerializeHelper<Mine> h = new XmlSerializeHelper<Mine>();
            Mining = h.Deserialize("Mining");
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

            AHeal.PatientHurted += AHeal_PatientHurted;


        }

        private void AHeal_PatientHurted(object sender, Libs.Healing.HurtedPatientArgs e)
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



        #region Serializable & Deserializable & Set Data
        public void Serialize(string filename)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var serializer = new XmlSerializer(this.GetType());
                if(File.Exists(path + filename))File.Delete(path + filename);
                using (var stream = File.OpenWrite(path + filename))
                {
                    serializer.Serialize(stream, this);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.InnerException.ToString()); }

        }

        public Settings Deserialize(string filename)
        {
            Settings XMLOBJ = null;
            try
            {
                var serializer = new XmlSerializer(GetType());
                using (var stream = File.OpenRead(path + filename))
                {
                    XMLOBJ = (Settings)serializer.Deserialize(stream);
                }

            }
            catch //(Exception ex)
            {

                UO.PrintError(filename + " neexistuje");
               // MessageBox.Show(ex.InnerException.ToString());
                return new Settings();
            }
            return XMLOBJ;

        }

        public void SetWindow(out int Width, out int Height)
        {
            Width = GWWidth<400?800:GWWidth;
            Height = GWHeight<300?600:GWHeight;
        }
        #endregion

        #region CallbackResults
        public CallbackResult onCrystal(byte[] data, CallbackResult prevResult)//0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            if (packet.Text.Contains("Jsi zpatky v normalnim stavu."))
            {
                crystalState = false;

                return CallbackResult.Normal;
            }
            foreach (string s in crystalOnCalls)
            {
                if (packet.Text.Contains(s))
                {
                    crystalState = true;
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }


      public CallbackResult onExp(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains(" zkusenosti."))
            {

                //string[] numbers = Regex.Split(packet.Text, @"\D+");
                string number = Regex.Match(packet.Text, @"-?\d+").Value;
                if (!string.IsNullOrEmpty(number))
                {
                    exp += Int32.Parse(number);

                }
            }
            return CallbackResult.Normal;
        }

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
                    Equip eq = new Equip(Weapons.ActualWeapon.Equip);
                    eq.BeginInvoke(null, null);
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
                UO.Print(SpeechFont.Bold, 0x0076, "Naprah na " + new UOCharacter(Aliases.GetObject("laststatus")).Name);
            //UO.PrintWarning("Naprah na " + new UOCharacter(Aliases.GetObject("laststatus")).Name);
            return CallbackResult.Normal;
        }

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


        public CallbackResult onBandageDone(byte[] data, CallbackResult prevResult)//0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in bandageDoneCalls)
            {
                if (packet.Text.Contains(s))
                {
                    //UO.Wait(100);
                    BandageDone = true;
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
                if (data[1] > 22)//max 31-tma
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

        [ClientMessageHandler(0x01)]//logout
        public CallbackResult LogOut(byte[] data, CallbackResult prevResult)
        {
            Main.Instance.save();
            return CallbackResult.Normal;
        }
        #endregion
    }
}
