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
        UOCharacter ch;
        UOItem Mace;
        List<string> TopMonster = new List<string>() { "golem", "spirit" };
        Graphic[] Humanoid = { 0x0191, 0x0190 };
        Point ActualPositon;
        public Battle(Action<int,int> moveTo, Action moveFarestField, UOCharacter Ch, UOItem mace)
        {
            MoveFar = moveFarestField;
            MoveTo = moveTo;
            ch = Ch;
            Mace = mace;
            ActualPositon = new Point(World.Player.X, World.Player.Y);
        }
        public void Kill()
        {
            Journal.Clear();
            ch.Click();
            UO.Wait(200);
            foreach (string s in TopMonster)
            {
                if (ch.Name.ToLower().Contains(s))

                {
                    Run();
                    return;
                }


            }
            foreach (Graphic g in Humanoid)
            {
                if (ch.Model == g)
                {
                    Run();
                    return;
                }
                
            }
            UO.Attack(ch);
            Mace.Equip();
            while (!Journal.Contains(true, "Ziskala jsi ", "Ziskal jsi ", "gogo"))
            {

                if(ch.Distance>1) MoveTo(ch.X, ch.Y);
                if (Journal.Contains("Vysavas zivoty!"))
                {
                    Journal.SetLineText(Journal.Find("Vysavas zivoty!"), " ");
                    UO.Say(".heal15");
                    Mace.Equip();
                    UO.Attack(ch);
                    Journal.WaitForText(true, 7000, " byl uspesne osetren", "Leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.");
                }
                UO.Wait(100);
            }
            MoveTo(ActualPositon.X, ActualPositon.Y);
        }

        private void Run()
        {
            while (ch.Distance < 19)
            {
                if (World.Player.Dead) return;
                MoveFar();
            }
        }
    }
}
