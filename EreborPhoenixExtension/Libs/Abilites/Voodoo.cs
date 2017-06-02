using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;

namespace EreborPhoenixExtension.Libs.Abilites
{
    public class Voodoo
    {
        Dictionary<string, UOItem> Heads;
        UOItem bagl;
        string[] boostCalls = {"str","dex","int" };
        Dictionary<string, UOItem> boostBottles;
        QueueEx que;
        private bool boosting;
        private bool vodo;
        DateTime timee;
        private VoodooState done;
        private int HeadsCount;

        enum VoodooState
        {
            Fail,
            Success,
            Wait,
            Redo
        }

        public Voodoo()
        {
            boosting = false;
            Heads = new Dictionary<string, UOItem>();
            boostBottles = new Dictionary<string, UOItem>();
            que = new QueueEx();
            
        }

        public void Sacrafire(Action bandage)
        {
            if (World.Player.Mana == World.Player.MaxMana)
            {
                UO.PrintInformation("Mas plnou manu!!");
                if (World.Player.Hits < World.Player.MaxHits) bandage();
                return;
            }
            if (World.Player.Hits > 70)
            {
                if (World.Player.Hits < World.Player.MaxHits)
                {
                    bandage();
                    UO.Say(".voodooobet");
                }
                else
                {
                    UO.Say(".voodooobet");
                    UO.Wait(100);
                    bandage();
                }
                UO.Wait(100);
            }
            else UO.PrintWarning("malo HP!!");
        }

        public bool VoDo
        {
            get
            {
                return vodo;


            }
            set
            {
                if (value)
                {
                    getBottles();
                    getHeads();
                    Journal.EntryAdded += Journal_EntryAdded;
                    que.added += Que_added;
                    Journal.ClearAll();
                    timee = DateTime.Now;
                    UO.Print("ON");
                }
                else
                {
                    Journal.EntryAdded -= Journal_EntryAdded;
                    que.added -= Que_added;
                    boostBottles.Clear();
                    Heads.Clear();
                    que.que.Clear();
                    UO.Print("OFF");
                }
                vodo = value;
            }
        }
        

        

        private void Que_added(object sender, EventArgs e)
        {
            while (boosting) UO.Wait(100);
            boost(que.Deque());
        }

        private void Journal_EntryAdded(object sender, JournalEntryAddedEventArgs e)
        {
           foreach(string s in boostCalls)
            {
                if(e.Entry.Text==s )
                {
                    UO.Print(e.Entry.Name + ":  " + e.Entry.Text);
                    que.Enque(e.Entry.Name + ";" + e.Entry.Text);

                        
                }
            }
            if (DateTime.Now - timee > TimeSpan.FromMinutes(12)) selfFeed();
        }

        private void getBottles()
        {
            UO.PrintInformation("Zamer pytlik s Potiony a Hlavami");
            UOItem head = new UOItem(UIManager.TargetObject());
            bagl = head;
            head.Use();

            boostBottles.Add("str", new UOItem(head.Items.FindType(0x0F0E, 0x0835)));
            boostBottles.Add("dex", new UOItem(head.Items.FindType(0x0F0E, 0x0006)));
            boostBottles.Add("int", new UOItem(head.Items.FindType(0x0F0E, 0x06C2)));
            boostBottles.Add("def", new UOItem(head.Items.FindType(0x0F0E, 0x0999)));
        }
        private void getHeads()
        {
            HeadsCount = bagl.Items.CountItems();
            Heads.Clear();
            foreach(UOItem it in bagl.Items)
            {
                if (it.Graphic == 0x0F0E) continue;
                it.Click();
                UO.Wait(200);
                if(!Heads.ContainsKey(it.Name))Heads.Add(it.Name, it);
            }
            
        }
        [Command]
        public void MoveHead()
        {
            UO.PrintWarning("Zamer hrace");
            UOCharacter chara = new UOCharacter(UIManager.TargetObject());
            chara.Click();
            UO.Wait(200);
            if (Heads.Count < 1)
            {
                UO.PrintWarning("Zamer bagl s hlavama");
                bagl = new UOItem(UIManager.TargetObject());
                getHeads();
            }
            if (Heads.ContainsKey(chara.Name)) Heads[chara.Name].Move(1, World.Player.Backpack);
            else UO.PrintError("Hlava nenalezena");
        }

        private void selfFeed()
        {
            boosting = true;
            World.FindDistance = 4;
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            UO.Cast(StandardSpell.NightSight, World.Player);
            UO.Wait(3333);
            timee = DateTime.Now;
            boosting = false;
        }
        private void boost(string[] args)
        {

            done = VoodooState.Fail;
            boosting = true;
            bagl.Use();
            UO.Wait(100);
            if (bagl.Items.CountItems() != HeadsCount)
            {
                UO.Say("Prepocitavam");
                getHeads();
            }
            boosting = false;
            Core.RegisterServerMessageCallback(0x1C, onVoodoo);
            foreach (string it in Heads.Keys)//.Where(x => x.Graphic != 0x0F0E && x.Name==args[0]).ToList())
            {
                if(it==args[0])
                {
                    Heads[it].Move(1, World.Player.Backpack);
                    UO.Wait(200);
                    while (done != VoodooState.Wait)
                    {
                        boostBottles[args[1]].WaitTarget();
                        Heads[it].Use();
                        UO.Wait(500);
                    }
                    UO.Wait(4100);
                    done = VoodooState.Redo;
                    while (done != VoodooState.Wait)
                    {
                        boostBottles["def"].WaitTarget();
                        Heads[it].Use();
                        UO.Wait(500);
                    }
                    while (done != VoodooState.Success) UO.Wait(500);

                    UO.Wait(500);
                    Heads[it].Move(1, bagl);
                    UO.Wait(300);
                    boosting = false;
                    Core.UnregisterServerMessageCallback(0x1C, onVoodoo);
                    break;
                }
            }
            if (bagl.Items.CountItems() != HeadsCount) getHeads();
            boosting = false;
        }



        CallbackResult onVoodoo(byte[] data , CallbackResult prev)//0x1C
        {
            AsciiSpeech ass = new AsciiSpeech(data);
            if (ass.Text.Contains("Nepovedlo se")) done = VoodooState.Fail;
            if (ass.Text.Contains("Cil podlehl voodoo!")) done = VoodooState.Success;
            if (ass.Text.Contains("Jeste nelze pouzit.")) done = VoodooState.Redo;
            if (ass.Text.Contains("prokleti voodoo seslano uspesne")) done = VoodooState.Wait;
            return CallbackResult.Normal;
        }
    }
}
