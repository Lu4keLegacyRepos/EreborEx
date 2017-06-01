using EreborPhoenixExtension;
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
    public delegate void CheckAll();
    [PhoenixWindowTabPage("Erebor")]
    public partial class Erebor : UserControl
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        public static Erebor Instance;
        public Erebor()
        {
            try
            {
                InitializeComponent();
                Instance = this;
                tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            }
            catch(Exception ex)
            { MessageBox.Show(ex.Message); }


        }



        private void btn_Load_Click(object sender, EventArgs e)
        {

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    SetForegroundWindow(Client.HWND);
                    Main.Instance.Settings.RuneTree.GetRunes();
                    Main.Instance.Settings.RuneTree.FillTreeView( Runes);
                    break;
                case 1:
                    SetForegroundWindow(Client.HWND);
                    Main.Instance.Settings.EquipSet.Add();
                    break;
                case 2:
                    SetForegroundWindow(Client.HWND);
                    Main.Instance.Settings.Weapons.Add();
                    break;
                case 3:
                    SetForegroundWindow(Client.HWND);
                    Main.Instance.Settings.AHeal.Add();
                    break;
                case 4:
                    SetForegroundWindow(Client.HWND);
                    Main.Instance.Settings.Track.Add();
                    break;
            }
            Main.Instance.Settings.EquipSet.fillListBox(listBox1);
            Main.Instance.Settings.Weapons.fillListBox(listBox2);
            Main.Instance.Settings.AHeal.fillListBox(listBox3);
            Main.Instance.Settings.Track.fillListBox(listBox4);

        }

        private void btn_Refrash_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    Main.Instance.Settings.RuneTree.FillTreeView( Runes);
                    break;
                case 1:
                    Main.Instance.Settings.EquipSet.fillListBox(listBox1);
                    break;
                case 2:
                    Main.Instance.Settings.Weapons.fillListBox(listBox2);
                    break;
                case 3:
                    Main.Instance.Settings.AHeal.fillListBox(listBox3);
                    break;
                case 4:
                    Main.Instance.Settings.Track.fillListBox(listBox4);
                    break;
                case 5:
                   // Main.instance.Items.fillListView(listView1);
                    break;
            }


        }
        private void button9_Click(object sender, EventArgs e)
        {

            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    if (Runes.SelectedNode == null) return;
                    foreach (Rune r in Main.Instance.Settings.RuneTree.Runes.Where(run => run.Id.ToString() == Runes.SelectedNode.Tag.ToString()))
                    {
                        Main.Instance.Settings.RuneTree.findRune(r);
                        r.RecallSvitek();
                    }
                    break;
                case 1:
                    if (listBox1.SelectedIndex >= 0)
                    {
                        UO.PrintInformation("Zamer odkladaci batoh");
                        Main.Instance.Settings.EquipSet.equipy[listBox1.SelectedIndex].DressOnly();
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    if (Runes.SelectedNode == null) return;
                    foreach (Rune r in Main.Instance.Settings.RuneTree.Runes.Where(run => run.Id.ToString() == Runes.SelectedNode.Tag.ToString()))
                    {
                        Main.Instance.Settings.RuneTree.findRune(r);
                        r.Recall();
                    }
                    break;
                case 1:
                    if (listBox1.SelectedIndex >= 0)
                    {
                        SetForegroundWindow(Client.HWND);
                        UO.PrintInformation("Zamer odkladaci batoh");
                        Main.Instance.Settings.EquipSet.equipy[listBox1.SelectedIndex].Dress(new UOItem(UIManager.TargetObject()));
                    }
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    if (Runes.SelectedNode == null) return;
                    foreach (Rune r in Main.Instance.Settings.RuneTree.Runes.Where(run => run.Id.ToString() == Runes.SelectedNode.Tag.ToString()))
                    {
                        Main.Instance.Settings.RuneTree.findRune(r);
                        r.Gate();
                    }
                    break;
                case 1:
                    if(listBox1.SelectedIndex>=0)
                    Main.Instance.Settings.EquipSet.Remove(listBox1.SelectedIndex);
                    break;
                case 2:
                    if (listBox2.SelectedIndex >= 0)
                        Main.Instance.Settings.Weapons.Remove(listBox2.SelectedIndex);
                    break;
                case 3:
                    if (listBox3.SelectedIndex >= 0)
                        Main.Instance.Settings.AHeal.Remove(listBox3.SelectedIndex);
                    break;
                case 4:
                    if (listBox4.SelectedIndex >= 0)
                        Main.Instance.Settings.Track.Remove(listBox4.SelectedIndex);
                    break;
            }
            Main.Instance.Settings.EquipSet.fillListBox(listBox1);
            Main.Instance.Settings.Weapons.fillListBox(listBox2);
            Main.Instance.Settings.AHeal.fillListBox(listBox3);
            Main.Instance.Settings.Track.fillListBox(listBox4);
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(tabControl1.SelectedIndex.ToString());
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    button9.Enabled = true;
                    button9.Visible = true;
                    button9.Text = "Recall-S";

                    button8.Enabled = true;
                    button8.Visible = true;
                    button8.Text = "Recall";

                    button7.Enabled = true;
                    button7.Visible = true;
                    button7.Text = "Gate";

                    btn_Refrash.Enabled = true;
                    btn_Refrash.Visible = true;
                    btn_Refrash.Text = "Refresh";

                    btn_Load.Visible = true;
                    btn_Load.Enabled = true;
                    btn_Load.Text = "Scan";

                    break;
                case 1:
                    button9.Enabled = true;
                    button9.Visible = true;
                    button9.Text = "Dress";

                    button8.Enabled = true;
                    button8.Visible = true;
                    button8.Text = "Un/Dress";

                    button7.Enabled = true;
                    button7.Visible = true;
                    button7.Text = "Delete";

                    btn_Refrash.Enabled = true;
                    btn_Refrash.Visible = true;
                    btn_Refrash.Text = "Refresh";

                    btn_Load.Visible = true;
                    btn_Load.Enabled = true;
                    btn_Load.Text = "Add";

                    break;
                case 2:
                    button9.Enabled = false;
                    button9.Visible = false;
                    button9.Text = "Recall-S";

                    button8.Enabled = false;
                    button8.Visible = false;
                    button8.Text = "Recall";

                    button7.Enabled = true;
                    button7.Visible = true;
                    button7.Text = "Delete";

                    btn_Refrash.Enabled = true;
                    btn_Refrash.Visible = true;
                    btn_Refrash.Text = "Refresh";

                    btn_Load.Visible = true;
                    btn_Load.Enabled = true;
                    btn_Load.Text = "Add";

                    break;
                case 3:
                    button9.Enabled = false;
                    button9.Visible = false;
                    button9.Text = "Recall-S";

                    button8.Enabled = false;
                    button8.Visible = false;
                    button8.Text = "Recall";

                    button7.Enabled = true;
                    button7.Visible = true;
                    button7.Text = "Delete";

                    btn_Refrash.Enabled = true;
                    btn_Refrash.Visible = true;
                    btn_Refrash.Text = "Refresh";

                    btn_Load.Visible = true;
                    btn_Load.Enabled = true;
                    btn_Load.Text = "Add";

                    break;
                case 4:
                    button9.Enabled = false;
                    button9.Visible = false;
                    button9.Text = "Recall-S";

                    button8.Enabled = false;
                    button8.Visible = false;
                    button8.Text = "Recall";

                    button7.Enabled = true;
                    button7.Visible = true;
                    button7.Text = "Delete";

                    btn_Refrash.Enabled = true;
                    btn_Refrash.Visible = true;
                    btn_Refrash.Text = "Refresh";

                    btn_Load.Visible = true;
                    btn_Load.Enabled = true;
                    btn_Load.Text = "Add";
                    break;

            }
            button7.Refresh();
            button8.Refresh();
            button9.Refresh();
            btn_Load.Refresh();
            btn_Refrash.Refresh();
        }








        //Settings parts
        private void chbAutodrink_CheckedChanged(object sender, EventArgs e)
        {
            if(Main.Instance.Settings.AutoDrink!= chbAutodrink.Checked)
                Main.Instance.Settings.AutoDrink = chbAutodrink.Checked;

        }

        private void chbAutoArrow_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Spells.AutoArrow != chbAutoArrow.Checked)
                Main.Instance.Settings.Spells.AutoArrow = chbAutoArrow.Checked;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.HitBandage != checkBox6.Checked)
                Main.Instance.Settings.HitBandage = checkBox6.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Amorf.Amorf != checkBox1.Checked)
                Main.Instance.Settings.Amorf.Amorf = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.HideCorpses != checkBox2.Checked)
                Main.Instance.Settings.Lot.HideCorpses = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.DoLot != checkBox3.Checked)
                Main.Instance.Settings.Lot.DoLot = checkBox3.Checked;

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Food != checkBox5.Checked)
                Main.Instance.Settings.Lot.Food = checkBox5.Checked;

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Leather != checkBox4.Checked)
                Main.Instance.Settings.Lot.Leather = checkBox4.Checked;
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Bolts != checkBox13.Checked)
                Main.Instance.Settings.Lot.Bolts = checkBox13.Checked;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Extend1 != checkBox12.Checked)
                Main.Instance.Settings.Lot.Extend1 = checkBox12.Checked;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Extend2 != checkBox11.Checked)
                Main.Instance.Settings.Lot.Extend2 = checkBox11.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetForegroundWindow(Client.HWND);
            UO.PrintWarning("Zamer item, ktery chces lotit");
            UOItem it = new UOItem(UIManager.TargetObject());
            Main.Instance.Settings.Lot.extend1_type = new Graphic(it.Graphic);
            button1.Text = Main.Instance.Settings.Lot.extend1_type.ToString();
            button1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetForegroundWindow(Client.HWND);
            UO.PrintWarning("Zamer item, ktery chces lotit");
            UOItem it = new UOItem(UIManager.TargetObject());
            Main.Instance.Settings.Lot.extend2_type = new Graphic(it.Graphic);
            button2.Text = Main.Instance.Settings.Lot.extend1_type.ToString();
            button2.Refresh();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetForegroundWindow(Client.HWND);
            UO.PrintInformation("Zamer Kuchatko");
            Main.Instance.Settings.Lot.CarvTool = new UOItem(UIManager.TargetObject());




        }

        private void button11_Click(object sender, EventArgs e)
        {
            SetForegroundWindow(Client.HWND);
            UO.PrintInformation("Zamer batoh");
            Main.Instance.Settings.Lot.LotBag = new UOItem(UIManager.TargetObject());
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Feathers != checkBox9.Checked)
                Main.Instance.Settings.Lot.Feathers = checkBox9.Checked;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Gems != checkBox8.Checked)
                Main.Instance.Settings.Lot.Gems = checkBox8.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.Lot.Reageants != checkBox7.Checked)
                Main.Instance.Settings.Lot.Reageants = checkBox7.Checked;
        }




        public void CheckAll()
        {
            textBox6.Text = Main.Instance.Settings.VoodooManaLimit.ToString();
            textBox5.Text = Main.Instance.Settings.GWWidth.ToString();
            textBox7.Text = Main.Instance.Settings.GWHeight.ToString();
            textBox3.Text = Main.Instance.Settings.minHP.ToString();
            textBox2.Text = Main.Instance.Settings.criticalHits.ToString();
            textBox1.Text = Main.Instance.Settings.GoldLimit.ToString();
            checkBox7.Checked = Main.Instance.Settings.Lot.Reageants;
            checkBox8.Checked = Main.Instance.Settings.Lot.Gems;
            checkBox9.Checked = Main.Instance.Settings.Lot.Feathers;
            button2.Text = Main.Instance.Settings.Lot.extend1_type.ToString();
            button1.Text = Main.Instance.Settings.Lot.extend1_type.ToString();
            checkBox11.Checked = Main.Instance.Settings.Lot.Extend2;
            checkBox12.Checked = Main.Instance.Settings.Lot.Extend1;
            checkBox13.Checked = Main.Instance.Settings.Lot.Bolts;
            checkBox4.Checked = Main.Instance.Settings.Lot.Leather;
            checkBox5.Checked = Main.Instance.Settings.Lot.Food;
            checkBox3.Checked = Main.Instance.Settings.Lot.DoLot;
            checkBox2.Checked = Main.Instance.Settings.Lot.HideCorpses;
            checkBox1.Checked = Main.Instance.Settings.Amorf.Amorf;
            checkBox6.Checked = Main.Instance.Settings.HitBandage;
            chbAutoArrow.Checked = Main.Instance.Settings.Spells.AutoArrow;
            chbAutodrink.Checked = Main.Instance.Settings.AutoDrink;
            checkBox10.Checked = Main.Instance.Settings.PrintAnim;
            checkBox14.Checked = Main.Instance.Settings.HitTrack;

            textBox4.Text = Main.Instance.Settings.hidDelay.ToString();
            Main.Instance.Settings.RuneTree.FillTreeView(Runes);

            Main.Instance.Settings.EquipSet.fillListBox(listBox1);
            Main.Instance.Settings.Weapons.fillListBox(listBox2);
            Main.Instance.Settings.AHeal.fillListBox(listBox3);

        }

        private void button3_Click(object sender, EventArgs e)
        {

            Main.Instance.Settings.HotKeys.Add();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Main.Instance.Settings.HotKeys.Clear();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);

        }


        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.GoldLimit != ushort.Parse(textBox1.Text))
                Main.Instance.Settings.GoldLimit = ushort.Parse(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.criticalHits != uint.Parse(textBox2.Text))
                Main.Instance.Settings.criticalHits = uint.Parse(textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.minHP != uint.Parse(textBox3.Text))
                Main.Instance.Settings.minHP = uint.Parse(textBox3.Text);
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.PrintAnim != checkBox10.Checked)
                Main.Instance.Settings.PrintAnim = checkBox10.Checked;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.hidDelay != uint.Parse(textBox4.Text))
                Main.Instance.Settings.hidDelay = uint.Parse(textBox4.Text);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.HitTrack != checkBox14.Checked)
                Main.Instance.Settings.HitTrack = checkBox14.Checked;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.GWWidth != int.Parse(textBox5.Text))
                Main.Instance.Settings.GWWidth = int.Parse(textBox5.Text);
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.GWHeight != int.Parse(textBox7.Text))
                Main.Instance.Settings.GWHeight = int.Parse(textBox7.Text);
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetForegroundWindow(Client.HWND);
            UOItem pois;
            UO.PrintInformation("Zamer Poison");
            Main.Instance.Settings.Poisoning.PoisonBottle = new UOItem(UIManager.TargetObject());
            pois = new UOItem(Main.Instance.Settings.Poisoning.PoisonBottle);
            pois.Click();
            UO.Wait(200);
            label10.Text = pois.Name;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (Main.Instance.Settings.VoodooManaLimit != uint.Parse(textBox6.Text))
                Main.Instance.Settings.VoodooManaLimit = uint.Parse(textBox6.Text);
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
        }



    }
}
