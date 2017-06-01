using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace EreborPhoenixExtension.Libs.Skills.Mining.PathFinding
{
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
    {
        private Point startLocation;
        private Point endLocation;
        public Point StartLocation
        {
            get { return startLocation; }
            set
            {
                if (value.X > 500 || value.Y > 500)
                    startLocation = new Point(value.X - MapStart.X, value.Y - MapStart.Y);
                else startLocation = value;
            }
        }

        public Point EndLocation
        {
            get { return endLocation; }
            set
            {
                if (value.X > 500 || value.Y > 500)
                    endLocation = new Point(value.X - MapStart.X, value.Y - MapStart.Y);
                else endLocation = value;
            }
        }

        public Point MapStart { get; set; }


        public bool[,] Map { get; set; }

        public SearchParameters(Point startLocation, Point endLocation, MapPlan mapPlan)
        {

            this.Map = mapPlan.Map;
            MapStart = mapPlan.MapStart;
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;

        }
    }
}
