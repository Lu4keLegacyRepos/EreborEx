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
        Dictionary<uint, string> que_Boost;
        private bool boosting;
        private bool vodo;
        DateTime timee;
        private VoodooState done;
        private int HeadsCount;
        List<Graphic> Hlavy = new List<Graphic>() { 0x1DAE, 0x1DA0, 0x1CE9, 0x1CE1 };

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
            que_Boost = new Dictionary<uint,string>();
            
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

        private void getBottles(bool something)
        {
            boostBottles.Add("str", new UOItem(bagl.Items.FindType(0x0F0E, 0x0835)));
            boostBottles.Add("dex", new UOItem(bagl.Items.FindType(0x0F0E, 0x0006)));
            boostBottles.Add("int", new UOItem(bagl.Items.FindType(0x0F0E, 0x06C2)));
            boostBottles.Add("def", new UOItem(bagl.Items.FindType(0x0F0E, 0x0999)));
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
            ushort tmpX, tmpY;
            done = VoodooState.Fail;
            boosting = true;
            bagl.Use();
            UO.Wait(100);
            if (bagl.Items.CountItems() != HeadsCount)
            {
                UO.Say("Prepocitavam");
                Journal.EntryAdded -= Journal_EntryAdded;
                que.added -= Que_added;
                boostBottles.Clear();
                Heads.Clear();
                que.que.Clear();
                UO.Print("OFF");
                getBottles(true);
                getHeads();
                Journal.EntryAdded += Journal_EntryAdded;
                que.added += Que_added;
                Journal.ClearAll();
                timee = DateTime.Now;
                UO.Print("ON");

            }
            boosting = false;
            Core.RegisterServerMessageCallback(0x1C, onVoodoo);
            foreach (string it in Heads.Keys)
            {
                if(it==args[0])
                {
                    tmpX = Heads[it].X;
                    tmpY = Heads[it].Y;
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
                    Heads[it].Move(1, bagl, tmpX, tmpY);
                    UO.Wait(300);
                    boosting = false;
                    Core.UnregisterServerMessageCallback(0x1C, onVoodoo);
                    break;
                }
            }
            if (bagl.Items.CountItems() != HeadsCount) getHeads();
            boosting = false;
        }



        public void SetBoost(UOItem head, string type)
        {
            if (head == null || head == 0) return;
            switch(type)
            {
                case "str":
                    head.Click();
                    UO.Wait(200);
                    if (que_Boost.ContainsKey(head.Serial))
                        que_Boost.Remove(head.Serial);
                    que_Boost.Add(head.Serial, "str");

                    break;
                case "dex":
                    head.Click();
                    UO.Wait(200);
                    if (que_Boost.ContainsKey(head.Serial))
                        que_Boost.Remove(head.Serial);
                    que_Boost.Add(head.Serial, "dex");

                    break;
                case "int":
                    head.Click();
                    UO.Wait(200);
                    if (que_Boost.ContainsKey(head.Serial))
                        que_Boost.Remove(head.Serial);
                    que_Boost.Add(head.Serial, "int");

                    break;
                case "def":
                    head.Click();
                    UO.Wait(200);
                    if (que_Boost.ContainsKey(head.Serial))
                        que_Boost.Remove(head.Serial);
                    que_Boost.Add(head.Serial, "def");

                    break;
            }
        }

        public void ManualBoost(UOItem bagl)
        {
            ushort x, y;
            try
            {

                boostBottles.Clear();
                boostBottles.Add("str", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0835)));
                boostBottles.Add("dex", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0006)));
                boostBottles.Add("int", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x06C2)));
                boostBottles.Add("def", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0999)));
                Core.RegisterServerMessageCallback(0x1C, onVoodoo,CallbackPriority.High);
                bagl.Use();
                foreach (UOItem it in bagl.Items)
                {
                    x = it.X;
                    y = it.Y;
                    if (Hlavy.Contains(it.Graphic) && que_Boost.ContainsKey(it.Serial))
                    {
                        done = VoodooState.Fail;
                        it.Move(1, World.Player.Backpack);
                        UO.Wait(100);
                        while (done != VoodooState.Wait)
                        {
                            boostBottles[que_Boost[it.Serial]].WaitTarget();
                            it.Use();
                            UO.Wait(500);
                        }
                        if (que_Boost[it.Serial] == "def")
                        {
                            UO.Wait(100);
                            it.Move(1, bagl, x, y);
                            continue;
                        }
                        UO.Wait(4000);
                        done = VoodooState.Redo;
                        while (done != VoodooState.Wait)
                        {

                            boostBottles["def"].WaitTarget();
                            it.Use();
                            UO.Wait(500);
                        }
                        while (done != VoodooState.Success) UO.Wait(100);
                        UO.Wait(100);
                        it.Move(1, bagl,x,y);

                    }
                }
            }
            catch(Exception ex) { UO.PrintError(ex.Message); }
            finally
            {
                Core.UnregisterServerMessageCallback(0x1C, onVoodoo);
            }

        }

        public void selfBoost(string type)
        {
            try
            {
                boostBottles.Clear();
                boostBottles.Add("str", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0835)));
                boostBottles.Add("dex", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0006)));
                boostBottles.Add("int", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x06C2)));
                boostBottles.Add("def", new UOItem(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0999)));
                Core.RegisterServerMessageCallback(0x1C, onVoodoo, CallbackPriority.High);
                UOItem it = null;
                foreach (UOItem i in World.Player.Backpack.AllItems)
                {
                    if (i.Graphic == 0x1CE1 )
                    {
                        i.Click();
                        UO.Wait(150);
                        if(i.Name == World.Player.Name) it = i;
                    }
                }
                if (it == null)
                {
                    UO.Print("Nemas svou hlavu");
                    if (type == "int")
                    {
                        UO.Cast(StandardSpell.Cunning, World.Player);
                    }
                }
                done = VoodooState.Fail;
                while (done != VoodooState.Wait)
                {
                    boostBottles[type].WaitTarget();
                    it.Use();
                    UO.Wait(500);
                }
                UO.Wait(4000);
                done = VoodooState.Redo;
                while (done != VoodooState.Wait)
                {

                    boostBottles["def"].WaitTarget();
                    it.Use();
                    UO.Wait(500);
                }
                while (done != VoodooState.Success) UO.Wait(100);
                UO.Wait(100);
            }
            catch (Exception ex) { UO.PrintError(ex.Message); }
            finally
            {
                Core.UnregisterServerMessageCallback(0x1C, onVoodoo);
            }

        }
            
        

        CallbackResult onVoodoo(byte[] data , CallbackResult prev)//0x1C
        {
            AsciiSpeech ass = new AsciiSpeech(data);
            if (ass.Text.Contains("Nepovedlo se")) done = VoodooState.Fail;
            if (ass.Text.Contains("Cil podlehl ")) done = VoodooState.Success;
            if (ass.Text.Contains("Jeste nelze pouzit.")) done = VoodooState.Redo;
            if (ass.Text.Contains("prokleti voodoo seslano uspesne")) done = VoodooState.Wait;
            if (ass.Text.Contains("prokleti nenalezlo cil.")) done = VoodooState.Wait;
            return CallbackResult.Normal;
        }
    }
}
