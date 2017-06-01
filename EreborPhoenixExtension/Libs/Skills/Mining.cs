//using Phoenix;
//using Phoenix.Communication;
//using Phoenix.WorldData;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

////namespace EreborPhoenixExtension.Libs.Skills
//{

//    public class Mining
//    {
//        private Serial baglik = 0x000;
//        private ushort[] baglCoord = { 0, 0 };
//        private Keys NORTH = System.Windows.Forms.Keys.PageUp;
//        private Keys SOUTH = System.Windows.Forms.Keys.End;

//        string alarmPath = "C:\\afk.wav";
//        UOItem pickaxe, weapon;
//        UOPlayer p = World.Player;
//        string[] calls = { "You put ", "Nevykopala jsi nic ", "Jeste nemuzes pouzit skill", " There is no ore", "too far", "Try mining", " is attacking ", "afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };

//        private Keys last;
//        private UOItem materialBag = new UOItem(0x40053BD2);
//        private UOItem resourcesBag = new UOItem(0x40044375);
//        private Graphic oreType = 0x19B7;

//        public Mining()
//        {
//        }

//        public void mine()
//        {

//            Journal.ClearAll();
//            if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85)) pickaxe = World.Player.Layers[Layer.RightHand];
//            else pickaxe = new UOItem(p.Backpack.AllItems.FindType(0x0E85));
//            if (!pickaxe.Exist)//TODO dodelat 2. graphic krumpace
//            {
//                UO.PrintError("Nemas krumpac");
//                return;
//            }
//            baglik = World.Player.Backpack.Items.FindType(0x0E76).Serial;
//            UO.Wait(200);
//            if (baglik == 0x000) UO.PrintError("Nemas Baglik");

//            weapon = new UOItem(p.Backpack.AllItems.FindType(0x1406).Exist ? p.Backpack.AllItems.FindType(0x1406) : p.Backpack.AllItems.FindType(0x1407).Exist ? p.Backpack.AllItems.FindType(0x1407) : (Serial)0x0000);
//            last = NORTH;
//            //World.Player.Click();

//            while (!p.Dead)
//            {
//                if (p.Weight > (p.Strenght * 4 + 15))
//                {
//                    unload();
//                }
//                UO.Wait(100);
//                mineHere(true);
//                UO.Wait(100);
//                move(ref last);
//                check();
//                UO.Wait(2000);
//            }

//        }
//        [Command]
//        public void mineHere(bool useCrystal)
//        {
//            DateTime startTime;
//            bool first = true;
//            while (true)
//            {
//                if (first && useCrystal)
//                {
//                    UO.Say(".vigour");
//                }
//                check();
//                UO.WaitTargetTile(p.X, p.Y, p.Z, 0);
//                pickaxe.Use();
//                startTime = DateTime.Now;
//                UO.Wait(500);
//                if (first)
//                {
//                    Journal.WaitForText(true, 2000, "Nasla jsi lepsi material!", calls[3]);
//                    UO.Say(".vigour");
//                    first = false;
//                }
//                //Journal.WaitForText(true, 3000, calls);

//                while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(3150))
//                {
//                    if (!check()) return;
//                    UO.Wait(50);
//                }
//                if (!check()) return;
//            }


//        }
//        public void battle(ushort x, ushort y, UOCharacter enemy)
//        {
//            UO.Say(",sound");
//            if (!weapon.Exist || weapon.Serial == 0x0000)
//            {
//                UO.Print(",mbox \"NEMAS ZBRAN !!  Prisera!!!\"");
//                return;
//            }
//            enemy.Click();
//            UO.Wait(200);
//            if (enemy.Name.Contains("Spirit") || enemy.Name.Contains("Golem"))
//            {
//                UO.Say(".afk");
//                UO.Say(",terminateall");
//            }
//            weapon.Equip();
//            UO.Attack(enemy);
//            UO.Print(",mbox \" Prisera!!!    \"");
//            while (!World.Player.Dead)
//            {
//                if (!enemy.Exist) break;
//                if (enemy.Distance > 1)
//                {
//                    Pathfinding.pathfinding(enemy.X, enemy.Y);
//                }
//                UO.Wait(100);
//                if (Journal.Contains("Vysavas zivoty!"))
//                {
//                    Journal.SetLineText(Journal.Find("Vysavas zivoty!"), " ");
//                    UO.Say(".heal15");
//                    weapon.Equip();
//                    UO.Attack(enemy);
//                    Journal.WaitForText(true, 7000, " byl uspesne osetren", "Leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.");
//                }
//                if (Journal.Contains("Ziskala jsi ", "gogo"))
//                {
//                    break;

//                }
//            }
//            pickaxe.Equip();
//            Pathfinding.pathfinding(x, y);
//            UO.Wait(100);

//        }

//        public bool check()
//        {
//            bool ret = true;
//            if (!pickaxe.Exist)
//            {
//                if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85)) pickaxe = World.Player.Layers[Layer.RightHand];
//                else pickaxe = new UOItem(p.Backpack.AllItems.FindType(0x0E85));
//                UO.Wait(100);
//                if (!pickaxe.Exist) unload();
//            }
//            afk();
//            foreach (UOCharacter ch in World.Characters)
//            {
//                if (ch.Notoriety > Notoriety.Criminal && ch.Distance < 15)
//                {
//                    if (ch.Model == 0x0191 || ch.Model == 0x0190)
//                    {
//                        hideBagl();
//                        UO.Wait(2000);
//                        UO.Say(",terminateall");
//                    }
//                    battle(p.X, p.Y, ch);
//                    break;
//                }
//            }
//            if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))//lucerna - zhasla ma tuto graphicu
//                World.Player.Layers[Layer.LeftHand].Use();
//            if (Journal.Contains(calls[2])) UO.Wait(5000); //skill
//            if (Journal.Contains(calls[3], calls[4], calls[5])) ret = false; //vytezeno
//            if (Journal.Contains(calls[0], calls[1])) ret = true;// vykopane ore

//            Journal.ClearAll();
//            return ret;
//        }

//        [Command]
//        public void sound()
//        {
//            System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(alarmPath);
//            my_wave_file.PlaySync();
//        }
//        public void afk()
//        {
//            if (Journal.Contains(calls[6], calls[7], calls[8], calls[9], calls[10], calls[11]))
//            {
//                System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(alarmPath);
//                my_wave_file.PlaySync();

//            }
//        }

//        public void unload()
//        {
//            UO.WaitTargetSelf();
//            recal(true, 0x000, p.X, p.Y);
//            openBank(15);
//            UO.Wait(500);
//            UO.OpenDoor();
//            UO.OpenDoor();
//            UO.Wait(500);
//            Pathfinding.pathfinding((ushort)2731, (ushort)3270);
//            UO.Wait(200);

//            while (!Journal.Contains("You are stuffed!", "You are simply too full to eat any more!"))
//            {
//                World.Ground.FindType(0x097B).Use();
//                UO.Wait(100);
//            }
//            Journal.Clear();

//            openBank(15);
//            p.Use();
//            UO.Wait(300);
//            materialBag.Use();
//            UO.Wait(200);
//            foreach (UOItem it in p.Backpack.Items)
//            {
//                if (it.Graphic == oreType)
//                {
//                    it.Move(1000, materialBag.Serial);
//                }
//                UO.Wait(200);
//            }

//            resourcesBag.Use();
//            UO.Wait(300);
//            foreach (UOItem it in resourcesBag.Items)
//            {
//                if (it.Graphic == 0x1F4C && p.Backpack.AllItems.FindType(0x1F4C).Amount < 8)
//                    it.Move(5, p.Backpack.Serial);

//                if (it.Graphic == 0x0E85 && p.Backpack.AllItems.FindType(0x0E85).Amount < 1)
//                    it.Move(1, p.Backpack.Serial);
//                it.Click();
//                UO.Wait(300);
//            }

//            new UOItem(0x4002E317).Use();//runy
//            UO.Wait(200);
//            new UOItem(0x4003F77D).Use();// doly
//            UO.Wait(200);
//            recal(false, 0x40055F36, p.X, p.Y);
//            UO.Wait(500);
//            Pathfinding.pathfinding(2061, 718);
//            UO.Wait(500);
//            Pathfinding.pathfinding(2063, 702);
//            UO.Wait(500);
//            Pathfinding.pathfinding(2060, 682);
//        }

//        public void recal(bool home, Serial rune, ushort x, ushort y)
//        {
//            if (home)
//            {
//                while (p.X == x || p.Y == y)
//                {
//                    while (p.Mana < 20)
//                    {
//                        UO.UseSkill(StandardSkill.Meditation);
//                        UO.Wait(1500);
//                    }

//                    UO.Say(".recallhome");
//                    Journal.ClearAll();
//                    UO.Wait(500);
//                    Journal.WaitForText(true, 11000, "Kouzlo se nezdarilo.");
//                }
//            }
//            else
//            {
//                while (p.X == x || p.Y == y)
//                {
//                    while (p.Mana < 20)
//                    {
//                        UO.UseSkill(StandardSkill.Meditation);
//                        UO.Wait(1500);
//                    }
//                    new UOItem(rune).WaitTarget();//runa  skalni doly
//                    p.Backpack.AllItems.FindType(0x1F4C).Use();//recally
//                    Journal.ClearAll();
//                    UO.Wait(500);
//                    Journal.WaitForText(true, 11000, "Kouzlo se nezdarilo.");
//                }
//            }
//        }
//        public void move(ref Keys k)
//        {
//            int tmp = 0;
//            int[][] tmpRock = new int[5][];
//            for (int i = 0; i < 5; i++)
//            {
//                tmpRock[i] = new int[5];

//            }
//            int[] startPos = { p.X, p.Y };

//            while (startPos[1] == p.Y)
//            {
//                if (tmp > 10) break;
//                UO.Press(k);
//                tmp++;
//                UO.Wait(100);
//            }
//            if (tmp > 10)
//            {
//                World.FindDistance = 2;
//                foreach (UOItem it in World.Ground)
//                {
//                    if (DataFiles.Tiledata.GetArt(it.Graphic).Flags.ToString().Contains("Impassible"))
//                    {
//                        for (int x = -2; x <= 2; x++)
//                        {
//                            for (int y = -2; y <= 2; y++)
//                            {
//                                if (it.X == p.X + x)
//                                {
//                                    if (it.Y == p.Y + y) tmpRock[x + 2][y + 2] = 1;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (k == NORTH)
//                {
//                    if (tmpRock[2][1] == 1 && tmpRock[2][0] == 0)
//                        Pathfinding.pathfinding(p.X, Convert.ToUInt16(p.Y - 2));
//                    else
//                    {
//                        k = SOUTH;
//                        if (tmpRock[3][2] == 0)
//                        {
//                            Pathfinding.pathfinding(Convert.ToUInt16(startPos[0] + 1), (ushort)startPos[1]);
//                        }
//                        else
//                        {
//                            Pathfinding.pathfinding(Convert.ToUInt16(startPos[0] + 1), Convert.ToUInt16(startPos[1] + 1));
//                            return;
//                        }
//                    }
//                }
//                else
//                {


//                    k = NORTH;
//                    if (tmpRock[3][2] == 0)
//                    {
//                        Pathfinding.pathfinding(Convert.ToUInt16(startPos[0] + 1), (ushort)startPos[1]);
//                    }
//                    else
//                    {
//                        Pathfinding.pathfinding(Convert.ToUInt16(startPos[0] + 1), Convert.ToUInt16(startPos[1] - 1));
//                        return;
//                    }

//                }


//            }

//        }


//        public void openBank(int equip)
//        {
//            Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
//            Core.RegisterServerMessageCallback(0xB0, onGumpBank);
//            UO.Say(".equip{0}", equip);
//            UO.Wait(500);
//            Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
//        }



//        [Command]
//        public void hideBagl()
//        {
//            UOItem bg = new UOItem(baglik);
//            baglCoord[0] = p.X;
//            baglCoord[1] = p.Y;
//            foreach (UOItem it in p.Backpack.Items)
//            {
//                if (it.Graphic == 0x19B7 && it.Color == 0x099A)//copr
//                {
//                    //it.Move((ushort)(it.Amount * (3 / 5)), bg.Serial);
//                    continue;
//                }
//                it.Move(1000, bg.Serial);

//                UO.Wait(20);
//            }
//            UO.Wait(100);
//            bg.Drop(1, 1, 1, 50);
//            Notepad.WriteLine("Bagl je na souradnicich {0},{1},{2}   serial: {3}", p.X + 1, p.Y + 1, 50, bg.Serial);

//        }
//        [Command]
//        public void bagl()
//        {

//            Pathfinding.pathfinding(baglCoord[0], baglCoord[1]);
//            UO.Wait(200);
//            UOItem bg = new UOItem(baglik);
//            bg.Move(1, p.Backpack.Serial);
//        }
//        public CallbackResult onGumpBank(byte[] data, CallbackResult prevResult)
//        {
//            byte cmd = 0xB1; //1 byte
//            uint ID, gumpID;
//            uint buttonID = 9; //4 byte
//            uint switchCount = 0;
//            uint textCount = 0;

//            PacketReader pr = new PacketReader(data);
//            if (pr.ReadByte() != 0xB0) return CallbackResult.Normal;
//            pr.ReadInt16();
//            ID = pr.ReadUInt32();
//            gumpID = pr.ReadUInt32();


//            PacketWriter reply = new PacketWriter();
//            reply.Write(cmd);
//            reply.WriteBlockSize();
//            reply.Write(ID);
//            reply.Write(gumpID);
//            reply.Write(buttonID);
//            reply.Write(switchCount);
//            reply.Write(textCount);

//            Core.SendToServer(reply.GetBytes());
//            return CallbackResult.Sent;
//        }

//        private class Pathfinding
//        {

//            private static Keys NORTH = System.Windows.Forms.Keys.PageUp;
//            private static Keys SOUTH = System.Windows.Forms.Keys.End;
//            private static Keys WEST = System.Windows.Forms.Keys.Home;
//            private static Keys EAST = System.Windows.Forms.Keys.PageDown;

//            private static Keys UP = System.Windows.Forms.Keys.Up;
//            private static Keys DOWN = System.Windows.Forms.Keys.Down;
//            private static Keys LEFT = System.Windows.Forms.Keys.Left;
//            private static Keys RIGHT = System.Windows.Forms.Keys.Right;


//            public static void pathfinding(ushort x, ushort y)
//            {
//                int px = World.Player.X;
//                int py = World.Player.Y;

//                UO.Print("Moving to {0},{1}", x, y);

//                while (!inPosition(x, y))
//                {
//                    if (py == y)
//                    {
//                        if (px > x)
//                        {
//                            while (!inPosition(x, y) && px > x)
//                            {
//                                moveWest();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                        else
//                        {
//                            while (!inPosition(x, y) && px < x)
//                            {
//                                moveEast();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                    }
//                    else if (px == x)
//                    {
//                        if (py > y)
//                        {
//                            while (!inPosition(x, y) && py > y)
//                            {
//                                moveNorth();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                        else
//                        {
//                            while (!inPosition(x, y) && py < y)
//                            {
//                                moveSouth();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                    }
//                    else if (py > y)
//                    {
//                        if (px > x)
//                        {
//                            while (!inPosition(x, y) && px > x && py > y)
//                            {
//                                moveUp();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                        else
//                        {
//                            while (!inPosition(x, y) && px < x && py > y)
//                            {
//                                moveRight();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                    }
//                    else if (py < y)
//                    {
//                        if (px > x)
//                        {
//                            while (!inPosition(x, y) && px > x && py < y)
//                            {
//                                moveLeft();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                        else
//                        {
//                            while (!inPosition(x, y) && px < x && py < y)
//                            {
//                                moveDown();
//                                px = World.Player.X;
//                                py = World.Player.Y;
//                            }
//                        }
//                    }

//                    px = World.Player.X;
//                    py = World.Player.Y;
//                }
//                UO.Print("In position!");
//            }

//            public static bool inPosition(int x, int y)
//            {
//                int px = World.Player.X;
//                int py = World.Player.Y;
//                if (px > x + 1 || py > y + 1 || px < x - 1 || py < y - 1)
//                {
//                    return false;
//                }
//                return true;
//            }

//            private static void move(Keys direction)
//            {
//                int px = World.Player.X;
//                int py = World.Player.Y;

//                Keys dir = intToDirection(World.Player.Direction);
//                if (dir == direction)
//                {
//                    UO.Press(direction);
//                    UO.Wait(250);
//                }
//                else
//                {
//                    UO.Press(direction);
//                    UO.Press(direction);
//                    UO.Wait(250);
//                }
//                if (px == World.Player.X && py == World.Player.Y)
//                {
//                    move(changeDirection(direction));
//                }
//            }

//            private static Keys changeDirection(Keys direction)
//            {

//                if (direction == NORTH)
//                {
//                    return EAST;
//                }

//                if (direction == SOUTH)
//                {
//                    return WEST;
//                }

//                if (direction == WEST)
//                {
//                    return NORTH;
//                }

//                if (direction == EAST)
//                {
//                    return SOUTH;
//                }

//                if (direction == RIGHT)
//                {
//                    return DOWN;
//                }

//                if (direction == DOWN)
//                {
//                    return LEFT;
//                }

//                if (direction == LEFT)
//                {
//                    return UP;
//                }

//                return RIGHT;
//            }

//            private static Keys intToDirection(int dir)
//            {
//                if (dir == 0)
//                {
//                    return NORTH;
//                }
//                if (dir == 1)
//                {
//                    return RIGHT;
//                }
//                if (dir == 2)
//                {
//                    return EAST;
//                }
//                if (dir == 3)
//                {
//                    return DOWN;
//                }
//                if (dir == 4)
//                {
//                    return SOUTH;
//                }
//                if (dir == 5)
//                {
//                    return LEFT;
//                }
//                if (dir == 6)
//                {
//                    return WEST;
//                }

//                return UP;
//            }

//            /*
//                West(Home)   North(PageUp)
//                           x
//                South(End)   East(PageDown)
//            */
//            public static void moveNorth()
//            {
//                move(NORTH);
//            }

//            public static void moveSouth()
//            {
//                move(SOUTH);
//            }

//            public static void moveWest()
//            {
//                move(WEST);
//            }

//            public static void moveEast()
//            {
//                move(EAST);
//            }

//            public static void moveRight()
//            {
//                move(RIGHT);
//            }

//            public static void moveLeft()
//            {
//                move(LEFT);
//            }

//            public static void moveDown()
//            {
//                move(DOWN);
//            }

//            public static void moveUp()
//            {
//                move(UP);
//            }

//        }
//    }

//}
