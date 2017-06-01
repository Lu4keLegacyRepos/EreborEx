using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace EreborPhoenixExtension.Libs.Skills.Mining
{
    public class MinePoint
    {
        private Point MapStart;
        private MinePointState state;
        public DateTime TimeStamp { get; set; }
        public Point Location { get; private set; }
        public double Distance
        {
            get
            {
                return Math.Sqrt(Math.Pow((Location.X- (World.Player.X-MapStart.X)),2)+Math.Pow((Location.Y-( World.Player.Y-MapStart.Y)),2));
            }
        }

        public MinePointState State
        { get

            {
                return state;
            }
            set
            {
                if(value==MinePointState.Empty)
                {
                    TimeStamp = DateTime.Now;
                }
                state = value;
            }
        }


        public MinePoint(Point Location, MinePointState State, Point mapStart)
        {
            this.Location = Location;
            this.MapStart = mapStart;
            this.State = State;
        }
    }
}
