using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EreborPhoenixExtension.Libs.Skills.Mining
{
    public class Battle  // TODO Battle system
    {
        Action<int,int> MoveTo;
        Action MoveFar;
        Action<int> Move5;
        UOCharacter ch;
        UOItem Mace;
        List<string> TopMonster = new List<string>() { "golem", "spirit" };
        Graphic[] Humanoid = { 0x0191, 0x0190 };
        Point ActualPositon;
        private bool StoodUp;

        public Battle(Action<int,int> moveTo, Action moveFarestField, Action<int> move5Field, UOCharacter Ch, UOItem mace)
        {
            MoveFar = moveFarestField;
            MoveTo = moveTo;
            Move5 = move5Field;
            ch = Ch;
            Mace = mace;
            ActualPositon = new Point(World.Player.X, World.Player.Y);
        }
        public void Kill()
        {
            Journal.Clear();
            if (!Main.Instance.Settings.PrintAnim) Main.Instance.Settings.PrintAnim = true;
            Main.Instance.Settings.OnStoodUp += Settings_OnStoodUp;
            ch.Click();
            UO.Wait(200);
            foreach (string s in TopMonster)
            {
                if (ch.Name.ToLower().Contains(s))

                {
                    Run( false);
                    return;
                }


            }
            foreach (Graphic g in Humanoid)
            {
                if (ch.Model == g)
                {
                    Run( true);
                    return;
                }
                
            }
            UO.Attack(ch);
            Mace.Equip();
            if (ch.Distance > 1) MoveTo(ch.X, ch.Y);
            while (!Journal.Contains(true, "Ziskala jsi ", "Ziskal jsi ", "gogo"))
            {
                if(!ch.Exist || ch.Hits<1)
                {
                    UO.Wait(1000);
                    //Main.Instance.Settings.OnStoodUp -= Settings_OnStoodUp;
                    MoveTo(ActualPositon.X, ActualPositon.Y);
                    return;
                }
               
                if (ch.Distance > 1) MoveTo(ch.X, ch.Y);

                if (Journal.Contains("Vysavas zivoty!"))
                {
                    Journal.SetLineText(Journal.Find("Vysavas zivoty!"), " ");
                    UO.Say(".heal15");
                    Mace.Equip();
                    try
                    {
                        UO.Attack(ch);
                    }
                    catch { }
                    Journal.WaitForText(true, 7000, " byl uspesne osetren", "Leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.");
                }
                UO.Wait(100);
            }
           // Main.Instance.Settings.OnStoodUp -= Settings_OnStoodUp;
            MoveTo(ActualPositon.X, ActualPositon.Y);
        }

        private void Settings_OnStoodUp(object sender, EventArgs e)
        {
            StoodUp = true;
        }

        private void Run(bool recall)
        {
            bool first = true;
            while (ch.Distance < 19)
            {
                if(first)
                {
                    if (recall) UO.Say(".recallhome");
                    first = false;
                }
                if (World.Player.Dead) UO.TerminateAll();
                MoveFar();
            }
        }
    }
}
