using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mining
{
    [Serializable]
    public class Mine
    {
        private string[] calls = { "You put ", "Nevykopala jsi nic ", "Jeste nemuzes pouzit skill",                  // 0-1, 2
                                   " There is no ore", "too far", "Try mining",                                      // 3-5
                                   " is attacking ", "afk", "AFK", "kontrola", "GM", "gm", "Je spatne videt." };     // 6, 7-11, 12

        const string AlarmPath = "C:\\afk.wav";
        private Graphic Ore = 0x19B7;
        private Dictionary<string, UOColor> Material = new Dictionary<string, UOColor>() { { "Copper", 0x099A }, {"Iron", 0x0763 }, {"Kremicity", 0x0481 }, {"Verite", 0x097F } };
        // TODO materialy, pocitadlo ore u sebe ukladani
        private DateTime StartMine=DateTime.Now;

        private List<Map> maps;

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
                return pickAxe==null?0:(uint)pickAxe;
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
                return mace == null ? 0 : (uint)mace;
            }

            set
            {
                mace = new UOItem(value);
            }
        }

        public List<Map> Maps
        {
            get
            {
                if(maps==null)
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

        private int MapIndex = -1;
        private bool crystalEnabled = true;
        private bool autoStockUp = false;
        private bool dropMaterial = true;
        private bool dropCopper = true;
        private bool dropIron = false;
        private bool dropSilicon = false;
        private bool dropVerite = false;
        private bool removeObstacles = false;
        private UOItem pickAxe;
        private UOItem mace;


        public Mine()
        {

        }

        public void RemoveObstacle(UOItem it)
        {
            // TODO odkopani
        }

        /// <summary>
        /// Check AFK, Attack, Weight etc..
        /// </summary>
        /// <returns>True - MineField is empty</returns>
        public bool Check()
        {
            bool rtrnTmp = false;
            // Check AFK
            if(Journal.Contains(true, calls[7], calls[8], calls[9], calls[10], calls[11]))
            {
                System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
                my_wave_file.PlaySync();
            }
            // Check Attack
            if (Journal.Contains(true, calls[6]))
            {
                Battle b = new Battle();
                b.Kill();
            }
            // Check Light
            if (Journal.Contains(true, calls[12]) || World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A18))
            {
                World.Player.Layers[Layer.LeftHand].Use();
                UO.Wait(200);
                if (World.Player.Layers[Layer.LeftHand].Graphic.Equals(0x0A15)) World.Player.Backpack.AllItems.FindType(0x0A18).Use();

            }
            // No Ore
            if (Journal.Contains(true, calls[3], calls[4], calls[5]))
                rtrnTmp= true;
            // Skill delay
            if (Journal.Contains(true, calls[2]))
            {
                UO.Wait(5000);
            }

            // Check Weight
            if(World.Player.Weight > (World.Player.Strenght * 4 + 15))
            {
                Unload();
            }

            // Incoming Ore
            if (Journal.Contains(true, calls[0], calls[1]))
            {
                if(DropMaterial)
                {
                    if (DropCopper && Journal.Contains(true, "Copper "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Copper"]).DropHere(ushort.MaxValue);
                        rtrnTmp= true;
                    }
                    if (DropIron && Journal.Contains(true, "Iron "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Iron"]).DropHere(ushort.MaxValue);
                        rtrnTmp= true;
                    }
                    if (DropSilicon && Journal.Contains(true, "Kremicity "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Kremicity"]).DropHere(ushort.MaxValue);
                        rtrnTmp =true;
                    }
                    if (DropVerite && Journal.Contains(true, "Verite "))
                    {
                        World.Player.Backpack.AllItems.FindType(Ore, Material["Verite"]).DropHere(ushort.MaxValue);
                        rtrnTmp= true;
                    }
                }

            }
            Journal.Clear();
            return rtrnTmp;
        }

        public void Work()
        {
            Map map = Maps[MapIndex];
            Mace = World.Player.Backpack.AllItems.FindType(0x1406).Exist
                ? World.Player.Backpack.AllItems.FindType(0x1406).Serial
                : World.Player.Backpack.AllItems.FindType(0x1407).Exist
                ? World.Player.Backpack.AllItems.FindType(0x1407).Serial : 0;

            if (World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E85) | World.Player.Layers[Layer.RightHand].Graphic.Equals(0x0E86))
                PickAxe = World.Player.Layers[Layer.RightHand];

            else PickAxe = World.Player.Backpack.AllItems.FindType(0x0E85).Exist
                    ?World.Player.Backpack.AllItems.FindType(0x0E85).Serial : World.Player.Backpack.AllItems.FindType(0x0E86).Exist 
                    ? World.Player.Backpack.AllItems.FindType(0x0E86).Serial : 0;

            while (true)
            {
                if (MapIndex < 0)
                {
                    UO.Print("Neni vybrána mapa!!");
                    SelectMap();
                    if(MapIndex<0)
                    {
                        throw new InvalidProgramException("Neni vybrana mapa");
                    }
                }

                MineHere(map.MoveToClosestExploitable(),0);
                UO.Wait(200);
                if(RemoveObstacles)
                {
                    map.RemoveNearObstacles(RemoveObstacle);
                }
            }

        }

        public void Unload()
        {
            Point ActualPosition = new Point(World.Player.X, World.Player.Y);
            Recall(0);
            StockUp();
            //Recall(1) -1== dul ve kterem kopem? podle nastavene mapy?
        }

        private void Recall(int v)
        {
            UO.Say(".recallhome");
            /*
            0-home  pridat doly ?
            */
        }

        public void StockUp()
        {
            // TODO vykladani
        }

        public void MineHere(MineField mf,int Try)
        {
            if(mf==null)
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
            
            if(CrystalEnabled && Try==0)
            {
                UO.Say(".vigour");
                UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
                (pickAxe).Use();
                StartMine = DateTime.Now;
                Journal.WaitForText(true, 2000, "Nasla jsi lepsi material!","Nasel jsi lepsi material!");
                UO.Say(".vigour");
                MineHere(mf, Try + 1);
            }
            else
            {
                UO.WaitTargetTile(World.Player.X, World.Player.Y, World.Player.Z, 0);
                (pickAxe).Use();
                StartMine = DateTime.Now;
                UO.Wait(300);
                MineHere(mf, Try+1);
            }
        }

        // TODO : SelectMap
        public void SelectMap()
        {
            MapIndex = 0;
        }
    }
}
