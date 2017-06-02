using Phoenix;
using Phoenix.WorldData;
using System;
using System.Timers;

namespace EreborPhoenixExtension.Libs.Events
{
    public class Handler
    {
        private Timer EventsHandler;
        private bool hiddenState;
        private short hits;
        private bool poison;

        public delegate void HiddenChangeHandler(object sender, HiddenChangeArgs e);
        public event HiddenChangeHandler hiddenChange;

        public delegate void HitsChangedHandler(object sender, HitsChangedArgs e);
        public event HitsChangedHandler hitsChanged;



        public Handler()
        {
            EventsHandler = new Timer(200);
            EventsHandler.Elapsed += EventsHandler_Elapsed;
            EventsHandler.Start();

        }

        private void EventsHandler_Elapsed(object sender, ElapsedEventArgs e)
        {
                
            if (World.Player.Hidden != hiddenState)
            {

                if (hiddenChange != null)
                {
                    EventsHandler.Elapsed -= EventsHandler_Elapsed;
                    hiddenChange(this, new HiddenChangeArgs(World.Player.Hidden));
                    hiddenState = World.Player.Hidden;

                    UO.Wait(50);
                    EventsHandler.Elapsed += EventsHandler_Elapsed;
                }
            }

            if (World.Player.Hits != hits || World.Player.Poisoned != poison)
            {
                if (hitsChanged != null)
                {
                    EventsHandler.Elapsed -= EventsHandler_Elapsed;
                    hitsChanged(this, new HitsChangedArgs(World.Player.Hits < hits ? false : true, (short)Math.Abs(hits - World.Player.Hits), World.Player.Poisoned));
                    hits = World.Player.Hits;
                    poison = World.Player.Poisoned;

                    UO.Wait(50);
                    EventsHandler.Elapsed += EventsHandler_Elapsed;
                }

            }

        }
    }
}
