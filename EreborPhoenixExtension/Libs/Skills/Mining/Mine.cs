using EreborPhoenixExtension.Libs.Extensions.Pathfinding;
using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EreborPhoenixExtension.Libs.Skills.Mining
{
    [Serializable]
    public class Mine
    {
        #region Fields & Property

        private string[] calls = { "You put ", "Nevykopala jsi nic ", "Jeste nemuzes pouzit skill",                  // 0-1, 2
                                   " There is no ore", "too far", "Try mining",                                      // 3-5
                                   " is attacking ", "afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };     // 6, 7-11, 12


        private string path = Core.Directory + @"\Profiles\XML\";
        
        private string AlarmPath =Core.Directory+@"\afk.wav";// "C:\\afk.wav";
        private Graphic Ore = 0x19B7;
        private Dictionary<string, UOColor> Material = new Dictionary<string, UOColor>() { { "Copper", 0x099A }, { "Iron", 0x0763 }, { "Kremicity", 0x0481 }, { "Verite", 0x097F }, { "Valorite", 0x0985 }, { "Obsidian", 0x09BD }, { "Adamantium", 0x19B7 } };
        private int[] MaterialsCount = { 0, 0, 0, 0, 0, 0, 0 };
        private DateTime StartMine = DateTime.Now;
        public int MapIndex = -1;
        private bool crystalEnabled = true;
        private bool autoStockUp = false;
        private bool dropMaterial = true;
        private bool dropCopper = true;
        private bool dropIron = true;
        private bool dropSilicon = false;
        private bool dropVerite = false;
        private bool removeObstacles = true;
        private UOItem pickAxe;
        private UOItem mace;
        private int maxObsidian = 5;
        private int maxAdamantium = 2;
        private List<Map> maps;
        private Movement mov;
        private PathFinder pathFinder;
        private SearchParameters searchParams;

        public bool DropCopper
        {
            get
            {
                return dropCopper;
            }

            set
            {
                dropCopper = value;
            }
        }

        public bool CrystalEnabled
        {
            get
            {
                return crystalEnabled;
            }

            set
            {
                crystalEnabled = value;
            }
        }

        public bool AutoStockUp
        {
            get
            {
                return autoStockUp;
            }

            set
            {
                autoStockUp = value;
            }
        }

        public bool DropMaterial
        {
            get
            {
                return dropMaterial;
            }

            set
            {
                dropMaterial = value;
            }
        }

        public bool DropIron
        {
            get
            {
                return dropIron;
            }

            set
            {
                dropIron = value;
            }
        }

        public bool DropSilicon
        {
            get
            {
                return dropSilicon;
            }

            set
            {
                dropSilicon = value;
            }
        }

        public bool DropVerite
        {
            get
            {
                return dropVerite;
            }

            set
            {
                dropVerite = value;
            }
        }

        public uint PickAxe
        {
            get
            {
                uint tmp;
                if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85) | World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E86))
                    tmp = World.Player.Layers[Layer.RightHand];

                else tmp = World.Player.Backpack.AllItems.FindType(0x0E85).Exist
                        ? World.Player.Backpack.AllItems.FindType(0x0E85).Serial : World.Player.Backpack.AllItems.FindType(0x0E86).Exist
                        ? World.Player.Backpack.AllItems.FindType(0x0E86).Serial : 0;
                if (tmp != 0)
                    pickAxe = new UOItem(tmp);
                return tmp;
            }

            set
            {
                pickAxe = new UOItem(value);
            }
        }

        public uint Mace
        {
            get
            {
                uint tmp = World.Player.Backpack.AllItems.FindType(0x1406).Exist
                ? World.Player.Backpack.AllItems.FindType(0x1406).Serial
                : World.Player.Backpack.AllItems.FindType(0x1407).Exist
                ? World.Player.Backpack.AllItems.FindType(0x1407).Serial : 0;
                if (tmp != 0)
                    mace = new UOItem(tmp);
                return tmp;
            }

            set
            {
                mace = new UOItem(value);
            }
        }

        public bool RemoveObstacles
        {
            get
            {
                return removeObstacles;
            }

            set
            {
                removeObstacles = value;
            }
        }

        public int MaxObsidian
        {
            get
            {
                return maxObsidian;
            }

            set
            {
                maxObsidian = value;
            }
        }

        public int MaxAdamantium
        {
            get
            {
                return maxAdamantium;
            }

            set
            {
                maxAdamantium = value;
            }
        }
        #endregion
        private int ActualMapIndex;

        public Mine()
        {
            mov = new Movement();
            searchParams = new SearchParameters(new Point(), new Point(), null);
            // Defaultne Skalni
            ActualMapIndex = 0;
            Notepad.WriteLine(AlarmPath);
        }



        [XmlArray]
        public List<Map> Maps
        {
            get
            {
                if (maps == null)
                {
                    Maps = new List<Map>();
                }
                return maps;
            }

            set
            {
                maps = value;
            }
        }

        public uint doorLeft { get;  set; }
        public uint doorRight { get;  set; }

        public void AddMap(string Name)
        {
            var tmp = new Map();
            tmp.Record(Name, Maps.Count());
            Maps.Add(tmp);
        }


        public void RemoveObstacle(UOItem it)
        {
            while(!Journal.Contains(true, "Odstranila jsi zaval!", "Odstranil jsi zaval!"))
            {
                Journal.Clear();
                it.WaitTarget();
                pickAxe.Use();
                Journal.WaitForText(true, 4500, "Nepovedlo se ti odstranit zaval."); //TODO pauzy jako kopani predelat!

                UO.Wait(500);
            }
        }

        /// <summary>
        /// Check AFK, Attack, Weight etc..
        /// </summary>
        /// <returns>True - MineField is empty</returns>
        public bool Check()
        {
            bool rtrnTmp = false;
            // Check AFK
            if (Journal.Contains(true, calls[7], calls[8], calls[9], calls[10], calls[11]))
            {
                System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
                my_wave_file.PlaySync();
            }
            // Check CK/Monster
            foreach (UOCharacter ch in World.Characters)
            {
                if (ch.Notoriety > Notoriety.Criminal)
                {
                    System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
                    my_wave_file.Play();
                    Battle b = new Battle(MoveTo,MoveToFarestField,ch,mace);
                    b.Kill();
                }
            }

            // Check Light
            if (Journal.Contains(true, calls[12]) || World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))
            {
                World.Player.Layers[Layer.LeftHand].Use();
                UO.Wait(200);
                if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A15)) World.Player.Backpack.AllItems.FindType(0x0A18).Use();

            }
            // Count materials
            for (int o = 0; o < Material.Count; o++)
            {
                int tmp;
                switch (o)
                {
                    case 0:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Copper"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;
                    case 1:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Iron"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;
                    case 2:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Kremicity"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;
                    case 3:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Verite"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;
                    case 4:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Valorite"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;
                    case 5:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Obsidian"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;
                    case 6:
                        tmp = World.Player.Backpack.Items.FindType(Ore, Material["Adamantium"]).Amount;
                        MaterialsCount[o] = tmp > 0 ? tmp : 0;
                        break;

                }

            }

            // No Ore
            if (Journal.Contains(true, calls[3], calls[4], calls[5]))
                rtrnTmp = true;

            // Skill delay
            if (Journal.Contains(true, calls[2]))
            {
                UO.Wait(5000);
            }

            // Check Weight
            if (World.Player.Weight > (World.Player.Strenght * 4 + 15))
            {
                Unload();
            }

            // Incoming Ore
            if (Journal.Contains(true, calls[0], calls[1]))
            {
                if (DropMaterial)
                {
                    if (DropCopper && Journal.Contains(true, "Copper "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).DropHere(ushort.MaxValue);
                        rtrnTmp = true;
                    }
                    if (DropIron && Journal.Contains(true, "Iron "))
                    {
                       // World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).DropHere(ushort.MaxValue);
                        rtrnTmp = true;
                    }
                    if (DropSilicon && Journal.Contains(true, "Kremicity "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Kremicity"]).DropHere(ushort.MaxValue);
                        rtrnTmp = true;
                    }
                    if (DropVerite && Journal.Contains(true, "Verite "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Verite"]).DropHere(ushort.MaxValue);
                        rtrnTmp = true;
                    }
                }

            }
            Journal.Clear();
            if(rtrnTmp)
            {
                // Check amount of Best materials
                if (MaterialsCount[5] >= MaxObsidian | MaterialsCount[6] >= MaxAdamantium)
                {
                    Unload();
                }
            }
            return rtrnTmp;
        }

        public void Work()
        {


            while (true)
            {// TODO
               /* if (Maps.Count < 1)
                {
                    UO.Print("Nejsou nahrány mapy!  KONEC");
                    new InvalidProgramException("Neni vybrana mapa");
                    
                }*/
                Mace = Mace;
                PickAxe = PickAxe;
                MineHere(MoveToClosestExploitable(), 0);
                UO.Wait(200);
                if (RemoveObstacles)
                {
                    while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3200)) UO.Wait(100);
                    Maps[MapIndex].RemoveNearObstacles(RemoveObstacle);
                }
            }

        }

        #region Move

        private MineField MoveToClosestExploitable()
        {
            Maps[ActualMapIndex].FindObstacles();
            Maps[ActualMapIndex].Fields.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            MineField tmp;
            try
            {
                tmp = Maps[ActualMapIndex].Fields.First(x => x.IsExploitable);
                MoveTo(tmp.Location);
            }
            catch
            {
                tmp = null;
                UO.PrintError("Nenalezen tezitelbne pole");
            }
            return tmp;
        }

        public void MoveTo(int X, int Y)
        {
            foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), new Point(X, Y)))
            {
                mov.moveToPosition(p);
            }
        }
        public void MoveTo(Point location)
        {
            foreach (Point p in GetWay(new Point(World.Player.X, World.Player.Y), location))
            {
                mov.moveToPosition(p);
            }
        }

        private List<Point> GetWay(Point StartPosition, Point EndPosition)
        {
            searchParams = new SearchParameters(StartPosition, EndPosition, Maps[ActualMapIndex]);
            pathFinder = new PathFinder(searchParams);
            return pathFinder.FindPath();
        }

        public void MoveToFarestField()
        {
            Maps[ActualMapIndex].FindObstacles();
            Maps[ActualMapIndex].Fields.Sort((b, a) => a.Distance.CompareTo(b.Distance));
            MineField tmp;
            try
            {
                tmp = Maps[ActualMapIndex].Fields.First(x => x.IsExploitable);
                MoveTo(tmp.Location);
            }
            catch
            {
                tmp = null;
                UO.PrintError("Nenalezen tezitelbne pole");
            }
        }
        #endregion 
        public void Unload()
        {
            Point ActualPosition = new Point(World.Player.X, World.Player.Y);
            int tmpMapIndex = ActualMapIndex;
            Recall(0);
            StockUp();
            //Recall(1) -1== dul ve kterem kopem? podle nastavene mapy?
        }

        private void Recall(int v)
        {

            int x = World.Player.X;
            int y = World.Player.Y;
            while (World.Player.X == x || World.Player.Y == y)
            {
                while (World.Player.Mana < 20)
                {
                    UO.UseSkill(StandardSkill.Meditation);
                    UO.Wait(1500);
                }

                UO.Say(".recallhome");
                Journal.ClearAll();
                UO.Wait(500);
                Journal.WaitForText(true, 11000, "Kouzlo se nezdarilo.");
            }
            StockUp();
        }

        public void StockUp()
        {
            ActualMapIndex = 0;
            //new UOItem(doorLeft).Use(); // dvere
           // new UOItem(doorRight).Use();
        }

        public void MineHere(MineField mf, int Try)
        {
            if (mf == null)
            {
                return;
            }

            while (DateTime.Now - StartMine < TimeSpan.FromMilliseconds(3200))
            {
                UO.Wait(300);
                if (Check())
                {
                    mf.State = MineFieldState.Empty;
                    return;
                }
            }

            if (CrystalEnabled && Try == 0)
            {
                UO.Say(".vigour");
                UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
                (pickAxe).Use();
                StartMine = DateTime.Now;
                Journal.WaitForText(true, 1500, "Nasla jsi lepsi material!", "Nasel jsi lepsi material!");
                UO.Say(".vigour");
                MineHere(mf, Try + 1);
            }
            else
            {
                UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
                (pickAxe).Use();
                StartMine = DateTime.Now;
                UO.Wait(300);
                MineHere(mf, Try + 1);
            }
        }

        // TODO : SelectMap
        public void SelectMap()
        {
            foreach (Map m in Maps)
            {
                UO.Print(m.Index + " - " + m.Name);
            }
            MapIndex = 0;
        }



    }
}
