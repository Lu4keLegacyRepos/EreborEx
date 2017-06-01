using Phoenix;
using Phoenix.Communication.Packets;
using System;
using System.Timers;

namespace EreborPhoenixExtension.Libs
{
    [RuntimeObject]
    public class WallInfo
    {
        private class WallHandler
        {
            public WallHandler(string Name, int delay)
            {
                DateTime ReqTime = DateTime.Now;
                Timer w;
                w = new Timer(delay);
                w.Elapsed += (sender, e) => W_Elapsed(sender, e, ReqTime, Name, delay, w);

                w.Start();

            }
            private void W_Elapsed(object sender, ElapsedEventArgs e, DateTime startTime, string name, int delay, Timer w)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (DateTime.Now - startTime >= TimeSpan.FromMilliseconds(delay * i)
                        && DateTime.Now - startTime < TimeSpan.FromMilliseconds(delay * (i + 1)))
                    {
                        UO.PrintInformation("{0} zed za {1}", name, 8 - i);
                        return;
                    }
                }
                if (DateTime.Now - startTime > TimeSpan.FromSeconds(7))
                {
                    w.Close();
                    w.Stop();
                    w.Dispose();
                }

                throw new NotImplementedException();
            }
        }

        public WallInfo()
        {
            Core.RegisterServerMessageCallback(0x1C, onWallSpeech);
        }

        ~WallInfo()
        {
            Core.UnregisterServerMessageCallback(0x1C, onWallSpeech);
        }

        CallbackResult onWallSpeech(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains("Anna Tir Kemen"))
            {
                WallHandler w = new WallHandler(packet.Name, 850);
            }
            if (packet.Text.Contains("Anna Tir Esgal"))
            {
                WallHandler w = new WallHandler(packet.Name, 700);
            }

            return CallbackResult.Normal;
        }
    }
}
