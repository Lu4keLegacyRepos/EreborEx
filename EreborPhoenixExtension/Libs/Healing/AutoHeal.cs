using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml.Serialization;

namespace EreborPhoenixExtension.Libs.Healing
{
    public delegate void bandage();
    [Serializable]
    public class AutoHeal
    {
        private System.Timers.Timer checker;
        private Thread t;
        [XmlIgnore]
        public DateTime startBandage;
        public event EventHandler<HurtedPatientArgs> PatientHurted;
        private bool onOff;
        [XmlIgnore]
        public bool OnOff
        {
            get { return onOff; }
            set
            {
                if (value)
                {
                    Main.Instance.Settings.BandageDone = true;
                    //checker.Start();
                    t = new Thread(ThreadChecker);
                    t.Start();

                    GetStatuses();
                    UO.PrintInformation("Heal ON");
                }
                else
                {
                    // checker.Stop();
                    t.Abort();
                    UO.PrintInformation("Heal OFF");
                }
                onOff = value;
            }
        }
        [XmlArray]
        public List<int> avaibleEquips { get; set; }

        private object Lock= new object();
        private List<Patient> healedPlayers;
        [XmlArray] // TODO upraveno na thread safe
        public List<Patient> HealedPlayers { get
            {
                lock(Lock)
                {
                    return healedPlayers;
                }
            }
            set
            {
                lock (Lock)
                {
                    healedPlayers = value;
                }
            } }
        public ushort PatientHPLimit { get; set; }

        public AutoHeal()
        {
            
            avaibleEquips = new List<int>();
            HealedPlayers = new List<Patient>();
            checker = new System.Timers.Timer(300);
            checker.Elapsed += Checker_Elapsed;

        }


        public void GetStatuses()
        {
            Serial tmp=Aliases.GetObject("laststatus");
            foreach(Patient p in HealedPlayers)
            {
                if (p.chara.Hits < 1 && p.chara.Distance < 15)
                    p.chara.RequestStatus(20);
                UO.Wait(10);
            }
            Aliases.SetObject("laststatus",tmp);
        }
        private void Checker_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<Patient> tmp = HealedPlayers.Where(pat => pat.chara.Distance < 7 && pat.chara.Hits > 0 && pat.chara.Hits < PatientHPLimit).ToList();
            tmp.Sort((x, y) => (x.chara.Hits.CompareTo(y.chara.Hits)));
            if (Main.Instance.Settings.ArrowSelfProgress || World.Player.Hidden) return;// || Main.Instance.Settings.RessurectInProgress) return;
            if (World.Player.Hits < Main.Instance.Settings.MinHP)
                PatientHurted?.Invoke(this, new HurtedPatientArgs() { selfHurted = true });
            if (tmp.Count == 0)
            {
                PatientHurted?.Invoke(this, new HurtedPatientArgs() { crystalOff = true });
                return;
            }
            if (tmp[0].chara.Hits > 60 && Main.Instance.Settings.MusicProgress) return;
           if(PatientHurted!=null)
            {
                PatientHurted(this, new HurtedPatientArgs() { pati = tmp[0] });
            }
        }


        private void ThreadChecker()
        {

            List<Patient> tmp = HealedPlayers.Where(pat => pat.chara.Distance < 7 && pat.chara.Hits > 0 && pat.chara.Hits < PatientHPLimit).ToList();
            tmp.Sort((x, y) => (x.chara.Hits.CompareTo(y.chara.Hits)));
            if (Main.Instance.Settings.ArrowSelfProgress || World.Player.Hidden) return;// || Main.Instance.Settings.RessurectInProgress) return;
            if (World.Player.Hits < Main.Instance.Settings.MinHP)
                PatientHurted?.BeginInvoke(null, new HurtedPatientArgs() { selfHurted = true },null,null);
            if (tmp.Count == 0)
            {
                PatientHurted?.BeginInvoke(null, new HurtedPatientArgs() { crystalOff = true },null,null);
                return;
            }
            if (tmp[0].chara.Hits > 60 && Main.Instance.Settings.MusicProgress) return;
            PatientHurted?.BeginInvoke(null, new HurtedPatientArgs() { pati = tmp[0] }, null, null);

            Thread.Sleep(200);

        }



        public void bandage()
        {
            if (startBandage == null) startBandage = DateTime.Now;
            if ((DateTime.Now - startBandage) > TimeSpan.FromSeconds(7)) Main.Instance.Settings.BandageDone = true;
            if (!Main.Instance.Settings.BandageDone ) return;
            startBandage = DateTime.Now;
            Main.Instance.Settings.BandageDone = false;
            if(Main.Instance.Settings.ActualCharacter==Character.EreborClass.Shaman)UO.Say(".samheal15");
            else UO.Say(".heal15");

        }


        public void bandage(Patient p)
        {
            if (startBandage == null) startBandage = DateTime.Now;
            if ((DateTime.Now - startBandage) > TimeSpan.FromSeconds(7)) Main.Instance.Settings.BandageDone = true;
            if (!Main.Instance.Settings.BandageDone) return;
            startBandage = DateTime.Now;
            Main.Instance.Settings.BandageDone = false;
            if (!Main.Instance.Settings.CrystalState) UO.Say(Main.Instance.Settings.CrystalCmd);
            UO.Say(Main.Instance.Settings.HealCmd + p.equip);
            UO.Wait(100);
            if (Main.Instance.Settings.ActualCharacter == Character.EreborClass.Shaman)
            {
                if (Main.Instance.Settings.CrystalState)
                {
                    UO.Say(Main.Instance.Settings.CrystalCmd);
                }
            }
        }
        // TODO Res

        public void Res()
        {
            UOItem bandy = null;
            if (Main.Instance.Settings.ActualCharacter == Character.EreborClass.Shaman)
                bandy = World.Player.Backpack.AllItems.FindType(0x0E21);
            else
                bandy = World.Player.Backpack.AllItems.FindType(0x0E20);
            World.FindDistance = 3;
            foreach(UOItem corps in World.Ground.Where(x=>x.Graphic==0x2006 && x.Distance<3))
            {
                corps.Click();
                corps.WaitTarget();
                if (bandy == null) return;
                bandy.Use();
                UO.Wait(200);
                if (Journal.Contains("Jako Priest nemuzes ozivovat")) continue;
                if (Journal.Contains("Duch neni ve ")) UO.Say(" dej WAR ");
                if (Journal.Contains("Necht se navrati "))
                {
                    UO.Say("Resuju ");
                    Journal.WaitForText(true, 5000, "Ozivil jsi", "Ozivila jsi");
                    return;
                }

            }

        }

        public void Add()
        {
            UO.PrintInformation("Zamer hrace");
            UOCharacter tmp = new UOCharacter(UIManager.TargetObject());
            if (HealedPlayers.Any(ch => ch.chara.Serial == tmp.Serial))
            {
                UO.PrintError("Uz je v seznamu");
                return;
            }
            if (HealedPlayers.Count >= 14)
            {
                UO.PrintError("Plny seznam");
                return;
            }
            if(HealedPlayers.Count==0)
            {
                avaibleEquips.AddRange(new List<int>() {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14 });
            }
            int selectedEq = avaibleEquips.First();
            avaibleEquips.Remove(selectedEq);
            UO.WaitTargetObject(tmp);
            UO.Say(".setequip" + selectedEq);
            HealedPlayers.Add(new Patient() { character=tmp, equip=selectedEq });

        }

        public void fillListBox(System.Windows.Forms.ListBox lb)
        {
            lb.Items.Clear();
            if (HealedPlayers.Count < 1 || HealedPlayers == null) return;
            foreach (Patient e in HealedPlayers)
            {
                lb.Items.Add(e.chara.Name == null ? e.chara.ToString() : e.chara.Name.Length < 2 ? e.chara.ToString() : e.chara.Name + ", equip: " + e.equip.ToString());
            }

        }

        public void Remove(int index)
        {
            if (index >= 0 && index >= HealedPlayers.Count) return;
            avaibleEquips.Add(HealedPlayers[index].equip);
            HealedPlayers.RemoveAt(index);

        }
    }
}
