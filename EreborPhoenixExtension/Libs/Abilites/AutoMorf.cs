using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;

namespace EreborPhoenixExtension.Libs.Abilites
{

    public class AutoMorf
    {

        private bool amorf;
        public bool Amorf
        {
            get { return amorf; }
            set
            {
                if (value)
                {
                    Core.RegisterServerMessageCallback(0x77, automorf);
                    UO.Print("Morf ON");
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x77, automorf);
                    UO.Print("Morf OFF");
                }
                amorf = value;
            }
        }




        public void MorfTo(ushort model)
        {
            UOCharacter toMorf;

            toMorf = new UOCharacter(UIManager.TargetObject());

            UO.Print(toMorf.Model);
            PacketWriter pw = new PacketWriter(0x77);
            pw.Write((int)toMorf.Serial);
            pw.Write((ushort)model);
            pw.Write((ushort)toMorf.X);
            pw.Write((ushort)toMorf.Y);
            pw.Write((byte)toMorf.Z);
            pw.Write((byte)toMorf.Direction);
            pw.Write((ushort)toMorf.Color + 10);
            pw.Write((byte)toMorf.Flags);
            pw.Write((byte)toMorf.Notoriety);

            Core.SendToClient(pw.GetBytes());
        }


        CallbackResult automorf(byte[] data, CallbackResult prev)
        {

            PacketReader reader = new PacketReader(data);
            reader.Skip(5);
            ushort model = reader.ReadUInt16();
            if ((model == 0x000A) || (model == 0x0009)) model = 39;//demon-summ
            if ((model == 0x00AD)) model = 11;//elder spider
            ByteConverter.BigEndian.ToBytes((ushort)model, data, 5);
            return CallbackResult.Normal;

        }



    }
}
