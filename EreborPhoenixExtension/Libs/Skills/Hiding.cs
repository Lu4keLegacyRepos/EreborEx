using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using System;

namespace EreborPhoenixExtension.Libs.Skills
{
    public class Hiding
    {
        Settings ch;
        public Hiding(Settings cha)
        {
            ch = cha;
        }
        public bool getHit { get; set; }

        public void hiding(bool first)
        {
            Core.UnregisterClientMessageCallback(0x02, makeStep);
            getHit = false;
            int step = 2;
            UO.Warmode(false);
            UO.UseSkill(StandardSkill.Hiding);
            World.Player.Print("3");
            DateTime startHid = DateTime.Now;
            while (DateTime.Now - startHid < TimeSpan.FromMilliseconds(ch.hidDelay))
            {
                if (getHit)
                {
                    Core.UnregisterClientMessageCallback(0x02, makeStep);
                    if (first) hiding(false);
                    return;
                }
                if (DateTime.Now - startHid > TimeSpan.FromMilliseconds(ch.hidDelay / 3) && step == 2)
                {
                    World.Player.Print("2");
                    step--;
                }
                if (DateTime.Now - startHid > TimeSpan.FromMilliseconds((ch.hidDelay / 3) * 2) && step == 1)
                {
                    World.Player.Print("1");
                    step--;

                    Core.RegisterClientMessageCallback(0x02, makeStep);
                }
                UO.Wait(10);
            }
            if (Journal.WaitForText(true, 500, "Nepovedlo se ti schovat.", "Skryti se povedlo."))
            {
                if (!World.Player.Hidden)
                    Core.UnregisterClientMessageCallback(0x02, makeStep);
            }
            else Core.UnregisterClientMessageCallback(0x02, makeStep);
        }
        public void hidoff()
        {
            Core.UnregisterClientMessageCallback(0x02, makeStep);
        }
        public CallbackResult makeStep(byte[] data, CallbackResult prev)//0x02 clientReq
        {
            PacketReader pr = new PacketReader(data);
            PacketWriter pw = new PacketWriter();
            byte cmd = pr.ReadByte();
            byte dir = pr.ReadByte();
            byte seq = pr.ReadByte();
            uint fwalkPrev = pr.ReadUInt32();
            if (Convert.ToUInt16(dir) > 7)
            {
                dir = Convert.ToByte(Convert.ToUInt16(dir) - 128);
                pw.Write(cmd);
                pw.Write(dir);
                pw.Write(seq);
                pw.Write(fwalkPrev);
                Core.SendToServer(pw.GetBytes());
                return CallbackResult.Sent;
            }

            return CallbackResult.Normal;
        }
    }
}
