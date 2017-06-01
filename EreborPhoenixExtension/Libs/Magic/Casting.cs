using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EreborPhoenixExtension.Libs.Magic
{
    public class Casting
    {
        public Dictionary<string, int> SpellsDelays = new Dictionary<string, int> { { "Fireball", 2060 }, { "Flame", 4100 }, { "Meteor", 6750 }, { "Lightning", 3550 }, { "Bolt", 3100 }, {"frostbolt",3000 }, { "Harm", 1500 }, { "Mind", 3450 }, { "Invis", 3300 }/*-150*/ };
        public string lastSpell;
        public Serial lastTarget;
        private bool acast = false;
        public uint ManaLimit { get; set; }
        public bool autoSacrafire { get; set; }
        public bool SpellFizz { get; set; }


        public void ccast(string spellname, Serial target, Action bandage, Action Sacrafire)
        {
            if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
            if (autoSacrafire && World.Player.Mana < World.Player.MaxMana - Main.Instance.Settings.VoodooManaLimit) Sacrafire();
            if (spellname == "necrobolt" || spellname == "frostbolt")
            {
                lastTarget = target;
                UO.WaitTargetObject(lastTarget);
                UO.Say("." + spellname);
                lastSpell = spellname;

            }
            else
            {
                lastSpell = spellname;
                lastTarget = target;
                UO.Cast(spellname, target);
            }
        }


        public void ccast(string spellname, Action bandage,Action Sacrafire)
        {
            if (World.Player.Hits < World.Player.MaxHits - 7) bandage();
            if (autoSacrafire && World.Player.Mana < World.Player.MaxMana - ManaLimit) Sacrafire();
            if (spellname == "necrobolt" || spellname == "frostbolt")
            {
                UO.Say("." + spellname);
                lastTarget = UIManager.TargetObject();
                lastSpell = spellname;


            }
            else
            {
                lastSpell = spellname;
                lastTarget = UIManager.TargetObject();
                UO.Cast(spellname, lastTarget);
            }
        }

        public void autocast(bool charged,Action bandage,Action Sacrafire)
        {
            DateTime start;
            bool first = true;
            if (!acast)
            {
                acast = true;
                lastTarget = Aliases.GetObject("laststatus");
                if (charged)
                {
                    UO.Print("Autocast charged ON");
                    if (new[] { "Fireball", "Flame", "Meteor" }.Contains(lastSpell))
                    {
                        while (acast)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (!acast)
                                {
                                    UO.Print("Autocast OFF");
                                    return;
                                }
                                start = DateTime.Now;
                                UO.Cast("Fireball", lastTarget);
                                while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                                {
                                    UO.Wait(50);
                                }

                            }
                            start = DateTime.Now;
                            UO.Cast(lastSpell, lastTarget);
                            while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                            {
                                UO.Wait(50);
                            }
                        }

                    }
                    else if (new[] { "Lightning", "Bolt" }.Contains(lastSpell))
                    {
                        while (acast)
                        {
                            for (int i = 0; i < (first ? 3 : 1); i++)
                            {
                                if (!acast)
                                {
                                    UO.Print("Autocast OFF");
                                    return;
                                }
                                start = DateTime.Now;
                                UO.Cast("Lightning", lastTarget);
                                while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                                {
                                    UO.Wait(50);
                                }

                            }
                            for (int i = 0; i < 5; i++)
                            {
                                if (!acast)
                                {
                                    UO.Print("Autocast OFF");
                                    return;
                                }
                                start = DateTime.Now;
                                UO.Cast("Bolt", lastTarget);
                                while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                                {
                                    UO.Wait(50);
                                }
                            }
                            first = false;
                        }
                    }
                    else if (new[] { "Harm", "Mind" }.Contains(lastSpell))
                    {
                        while (acast)
                        {
                            for (int i = 0; i < (first ? 3 : 1); i++)
                            {
                                if (!acast)
                                {
                                    UO.Print("Autocast OFF");
                                    return;
                                }
                                start = DateTime.Now;
                                UO.Cast("Harm", lastTarget);
                                while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                                {
                                    UO.Wait(50);
                                }

                            }
                            for (int i = 0; i < 5; i++)
                            {
                                if (!acast)
                                {
                                    UO.Print("Autocast OFF");
                                    return;
                                }
                                start = DateTime.Now;
                                UO.Cast("Mind", lastTarget);
                                while ((DateTime.Now - start) < TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                                {
                                    UO.Wait(50);
                                }
                            }
                            first = false;
                        }

                    }
                }
                else
                {
                    UO.Print("Autocast ON");
                    while (acast)
                    {
                        if (autoSacrafire && World.Player.Mana < World.Player.MaxMana - ManaLimit) Sacrafire();
                        start = DateTime.Now;
                        ccast(lastSpell, lastTarget,bandage,Sacrafire);
                        while((DateTime.Now-start)<TimeSpan.FromMilliseconds(SpellsDelays[lastSpell]))
                        {
                            UO.Wait(50);
                        }
                        UO.Wait(50);
                    }
                }
            }
            else
            {
                acast = false;
                UO.Print("Autocast OFF");
            }


        }



    }
}
