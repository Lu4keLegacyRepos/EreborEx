using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace EreborPhoenixExtension.Libs
{
    public class WallCounter
    {
        public List<Wall> walls;
        Timer t;
        int[] wallTimer = { 140, 280 };//Stone,Energy
        public enum WallTime
        {
            Stone = 140,
            Energy = 290
        }
        public WallCounter()
        {
            walls = new List<Wall>();
            t = new Timer(1000);
            t.Elapsed += T_Elapsed;
            t.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (walls.Count > 0)
            {
                foreach (Wall w in walls)
                {
                    w.Count();
                }
            }
        }

        public void Add(int x, int y, WallTime type)
        {
            uint id = 1111001110;
            walls.Add(new Wall((uint)(id + walls.Count()), x, y, (int)type, this));
        }


        public class Wall
        {
            public int X { get; set; }
            public int Y { get; set; }
            public uint Serial { get; set; }
            public UOCharacter ThisWall { get; set; }
            public int Duration { get; set; }
            public DateTime createTime { get; set; }
            private WallCounter WC;
            private bool first = true;

            public int Distance
            {
                get
                {
                    return (int)Math.Abs(Math.Sqrt(Math.Pow(World.Player.X - X, 2) + Math.Pow(World.Player.Y - Y, 2)));
                }
            }


            public Wall(uint serial, int x, int y, int duration, WallCounter wc)
            {
                WC = wc;
                createTime = DateTime.Now;
                Serial = serial;
                X = x;
                Y = y;
                Duration = duration;
                ThisWall = new UOCharacter(serial);
            }
            public void Count()
            {
                int print = (int)(DateTime.Now - createTime).TotalSeconds;
                if ((Duration - print) < 11 && first)
                {
                    first = false;
                    UO.PrintError("za 10 pada zed");
                    Arrow((ushort)X, (ushort)Y, 1);
                }
                if (Distance < 18)
                {
                    if (print > Duration)
                    {
                        Arrow((ushort)X, (ushort)Y, 0);
                        WC.walls.Remove(this);
                        return;
                    }
                    MakeCounter();
                    ThisWall.Print(Duration - print);
                }
            }

            private void Arrow(ushort x, ushort y, byte OnOff)
            {
                PacketWriter pw = new PacketWriter(0xBA);
                pw.Write(OnOff);
                pw.Write(x);
                pw.Write(y);

                Core.SendToClient(pw.GetBytes());
            }
            private void MakeCounter()
            {
                PacketWriter pw = new PacketWriter(0x78);
                pw.Write((ushort)23);
                pw.Write((int)Serial);
                pw.Write((ushort)1);
                pw.Write((ushort)X);
                pw.Write((ushort)Y);
                pw.Write((byte)0);
                pw.Write((byte)1);
                pw.Write((ushort)0);
                pw.Write((byte)8);
                pw.Write((byte)7);
                pw.Write((int)0);


                Core.SendToClient(pw.GetBytes());
            }
        }
    }
}
