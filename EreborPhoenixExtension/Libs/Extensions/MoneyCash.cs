using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EreborPhoenixExtension.Libs.Extensions
{
     public class MoneyCash
    {

        [Command, BlockMultipleExecutions]
        public void vysyp()
        {
            UO.PrintInformation("Zamer bagl asi v bance :D");
            UOItem bagl = new UOItem(UIManager.TargetObject());
            Graphic pokladna = 0x0E80;
            UOColor goldColor = 0x0995;
            Graphic mesec = 0x0E76;
            Graphic goldy = 0x0EED;



            foreach (UOItem it in World.Player.Backpack.Items) 
            {
                if(it.Graphic==pokladna &&it.Color==goldColor)
                {
                    Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                    Core.RegisterServerMessageCallback(0xB0, onPokladnicka);
                    it.Use();
                    UO.Wait(100);
                    Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                    UO.Wait(100);

                }

                if (it.Graphic == mesec && it.Color == goldColor)
                {
                    Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                    Core.RegisterServerMessageCallback(0xB0, onPokladnicka);
                    it.Use();
                    UO.Wait(100);
                    Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                    UO.Wait(100);

                }
                foreach(UOItem itt in World.Player.Backpack.Items)
                {
                    if (itt.Graphic == goldy && itt.Color == goldColor)
                        itt.Move(65535, bagl);
                    UO.Wait(200);
                }
            }

          /*  while (World.Player.Backpack.Items.FindType(pokladna, goldColor).Exist)
            {
                UO.Wait(200);
                Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                Core.RegisterServerMessageCallback(0xB0, onPokladnicka);
                World.Player.Backpack.Items.FindType(pokladna, goldColor).Use();
                UO.Wait(100);
                Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                UO.Wait(100);
                while (World.Player.Backpack.Items.FindType(goldy).Exist)
                {
                    World.Player.Backpack.Items.FindType(goldy).Move(65535, bagl);
                    UO.Wait(200);
                }

            }
            while (World.Player.Backpack.Items.FindType(mesec, goldColor).Exist)
            {
                UO.Wait(200);
                Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                Core.RegisterServerMessageCallback(0xB0, onPokladnicka);
                World.Player.Backpack.Items.FindType(mesec, goldColor).Use();
                UO.Wait(100);
                Core.UnregisterServerMessageCallback(0xB0, onPokladnicka);
                UO.Wait(100);
                while (World.Player.Backpack.Items.FindType(goldy,goldColor).Exist)
                {
                    World.Player.Backpack.Items.FindType(goldy).Move(65535, bagl);
                    UO.Wait(200);
                }

            }*/

        }

        public CallbackResult onPokladnicka(byte[] data, CallbackResult prevResult)
        {
            byte cmd = 0xB1; //1 byte
            ushort textID = 1;// 2 byte
            ushort textLength = 3;
            uint ID, gumpID;
            uint buttonID = 2; //4 byte
            uint switchCount = 0;//1-svetla
            uint textCount = 1;
            string amount = "200000";



            PacketReader pr = new PacketReader(data);
            pr.ReadByte();
            pr.ReadInt16();
            ID = pr.ReadUInt32();
            gumpID = pr.ReadUInt32();


            PacketWriter reply = new PacketWriter();
            reply.Write(cmd);
            reply.WriteBlockSize();
            reply.Write(ID);
            reply.Write(gumpID);
            reply.Write(buttonID);
            reply.Write(switchCount);
            reply.Write(textCount);
            reply.Write(textID);
            reply.Write(textLength);

            reply.WriteUnicodeString(amount);

            Core.SendToServer(reply.GetBytes());
            return CallbackResult.Sent;
        }

    }
}
