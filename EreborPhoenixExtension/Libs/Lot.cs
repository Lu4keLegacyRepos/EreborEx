

using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace EreborPhoenixExtension.Libs
{
    public delegate void AutoLot();
    [Serializable]
    public class Lot
    {

        private List<Graphic> toCarv = new List<Graphic> { 0x2006, 0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6, 0x2006 };//pvuciny+mrtvola
        private readonly List<string> jezdidla = new List<string> { "body of mustang", "body of zostrich", "body of oclock", "body of orn", "oody of ledni medved", "body of ridgeback", "body of ridgeback savage" };
        private bool running = false;
        private bool jidlo = false;
        private bool regy = false;
        private bool drahokamy = false;
        private bool peri = false;
        private bool kuze = false;
        private bool sipky = false;
        private bool extend1 = false;
        private bool extend2 = false;
        private bool doLot = false;
        private bool hideCorpses = false;
        private Graphic extend1_type, extend2_type = 0;
        private List<ushort> lotitems;
        private ushort[] food = new ushort[] { 0x0978, 0x097A, 0x097B, 0x097E, 0x098C, 0x0994, 0x09B7, 0x09B9, 0x09C0, 0x09C9, 0x09D0, 0x09D1, 0x09D2, 0x09E9, 0x09EA, 0x09EC, 0x09F2, 0x0C5C, 0x0C64, 0x0C66, 0x0C6A, 0x0C6D, 0x0C70, 0x0C72, 0x0C74, 0x0C77, 0x0C79, 0x0C7B, 0x0C7F, 0x0D39, 0x103B, 0x1040, 0x1041, 0x1608, 0x1609, 0x160A, 0x171F, 0x1726, 0x1727, 0x1728, 0x172A };
        [XmlIgnore]
        private UOItem lotBag;
        [XmlIgnore]
        private UOItem carvTool;

        public uint LotBag
        {
            get { return lotBag==null?0:(uint)lotBag; }
            set
            {
                lotBag = new UOItem(value);
            }
        }
        public uint CarvTool
        {
            get { return carvTool == null ? 0 : (uint)carvTool; }
            set
            {
                carvTool = new UOItem(value);
            }
        }
        public bool Food
        {
            get
            {
                return jidlo ;
            }
            set
            {
                try
                {

                    jidlo = value;
                    if (jidlo)
                    {
                        lotitems.AddRange(food);
                    }
                    else lotitems.RemoveAll(item => food.Any(f => f == item));
                }
                catch { }
            }
        }

        public bool Reageants
        {
            get
            {
                return regy;
            }
            set
            {
                try
                {

                    regy = value;
                    if (regy)
                    {
                        for (ushort i = 0x0F78; i < 0x0F92; i++)
                        {
                            lotitems.Add(i);
                        }
                    }
                    else
                    {
                        for (ushort i = 0x0F78; i < 0x0F92; i++)
                        {
                            lotitems.Remove(i);
                        }
                    }
                }
                catch { }
            }
        }
        public bool Gems
        {
            get
            {
                return drahokamy;
            }
            set
            {
                try
                {

                    drahokamy = value;
                    if (drahokamy)
                    {
                        for (ushort i = 0x0F0F; i < 0x0F31; i++)
                        {
                            lotitems.Add(i);
                        }
                    }
                    else
                    {
                        for (ushort i = 0x0F0F; i < 0x0F31; i++)
                        {
                            lotitems.Remove(i);
                        }
                    }
                }
                catch { }
            }
        }

        public bool Feathers
        {
            get
            {
                return peri;
            }

            set
            {
                try
                {

                    peri = value;
                    if (peri)
                    {
                        lotitems.Add(0x1BD1);
                    }
                    else lotitems.RemoveAt(lotitems.IndexOf(0x1BD1));
                }
                catch { }
            }
        }

        public bool Leather
        {
            get
            {
                return kuze;
            }

            set
            {
                try
                {
                    kuze = value;
                    if (kuze)
                    {
                        lotitems.Add(0x1078);
                    }
                    else lotitems.RemoveAt(lotitems.IndexOf(0x1078));
                }
                catch { }
            }
        }

        public bool Bolts
        {
            get
            {
                return sipky;
            }

            set
            {
                try
                {
                    sipky = value;
                    if (sipky)
                    {
                        lotitems.Add(0x1BFB);
                        lotitems.Add(0x0F3F);
                    }
                    else
                    {
                        lotitems.RemoveAt(lotitems.IndexOf(0x1BFB));
                        lotitems.RemoveAt(lotitems.IndexOf(0x0F3F));
                    }
                }
                catch { }
            }
        }
        public ushort Extend1_type
        {
            get
            {
                return extend1_type;
            }

            set
            {
                extend1_type = (Graphic)value;
            }
        }

        public ushort Extend2_type
        {
            get
            {
                return extend2_type;
            }

            set
            {
                extend2_type = (Graphic)value;
            }
        }

        public bool Extend1
        {
            get
            {
                return extend1;
            }

            set
            {
                try
                {
                    if (Extend1_type == 0) return;
                    extend1 = value;
                    if (extend1)
                    {
                        lotitems.Add(Extend1_type);
                    }
                    else lotitems.RemoveAt(lotitems.IndexOf(Extend1_type));
                }
                catch { }
            }
        }

        public bool Extend2
        {
            get
            {
                return extend2;
            }

            set
            {
                try
                {
                    if (Extend2_type == 0) return;
                    extend2 = value;
                    if (sipky)
                    {
                        lotitems.Add(Extend2_type);
                    }
                    else lotitems.RemoveAt(lotitems.IndexOf(Extend2_type));
                }
                catch { }
            }
        }

        public bool Running
        {
            get
            {
                return running;
            }

            set
            {
                running = value;
            }
        }

        public bool DoLot
        {
            get
            {
                return doLot;
            }

            set
            {
                doLot = value;
            }
        }

        public bool HideCorpses
        {
            get
            {
                return hideCorpses;
            }

            set
            {
                hideCorpses = value;
            }
        }



        public Lot()
        {

            lotitems = new List<ushort>();
            lotitems.Add(0x0eed);
            lotitems.Add(0x14EB);
            lotitems.Add(0x0E76);
            AutoLot alot = new AutoLot(AutoLot);
            new Thread(AutoLot).Start();
        }

        private void AutoLot()
        {
            if (Running) return;
            Running = true;
            while (true)
            {
                World.FindDistance = 7;
                foreach (UOItem it in World.Ground.Where(x => x.Distance < 5 && x.Items.CountItems() < 9 && x.Graphic == 0x2006).ToList())
                {
                    UO.Wait(100);
                    if (HideCorpses) UO.Hide(it);
                    if (!DoLot) continue;
                    foreach (UOItem i in it.Items.Where(item => lotitems.Any(li => item.Graphic == li)).ToList())//it.AllItems)
                    {
                        UO.MoveItem(i, ushort.MaxValue, lotBag == null ? World.Player.Backpack : lotBag);
                        UO.Wait(300);
                    }
                }
                UO.Wait(200);
            }
        }
        public void Carving()
        {
            World.FindDistance = 4;
            foreach (UOItem it in World.Ground.Where(x => x.Distance < 4 && toCarv.Any(p => x.Graphic == p)))//x.Graphic == 0x2006))// && jezdidla.All(y => y != x.Name.ToLower()) ))//!jezdidla.Contains(x.Name)).ToList())
            {
                it.Click();
                UO.Wait(100);
                if (jezdidla.Contains(it.Name.ToLower())) continue;

                it.WaitTarget();
                carvTool.Use();
                UO.Wait(300);
            }

        }
    }

}

