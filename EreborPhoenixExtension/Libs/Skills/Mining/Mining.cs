using EreborPhoenixExtension.Libs.Skills.Mining.PathFinding;
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
    public class Mining
    {
        string alarmPath = "C:\\afk.wav";
        UOItem pickaxe;
        UOPlayer p = World.Player;
        string[] calls = { "You put ", "Nevykopala jsi nic ", "Nevykopal jsi nic ", "Jeste nemuzes pouzit skill", " There is no ore", "too far", "Try mining", " is attacking ", "afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };

        [XmlIgnore]
        private PathFinder pathFinder;
        Movement mov;
        [XmlIgnore]
        private List<MinePoint> MinePlane;

        [XmlIgnore]
        private SearchParameters searchParameters;
        [XmlIgnore]
        private MapPlan mapPlan = null;
        [XmlIgnore]
        private bool RecordRun = false;
        [XmlIgnore]
        private bool[,] Map;
        [XmlArray]
        public List<MapPlan> MapPlans { get; set; }
        private List<MinePoint> EmptyPoints;


        public Mining()
        {
            MapPlans = new List<MapPlan>();
            EmptyPoints = new List<MinePoint>();
            mov = new Movement();
        }


        public void Mine()
        {
            int PointIndex=-1;
            List<int> ToDelete = new List<int>();
            Journal.ClearAll();
            if (mapPlan==null)
            {
                UO.Print("Nastav MapPlan");
                SetMapPlan();
                return;
            }
            searchParameters = new SearchParameters(mapPlan.MineEntryLocation, mapPlan.MineEntryLocation, mapPlan);
            pathFinder = new PathFinder(searchParameters);
            if (mapPlan.FindObstacles()) pathFinder.InitNodes(mapPlan.Map);
            FillMinePlane();

            if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85)) pickaxe = World.Player.Layers[Layer.RightHand];
            else pickaxe = new UOItem(p.Backpack.AllItems.FindType(0x0E85));
            if (!pickaxe.Exist)//TODO dodelat 2. graphic krumpace
            {
                UO.PrintError("Nemas krumpac");
                return;
            }
            // weapon = new UOItem(p.Backpack.AllItems.FindType(0x1406).Exist ? p.Backpack.AllItems.FindType(0x1406) : p.Backpack.AllItems.FindType(0x1407).Exist ? p.Backpack.AllItems.FindType(0x1407) : (Serial)0x0000);
            MinePoint temp = null;
            while (!World.Player.Dead)//Samotny mining
            {
                MinePlane.Sort((x, y) => (x.Distance.CompareTo(y.Distance)));//podle vzdalensoti
                foreach(MinePoint p in MinePlane)
                {
                    foreach(MinePoint pp in EmptyPoints)
                    {
                        if(DateTime.Now-pp.TimeStamp<TimeSpan.FromMinutes(30))
                        {
                            if(p.Location.X==pp.Location.X && p.Location.Y==pp.Location.Y)
                            {
                                ToDelete.Add(MinePlane.IndexOf(p));
                            }
                        }
                    }
                }
                foreach(int i in ToDelete)
                {
                    MinePlane.RemoveAt(i);
                }
                ToDelete.Clear();
                foreach(MinePoint p in MinePlane)
                {
                    if (p.State == MinePointState.UnKnown)
                    {
                        temp = p;
                        PointIndex = MinePlane.IndexOf(p);
                        break;
                    }
                }
                if (temp == null) throw new MissingFieldException();
                //temp=MinePlane.First(x => (x.State == MinePointState.UnKnown));//pridat kontrolu obnoveni poli
                MoveTo(temp.Location.X,temp.Location.Y);
                //MoveTo(MinePlane[0].Location.X, MinePlane[0].Location.Y);
                MineHere(true, PointIndex);//vytezit
               
                Check();//zkontrolovat material
                if (mapPlan.FindObstacles()) pathFinder.InitNodes(mapPlan.Map);
                FillMinePlane();

            }

        }

        private bool Check()
        {
            bool ret = true;
            if (!pickaxe.Exist)
            {
                if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85)) pickaxe = World.Player.Layers[Layer.RightHand];
                else pickaxe = new UOItem(p.Backpack.AllItems.FindType(0x0E85));
                UO.Wait(100);
                if (!pickaxe.Exist)
                {
                    MessageBox.Show("Nemas Krumpac");
                    UO.TerminateAll();
                }// unload();
            }
            if (Journal.Contains(calls[7], calls[8], calls[9], calls[10], calls[11], calls[12], calls[13]))
            {
                System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(alarmPath);
                my_wave_file.PlaySync();
            }
            foreach (UOCharacter ch in World.Characters)
            {
                if (ch.Notoriety > Notoriety.Criminal && ch.Distance < 18)
                {
                    if (ch.Model == 0x0191 || ch.Model == 0x0190)
                    {
                        // hideBagl();
                        System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(alarmPath);
                        my_wave_file.PlaySync();
                        UO.Wait(1000);
                        UO.Say(",terminateall");

                    }
                   // battle(p.X, p.Y, ch);
                    break;
                }
            }
            if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))//lucerna - zhasla ma tuto graphicu
                World.Player.Layers[Layer.LeftHand].Use();
            if (Journal.Contains(calls[3])) UO.Wait(5000); //skill
            if (Journal.Contains(calls[6], calls[4], calls[3], calls[5])) ret = false; //vytezeno
            if (Journal.Contains(calls[0], calls[1], calls[2])) ret = true;// vykopane ore

            Journal.ClearAll();
            return ret;
        }

        private void MineHere(bool CrystalUse, int PointIndex)
        {
            DateTime startTime;
            bool first = true;
            while (true)
            {
                if (first && CrystalUse)
                {
                    UO.Say(".vigour");
                }
                UO.WaitTargetTile(p.X, p.Y, p.Z, 0);
                pickaxe.Use();
                startTime = DateTime.Now;
                UO.Wait(500);
                if (first)
                {
                    Journal.WaitForText(true, 2000, "Nasla jsi lepsi material!", calls[3]);
                    UO.Say(".vigour");
                    first = false;
                }
                //Journal.WaitForText(true, 3000, calls);

                while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(3200))
                {
                    if (!Check())
                    {

                        MinePlane[PointIndex].State = MinePointState.Empty;
                        EmptyPoints.Add(MinePlane[PointIndex]);
                        UO.Wait(1000);
                        return;
                    }
                    UO.Wait(100);
                }
                if (!Check())
                {

                    MinePlane[PointIndex].State = MinePointState.Empty;
                    EmptyPoints.Add(MinePlane[PointIndex]);
                    UO.Wait(1000);

                    return;
                }
            }
        }

        private void FillMinePlane()
        {
            if (MinePlane == null) MinePlane = new List<MinePoint>();
            List<Point> tmp = pathFinder.GetAllWalkablePoints();
            MinePlane.Clear();
            foreach (Point p in tmp)
            {

                switch (mapPlan.MineEntry)
                {
                    case 'N':
                        if (p.Y < mapPlan.MineEntryY)
                        {

                            MinePlane.Add(new MinePoint(p, MinePointState.UnKnown, mapPlan.MapStart));

                        }
                        break;

                    case 'W':
                        {

                            MinePlane.Add(new MinePoint(p, MinePointState.UnKnown, mapPlan.MapStart));

                        }
                        break;

                    case 'S':
                        {

                            MinePlane.Add(new MinePoint(p, MinePointState.UnKnown, mapPlan.MapStart));
                        }
                        break;

                    case 'E':
                        {

                            MinePlane.Add(new MinePoint(p, MinePointState.UnKnown, mapPlan.MapStart));
                        }
                        break;

                }


            }



        }

        public void MoveTo(int x, int y)
        {
            if (mapPlan == null)
            {
                UO.PrintError("Neni nastaven MapPlan");// setmapplan 1 pro D 2 pro F...
                return;
            }

          //  searchParameters = new SearchParameters(new Point(World.Player.X, World.Player.Y), new Point(x, y), mapPlan);
            searchParameters.StartLocation = new Point(World.Player.X, World.Player.Y);
            searchParameters.EndLocation = new Point(x, y);
            pathFinder = new PathFinder(searchParameters);
            List<Point> output = pathFinder.FindPath();
            foreach(Point p in output)
            {
                ;
                mov.moveToPosition(new Point(p.X + mapPlan.MapStart.X, p.Y + mapPlan.MapStart.Y));
            }

        }



        public void SetMapPlan()
        {
            UO.PrintError("Nastav Map Plan pomoci:");
            UO.PrintInformation(",setmapplan X");
            for(int i=0;i< MapPlans.Count();i++)
            {
                UO.Print(i + " - " + MapPlans[i].Name);
            }
        }
        public void SetMapPlan(int MapPlan)
        {
            if (MapPlan >= MapPlans.Count || MapPlan < 0) { SetMapPlan(); return; }
            mapPlan = MapPlans[MapPlan];
        }

        public void RecordMap(int StartX, int StartY, string Name)//(X-2600, Y-3200)) GH
        {
            UO.PrintInformation("Projdi vsechna pole, po kterych lze chodit/kopat");
            UO.PrintWarning("Pro ukonceni napis STOP");
            UO.PrintWarning("Pro zaznamenani Vstupu do dolu napis ENTRY N-S-E-W");
            UO.PrintInformation("N-S-E-W podle toho jakym smerem se jde do dolu");
            UO.PrintInformation("tak aby se nesnazilo pod tento bod kopat - na pristupove ceste");

            int X = World.Player.X;
            int Y = World.Player.Y;
            RecordRun = true;
            List<Point> Record = new List<Point>();
            Journal.ClearAll();
            mapPlan = new MapPlan();
            while (RecordRun)
            {
                if (X != World.Player.X || Y != World.Player.Y)
                {
                    X = World.Player.X;
                    Y = World.Player.Y;
                    if (!Record.Contains(new Point(X, Y)))
                        if(X>500 || Y>500)
                        Record.Add(new Point(X - StartX, Y - StartY));
                    else Record.Add(new Point(X,Y));
                }
                if (Journal.Contains("ENTRY N"))
                {
                    Journal.Clear();
                    mapPlan.MineEntry = 'N';
                    mapPlan.MineEntryLocation = new Point(X,Y);
                }
                if (Journal.Contains("ENTRY S"))
                {
                    Journal.Clear();
                    mapPlan.MineEntry = 'S';
                    mapPlan.MineEntryLocation = new Point(X, Y);
                }
                if (Journal.Contains("ENTRY E"))
                {
                    Journal.Clear();
                    mapPlan.MineEntry = 'E';
                    mapPlan.MineEntryLocation = new Point(X, Y);
                }
                if (Journal.Contains("ENTRY W"))
                {
                    Journal.Clear();
                    mapPlan.MineEntry = 'W';
                    mapPlan.MineEntryLocation = new Point(X, Y);
                }
                if (Journal.Contains("STOP"))
                    RecordRun = false;
                UO.Wait(100);
            }
            Map = new bool[500, 500];
            for (int y = 0; y < 500; y++)//GH
                for (int x = 0; x < 500; x++)
                    Map[x, y] = false;
            foreach (Point p in Record)
            {
                this.Map[p.X, p.Y] =true;
            }
            Record.Clear();
            //mapPlan = new MapPlan() { Map = Map, MapStart = new Point(StartX, StartY), Name=Name };
            mapPlan.Map = Map;
            mapPlan.MapStart = new Point(StartX, StartY);
            mapPlan.Name = Name;
            MapPlans.Add(mapPlan);
            UO.Print("Recording done");



        }




    }
}
