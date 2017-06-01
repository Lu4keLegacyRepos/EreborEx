using Mining.Pathfinding;
using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mining
{
    public class Map : IEnumerable
    {
        public List<MineField> Fields { get; set; }
        public string Name { get; set; }
        private readonly List<Graphic> Obsatcles = new List<Graphic>() { 0x1363, 0x1364, 0x1365, 0x1366, 0x1367, 0x1368, 0x1369, 0x136A, 0x136B, 0x136C, 0x136D };

        public Map()
        {
            Fields = new List<MineField>();
        }
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Fields).GetEnumerator();
        }

        public void MoveTo(int X,int Y)
        {
            Movement.Movement mov = new Movement.Movement();
            foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), new Point(X,Y)))
            {
                mov.moveToPosition(p);
            }
        }

        private List<Point> GetWay(Point StartPosition, Point EndPosition)
        {
            SearchParameters sp = new SearchParameters(StartPosition, EndPosition, this);
            PathFinder pf = new PathFinder(sp);
            return pf.FindPath();
        }

        public MineField MoveToClosestExploitable()
        {
            FindObstacles();
            Fields.Sort((a, b) =>a.Distance.CompareTo(b.Distance));
            MineField tmp;
            try
            {
                tmp = Fields.First(x => x.IsExploitable);
                tmp.MoveHere(new System.Drawing.Point(World.Player.X, World.Player.Y));
            }
            catch
            {
                tmp = null;
                UO.PrintError("Nenalezen tezitelbne pole");
            }
            return tmp;
        }

        public void FindObstacles()
        {
            World.FindDistance = 15;
            foreach(UOItem it in World.Ground.Where(x=>Obsatcles.Any(a=>x.Graphic.Equals(a))))
            {
                Fields.Find(x => x.Location.X == it.X && x.Location.Y == it.Y).IsObstacle = true;
            }
        }

        public void RemoveNearObstacles(Action<UOItem> RemoveObstacle)
        {
            World.FindDistance = 3;
            foreach (UOItem it in World.Ground.Where(x => Obsatcles.Any(a => x.Graphic.Equals(a))))
            {
                RemoveObstacle(it);
            }
        }

        public static Map Record(string Name)
        {
            UO.PrintInformation("Projdi vsechna pole, po kterych lze chodit/kopat");
            UO.PrintWarning("STOP pro ukonceni");
            UO.PrintWarning("WALK pro prochazeni NEtezebnich poli");
            UO.PrintWarning("MINE pro prochazeni tezebnich poli");
            bool RecordState = false;
            int X = World.Player.X;
            int Y = World.Player.Y;
            var RecordRun = true;
            var map = new Map() { Name = Name };

            Journal.ClearAll();
            while (RecordRun)
            {
                if (X != World.Player.X || Y != World.Player.Y)
                {
                    X = World.Player.X;
                    Y = World.Player.Y;
                }
                if (Journal.Contains("WALK"))
                {
                    UO.PrintInformation("Nasledujici pole budou oznaceny jen pro CHUZI");
                    Journal.Clear();
                    RecordState = false;

                }
                if (Journal.Contains("MINE"))
                {
                    UO.PrintInformation("Nasledujici pole budou oznaceny pro TEZBU");
                    Journal.Clear();
                    RecordState = true; 
                }
                if (Journal.Contains("STOP"))
                    RecordRun = false;
                if (map.Fields.Contains(new MineField() { Location = new System.Drawing.Point(X, Y) })) continue;
                if(RecordState)//mine
                {
                    map.Fields.Add(new MineField()
                    {
                        IsExploitable = true,
                        IsObstacle = false,
                        IsWalkable = true,
                        Location = new System.Drawing.Point(X, Y),
                        Map = map,
                        State = MineFieldState.Unknown
                    });
                }
                else// only walk
                {
                    map.Fields.Add(new MineField()
                    {
                        IsExploitable = false,
                        IsObstacle = false,
                        IsWalkable = true,
                        Location = new System.Drawing.Point(X, Y),
                        Map = map,
                        State = MineFieldState.Unknown
                    });
                }
                UO.Wait(100);
            }


            UO.PrintInformation(".Record Done");
            return map;
        }
    }
}
