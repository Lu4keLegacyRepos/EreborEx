using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;

namespace EreborPhoenixExtension.Libs.Skills
{
    public class Peacemaking_Enticement
    {

        private string[] musicDoneCalls = new string[] { "Uklidneni se povedlo.", "Jiz neni co uklidnovat!", "Uklidnovani nezabralo.", "Tohle nemuzes ", "You play poorly.", "Oslabeni uspesne.", "Oslabeni se nepovedlo.", " tve moznosti." };
        Settings ch;
            
        public Peacemaking_Enticement(Settings cha)
        {
            ch = cha;
        }

        public void music(bool peaceEntic)
        {
            UO.Warmode(false);
            UOCharacter target = new UOCharacter(Aliases.GetObject("laststatus"));
            if (target.Distance < 18 && !ch.musicProgress)
            {

                Core.RegisterServerMessageCallback(0x1C, onMusicDone);
                ch.musicProgress = true;
                target.WaitTarget();
                if (peaceEntic)
                {
                    UO.UseSkill(StandardSkill.Peacemaking);
                    World.Player.Print(0x099, "Uspavam " + World.GetCharacter(target).Name);
                }
                else
                {
                    UO.UseSkill(StandardSkill.Discordance_Enticement);
                    World.Player.Print(0x099, "Oslabuju " + World.GetCharacter(target).Name);
                }
            }
            else
            {
                Core.UnregisterServerMessageCallback(0x1C, onMusicDone);
                ch.musicProgress = false;
                return;
            }
            DateTime startTime = DateTime.Now;
            while (ch.musicProgress)
            {
                UO.Wait(50);
                if (DateTime.Now - startTime > TimeSpan.FromSeconds(5))
                {
                    ch.musicProgress = false;
                    break;
                }
            }
            Core.UnregisterServerMessageCallback(0x1C, onMusicDone);

        }


        CallbackResult onMusicDone(byte[] data, CallbackResult prevResult)//0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in musicDoneCalls)
            {
                if (packet.Text.Contains(s))
                {
                    ch.musicProgress = false;
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }
    }
}
