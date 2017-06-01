using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EreborPhoenixExtension.Libs.Skills.Mining
{
    [Serializable]
    public class MapPlan
    {
        [XmlIgnore]
        private bool[,] map;
        [XmlIgnore]
        private List<string> xmlMap;
        private Point mapStart;
        private Point mineEntryLocation;
        public char MineEntry { get; set; }
        public int MineEntryX
        {
            get
            {
                return mineEntryLocation.X-MapStart.X;
            }
            set
            {
                mineEntryLocation.X = value;
            }
        }

        public int MineEntryY
        {
            get
            {
                return mineEntryLocation.Y-MapStart.Y;
            }
            set
            {
                mineEntryLocation.Y = value;
            }
        }

        public string Name { get; set; }


        public List<string> XMLMap {
            get {

                StringBuilder sb= new StringBuilder();
                xmlMap = new List<string>();
                if (map == null) return xmlMap;
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    sb = new StringBuilder();
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        bool s = map[i, j];
                        //xmlMap.Add(s == true ? 1 : 0);
                        sb.Append(s == true ? 'Y' : 'N');

                    }
                    xmlMap.Add(sb.ToString());
                }
                return xmlMap;
            }
        set
            {
                xmlMap = value;
            }

        } 

        [XmlIgnore]
        public bool[,] Map { get
            {
                if (map == null)
                {
                    int rowStep = 500;
                    map = new bool[rowStep, rowStep];
                    //int i = 0;
                    for (int x=0;x<xmlMap.Count();x++)
                    {
                        // map[(i / 500), i % 500] = bOOl==1;

                        //i++;
                        char[] tmp = xmlMap[x].ToCharArray();
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            map[x, i] = tmp[i] == 'Y' ? true : false;
                        }
                    }
                }
                return map;

            }
            set { map = value; } }
        public int X
        {
            get { return mapStart.X; }
            set
            {
                mapStart.X = value;
            }
        }
        public int Y
        {
            get { return mapStart.Y; }
            set
            {
                mapStart.Y = value;
            }
        }

        [XmlIgnore]
        public Point MapStart { get { return mapStart; }set { mapStart = value; } }

        [XmlIgnore]
        public Point MineEntryLocation { get { return mineEntryLocation; }set
            {
                mineEntryLocation = value;
            } }


        public bool FindObstacles()
        {
            List<Graphic> Obstacles = new List<Graphic>() {0x00,0x1 };//TODO
            World.FindDistance = 10;
            List<UOItem> tmp = World.Ground.Where(x => Obstacles.Any(y => y == x.Graphic)).ToList();
            if (tmp.Count < 1) return false;
            foreach (UOItem it in tmp)
            {
                map[it.X - X, it.Y - Y] = false;
            }
            return true;
        }
    }
}
