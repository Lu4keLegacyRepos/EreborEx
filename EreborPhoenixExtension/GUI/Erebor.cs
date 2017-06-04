using EreborPhoenixExtension.Libs.Runes;
using Phoenix;
using Phoenix.WorldData;
using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EreborPhoenixExtension.GUI

{

    [PhoenixWindowTabPage("Erebor")]
    public partial class Erebor : UserControl
    {
        public event EventHandler<EventChangedArgs> Changed;
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        public Erebor()
        {
            try
            {
                InitializeComponent();
                Main.Instance.EreborInstance = this;
                TabC.SelectedIndexChanged += TabControl1_SelectedIndexChanged;

                #region ChangeEvents
                chb_AutoArrow.CheckedChanged += ControlChanged;
                chb_AutoDrink.CheckedChanged += ControlChanged;
                chb_Automorf.CheckedChanged += ControlChanged;
                chb_Bolts.CheckedChanged += ControlChanged;
                chb_CorpseHide.CheckedChanged += ControlChanged;
                chb_Extend1.CheckedChanged += ControlChanged;
                chb_Extend2.CheckedChanged += ControlChanged;
                chb_feathers.CheckedChanged += ControlChanged;
                chb_Food.CheckedChanged += ControlChanged;
                chb_gems.CheckedChanged += ControlChanged;
                chb_HitBandage.CheckedChanged += ControlChanged;
                chb_HitTrack.CheckedChanged += ControlChanged;
                chb_Leather.CheckedChanged += ControlChanged;
                chb_lot.CheckedChanged += ControlChanged;
                chb_regeants.CheckedChanged += ControlChanged;
                chb_StoodUps.CheckedChanged += ControlChanged;

                btn_0.Click += ControlChanged;
                btn_1.Click += ControlChanged;
                btn_2.Click += ControlChanged;
                btn_3.Click += ControlChanged;
                btn_4.Click += ControlChanged;
                btn_AddHotkeys.Click += ControlChanged;
                btn_ClearHotkeys.Click += ControlChanged;
                btn_Extend1.Click += ControlChanged;
                btn_Extend2.Click += ControlChanged;
                btn_SetBag.Click += ControlChanged;
                btn_SetCarv.Click += ControlChanged;
                btn_Pois.Click += ControlChanged;

                tb_GoldLimit.TextChanged += ControlChanged;
                tb_GwHeight.TextChanged += ControlChanged;
                tb_GwWidth.TextChanged += ControlChanged;
                tb_HidDelay.TextChanged += ControlChanged;
                tb_Hits2Pot.TextChanged += ControlChanged;
                tb_MinHpBandage.TextChanged += ControlChanged;
                tb_Obet.TextChanged += ControlChanged;


                ///

                chb_skipCopper.CheckedChanged += ControlChanged;
                chb_skipIron.CheckedChanged += ControlChanged;
                chb_skipSilicon.CheckedChanged += ControlChanged;
                chb_skipVerite.CheckedChanged += ControlChanged;

                chb_dropCopper.CheckedChanged += ControlChanged;
                chb_dropIron.CheckedChanged += ControlChanged;
                chb_dropSilicon.CheckedChanged += ControlChanged;
                chb_dropVerite.CheckedChanged += ControlChanged;

                chb_crystal.CheckedChanged += ControlChanged;
                chb_autoStockUp.CheckedChanged += ControlChanged;
                chb_autoRemoveObstacles.CheckedChanged += ControlChanged;
                chb_useBank.CheckedChanged += ControlChanged;

                num_Obs.ValueChanged += ControlChanged;
                num_Ada.ValueChanged += ControlChanged;

                btn_setBankPos.Click += ControlChanged;
                btn_setGemBox.Click += ControlChanged;
                btn_setRunesPos.Click += ControlChanged;
                btn_setDoor.Click += ControlChanged;
                btn_setOreBox.Click += ControlChanged;


                #endregion

            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }


        }

        private void ControlChanged(object sender, EventArgs e)
        {

            if(sender is Button)
            {

                if (Changed != null)
                    Changed(sender, new EventChangedArgs() { btnName = ((Button)sender).Name, SelectedTabID=TabC.SelectedIndex, TextValue=string.Empty });
                return;
            }
            else if(sender is TextBox)
            {

                if (Changed != null)
                    Changed(sender, new EventChangedArgs() { TextValue=((TextBox)sender).Text });
                return;
            }
            if (Changed != null)
                Changed(sender, new EventChangedArgs());
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(tabControl1.SelectedIndex.ToString());
            switch (TabC.SelectedIndex)
            {
                case 0:
                    btn_0.Enabled = true;
                    btn_0.Visible = true;
                    btn_0.Text = "Recall-S";

                    btn_1.Enabled = true;
                    btn_1.Visible = true;
                    btn_1.Text = "Recall";

                    btn_2.Enabled = true;
                    btn_2.Visible = true;
                    btn_2.Text = "Gate";

                    btn_4.Enabled = true;
                    btn_4.Visible = true;
                    btn_4.Text = "Refresh";

                    btn_3.Visible = true;
                    btn_3.Enabled = true;
                    btn_3.Text = "Scan";

                    break;
                case 1:
                    btn_0.Enabled = true;
                    btn_0.Visible = true;
                    btn_0.Text = "Dress";

                    btn_1.Enabled = true;
                    btn_1.Visible = true;
                    btn_1.Text = "Un/Dress";

                    btn_2.Enabled = true;
                    btn_2.Visible = true;
                    btn_2.Text = "Delete";

                    btn_4.Enabled = true;
                    btn_4.Visible = true;
                    btn_4.Text = "Refresh";

                    btn_3.Visible = true;
                    btn_3.Enabled = true;
                    btn_3.Text = "Add";

                    break;
                case 2:
                    btn_0.Enabled = false;
                    btn_0.Visible = false;
                    btn_0.Text = "Recall-S";

                    btn_1.Enabled = false;
                    btn_1.Visible = false;
                    btn_1.Text = "Recall";

                    btn_2.Enabled = true;
                    btn_2.Visible = true;
                    btn_2.Text = "Delete";

                    btn_4.Enabled = true;
                    btn_4.Visible = true;
                    btn_4.Text = "Refresh";

                    btn_3.Visible = true;
                    btn_3.Enabled = true;
                    btn_3.Text = "Add";

                    break;
                case 3:
                    btn_0.Enabled = false;
                    btn_0.Visible = false;
                    btn_0.Text = "Recall-S";

                    btn_1.Enabled = false;
                    btn_1.Visible = false;
                    btn_1.Text = "Recall";

                    btn_2.Enabled = true;
                    btn_2.Visible = true;
                    btn_2.Text = "Delete";

                    btn_4.Enabled = true;
                    btn_4.Visible = true;
                    btn_4.Text = "Refresh";

                    btn_3.Visible = true;
                    btn_3.Enabled = true;
                    btn_3.Text = "Add";

                    break;
                case 4:
                    btn_0.Enabled = false;
                    btn_0.Visible = false;
                    btn_0.Text = "Recall-S";

                    btn_1.Enabled = false;
                    btn_1.Visible = false;
                    btn_1.Text = "Recall";

                    btn_2.Enabled = true;
                    btn_2.Visible = true;
                    btn_2.Text = "Delete";

                    btn_4.Enabled = true;
                    btn_4.Visible = true;
                    btn_4.Text = "Refresh";

                    btn_3.Visible = true;
                    btn_3.Enabled = true;
                    btn_3.Text = "Add";
                    break;
                case 5:
                    btn_0.Enabled = false;
                    btn_0.Visible = false;
                    btn_0.Text = "Recall-S";

                    btn_1.Enabled = false;
                    btn_1.Visible = false;
                    btn_1.Text = "Recall";

                    btn_2.Enabled = false;
                    btn_2.Visible = false;
                    btn_2.Text = "Delete";

                    btn_4.Enabled = true;
                    btn_4.Visible = true;
                    btn_4.Text = "Refresh";

                    btn_3.Visible = true;
                    btn_3.Enabled = true;
                    btn_3.Text = "Select";
                    break;

            }
            btn_2.Refresh();
            btn_1.Refresh();
            btn_0.Refresh();
            btn_3.Refresh();
            btn_4.Refresh();
        }



        public void tt() { }


        #region ControlProperties

        public bool Food
        {
            get
            {
                return chb_Food.Checked;

            }

            set
            {
                chb_Food.Checked = value;
            }
        }

        public bool Leather
        {
            get
            {
                return chb_Leather.Checked;
            }

            set
            {
                chb_Leather.Checked = value;
            }
        }

        public bool Bolts
        {
            get
            {
                return chb_Bolts.Checked;
            }

            set
            {
                chb_Bolts.Checked = value;
            }
        }

        public bool Extend1
        {
            get
            {
                return chb_Extend1.Checked;
            }

            set
            {
                chb_Extend1.Checked = value;
            }
        }

        public bool Extend2
        {
            get
            {
                return chb_Extend2.Checked;
            }

            set
            {
                chb_Extend2.Checked = value;
            }
        }

        public bool Lot
        {
            get
            {
                return chb_lot.Checked;
            }

            set
            {
                chb_lot.Checked = value;
            }
        }

        public bool Feathers
        {
            get
            {
                return chb_feathers.Checked;
            }

            set
            {
                chb_feathers.Checked = value;
            }
        }

        public bool Gems
        {
            get
            {
                return chb_gems.Checked;
            }

            set
            {
                chb_gems.Checked = value;
            }
        }

        public bool Regeants
        {
            get
            {
                return chb_regeants.Checked;
            }

            set
            {
                chb_regeants.Checked = value;
            }
        }

        public bool CorpsesHide
        {
            get
            {
                return chb_CorpseHide.Checked;
            }

            set
            {
                chb_CorpseHide.Checked = value;
            }
        }

        public bool AutoMorf
        {
            get
            {
                return chb_Automorf.Checked;
            }

            set
            {
                chb_Automorf.Checked = value;
            }
        }

        public bool HitBandage
        {
            get
            {
                return chb_HitBandage.Checked;
            }

            set
            {
                chb_HitBandage.Checked = value;
            }
        }

        public bool HitTrack
        {
            get
            {
                return chb_HitTrack.Checked;
            }

            set
            {
                chb_HitTrack.Checked = value;
            }
        }

        public bool AutoArrow
        {
            get
            {
                return chb_AutoArrow.Checked;
            }

            set
            {
                chb_AutoArrow.Checked = value;
            }
        }

        public bool AutoDrink
        {
            get
            {
                return chb_AutoDrink.Checked;
            }

            set
            {
                chb_AutoDrink.Checked = value;
            }
        }

        public bool StoodUps
        {
            get
            {
                return chb_StoodUps.Checked;
            }

            set
            {
                chb_StoodUps.Checked = value;
            }
        }

        public TreeView RuneView
        {
            get
            {
                return tw_Runes;
            }
        }
        public ListBox EquipList
        {
            get
            {
                return lb_Equips;
            }
        }

        public ListBox WeaponList
        {
            get
            {
                return lb_Weapons;
            }
        }

        public ListBox HealList
        {
            get
            {
                return lb_Healing;
            }
        }

        public ListBox TrackIgnoreList
        {
            get
            {
                return lb_TrackIgnore;
            }
        }
        public ListBox MiningList
        {
            get
            {
                return lb_mining;
            }
        }

        public string Extend1Type_Text
        {
            get
            {
                return btn_Extend1.Text;
            }
            set
            {
                btn_Extend1.Text = value;
            }
        }
        public string Extend2Type_Text
        {
            get
            {
                return btn_Extend2.Text;
            }
            set
            {
                btn_Extend2.Text = value;
            }
        }

        public string SelectedRuneID
        {
            get
            {
                if (RuneView.SelectedNode == null) return string.Empty;
                return RuneView.SelectedNode.Tag.ToString();
            }
        }
        public int SelectedEquip
        {
            get
            {
                return lb_Equips.SelectedIndex;
            }
        }
        public int SelectedWeapon
        {
            get
            {
                return lb_Weapons.SelectedIndex;
            }
        }

        public int SelectedHealed
        {
            get
            {
                return lb_Healing.SelectedIndex;
            }
        }

        public int SelectedIgnored
        {
            get
            {
                return lb_TrackIgnore.SelectedIndex;
            }
        }

        public int SelectedMine
        {
            get
            {
                return lb_mining.SelectedIndex;
            }
        }
        public string PoisType
        {
            set
            {
                lbl_pois.Text = value;
               
            }
        }



        public string GoldLimit
        {
            get { return tb_GoldLimit.Text; }
            set { tb_GoldLimit.Text = value; }
        }
        public string GwHeight
        {
            get { return tb_GwHeight.Text; }
            set { tb_GwHeight.Text = value; }
        }
        public string GwWidth
        {
            get { return tb_GwWidth.Text; }
            set { tb_GwWidth.Text = value; }
        }
        public string HidDelay
        {
            get { return tb_HidDelay.Text; }
            set { tb_HidDelay.Text = value; }
        }
        public string Hits2Pot
        {
            get { return tb_Hits2Pot.Text; }
            set { tb_Hits2Pot.Text = value; }
        }
        public string MinHp
        {
            get { return tb_MinHpBandage.Text; }
            set { tb_MinHpBandage.Text = value; }
        }
        public string VoodooObet
        {
            get { return tb_Obet.Text; }
            set { tb_Obet.Text = value; }
        }






        public bool SkipCopper
        {
            get
            {
                return chb_skipCopper.Checked;
            }

            set
            {
                chb_skipCopper.Checked = value;
            }
        }

        public bool SkipIron
        {
            get
            {
                return chb_skipIron.Checked;
            }

            set
            {
                chb_skipIron.Checked = value;
            }
        }

        public bool SkipSilicon
        {
            get
            {
                return chb_skipSilicon.Checked;
            }

            set
            {
                chb_skipSilicon.Checked = value;
            }
        }

        public bool SkipVerite
        {
            get
            {
                return chb_skipVerite.Checked;
            }

            set
            {
                chb_skipVerite.Checked = value;
            }
        }

        public bool DropCopper
        {
            get
            {
                return chb_dropCopper.Checked;
            }

            set
            {
                chb_dropCopper.Checked = value;
            }
        }

        public bool DropIron
        {
            get
            {
                return chb_dropIron.Checked;
            }

            set
            {
                chb_dropIron.Checked = value;
            }
        }

        public bool DropSilicon
        {
            get
            {
                return chb_dropSilicon.Checked;
            }

            set
            {
                chb_dropSilicon.Checked = value;
            }
        }

        public bool DropVerite
        {
            get
            {
                return chb_dropVerite.Checked;
            }

            set
            {
                chb_dropVerite.Checked = value;
            }
        }

        public bool Crystal
        {
            get
            {
                return chb_crystal.Checked;
            }

            set
            {
                chb_crystal.Checked = value;
            }
        }

        public bool AutoStockUp
        {
            get
            {
                return chb_autoStockUp.Checked;
            }

            set
            {
                chb_autoStockUp.Checked = value;
            }
        }

        public bool AutoRemoveObstacles
        {
            get
            {
                return chb_autoRemoveObstacles.Checked;
            }

            set
            {
                chb_autoRemoveObstacles.Checked = value;
            }
        }

        public bool UseBank
        {
            get
            {
                return chb_useBank.Checked;
            }

            set
            {
                chb_useBank.Checked = value;
            }
        }

        public int MaxObs
        {
            get
            {
                return (int)num_Obs.Value;
            }

            set
            {
                num_Obs.Value = value;
            }
        }

        public int MaxAda
        {
            get
            {
                return (int)num_Ada.Value;
            }

            set
            {
                num_Ada.Value = value;
            }
        }


        public string CopperCount
        {
            set
            {
                lbl_copperCnt.Text = value;
            }
        }

        public string IronCount
        {
            set
            {
                lbl_ironCnt.Text = value;
            }
        }
        public string SiliconCount
        {
            set
            {
                lbl_siliconCnt.Text = value;
            }
        }
        public string VeriteCount
        {
            set
            {
                lbl_veriteCnt.Text = value;
            }
        }
        public string ValoriteCount
        {
            set
            {
                lbl_valoriteCount.Text = value;
            }
        }
        public string ObsidianCount
        {
            set
            {
                lbl_obsidianCnt.Text = value;
            }
        }
        public string AdamantiumCount
        {
            set
            {
                lbl_adamantiumCnt.Text = value;
            }
        }


        #endregion
    }
}
