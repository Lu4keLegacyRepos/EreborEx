using Mining.Pathfinding;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace Mining
{
    [Serializable]
    public class MineField : IEquatable<MineField>
    {
        private bool isWalkable=false;
        private bool isExploitable = false;
        private bool isObstacle = false;
        private MineFieldState state=MineFieldState.Empty;

        public Point Location { get; set; }

        public DateTime TimeStamp { get; set; }

        public MineFieldState State
        {
            get
            {
                if (state == MineFieldState.Empty)
                {
                    if (DateTime.Now - TimeStamp > TimeSpan.FromMinutes(17))
                    {
                        state = MineFieldState.Unknown;
                        IsExploitable = true;
                    }

                }
                return state;
            }
            set
            {
                if (value == MineFieldState.Empty)
                {
                    TimeStamp = DateTime.Now;
                    IsExploitable = false;
                }
                state = value;
            }
        }

        [XmlIgnore]
        public Map Map { get; set; }

        [XmlIgnore]
        public bool IsWalkable
        {
            get
            {
                return isWalkable;
            }
            set
            {
                isWalkable = value;
            }
        }

        private char IsWalkableXML
        {
            get
            {
                return isWalkable==true?'Y':'N';
            }
            set
            {
                if (value == 'Y')
                    IsWalkable = true;
                else
                    IsWalkable = false;
            }
        }

        public bool IsExploitable
        {
            get
            {
                return isExploitable;
            }
            set
            {
                isExploitable = value;
            }
        }

        private char IsExploitableXML
        {
            get
            {
                return isExploitable == true ? 'Y' : 'N';
            }
            set
            {
                if (value == 'Y')
                    IsExploitable = true;
                else
                    IsExploitable = false;
            }
        }

        public bool IsObstacle
        {
            get
            {
                return isObstacle;
            }
            set
            {
                isObstacle = value;
            }
        }

        private char IsObstacleXML
        {
            get
            {
                return isObstacle == true ? 'Y' : 'N';
            }
            set
            {
                if (value == 'Y')
                    isObstacle = true;
                else
                    isObstacle = false;
            }
        }

        public double Distance
        {
            get
            {
                return Math.Sqrt(Math.Pow((Location.X - (World.Player.X)), 2) + Math.Pow((Location.Y - (World.Player.Y)), 2));
            }
        }


        public void MoveHere(Point StartPosition)
        {
            Movement.Movement mov = new Movement.Movement();
            foreach(Point p in GetWay(StartPosition))
            {
                mov.moveToPosition(p);
            }
        }

        private List<Point> GetWay(Point StartPosition)
        {
            SearchParameters sp = new SearchParameters(StartPosition, Location, Map);
            PathFinder pf = new PathFinder(sp);
            return pf.FindPath();
        }

        public bool Equals(MineField other)
        {
            if (other == null) return false;
            return Location.Equals(other.Location);
        }
    }
}
