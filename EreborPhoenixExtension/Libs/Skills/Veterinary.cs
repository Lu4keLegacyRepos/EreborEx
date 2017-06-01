using EreborPhoenixExtension.Libs.Weapons;
using Phoenix;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;

namespace EreborPhoenixExtension.Libs.Skills
{
    public class Veterinary
    {
        UOCharacter pet;
        bool healed = false;
        bool onoff = false;
        bool harmed = false;
        private WeaponsHandle ch;

        public Veterinary(WeaponsHandle cha)
        {
            pet = null;
            ch = cha;
        }

        public void autoVet()
        {
            if (!onoff)
            {
                onoff = true;
                UO.PrintWarning("Bandim do full");
            }
            else
            {
                onoff = false;
                UO.PrintWarning("Bandeni vypnuto");
            }
            while (onoff)
            {
                if (!Vet())
                {
                    UO.PrintError("Vypinám");
                    onoff = false;
                    return;
                }

            }
        }
        public bool Vet()
        {
            if (pet == null || pet.Distance > 15 )
            {
                UO.Print("Zamer mezlicka");
                pet = new UOCharacter(UIManager.TargetObject());
            }
            if (pet == null) return false;
            if (pet.Distance > 6)
            {
                UO.PrintError("Moc daleko");
                return false;
            }
            if(UIManager.CurrentState!=UIManager.State.Ready)
            {
                UO.Wait(200);
                return true;
            }
            Core.UnregisterServerMessageCallback(0x1C, onHeal);
            Core.RegisterServerMessageCallback(0x1C, onHeal);
            //Osetreni se ti nepovedlo.
            //byl uspesne osetren.
            // neni zranen.

            pet.WaitTarget();
            UO.Say(".bandage");
            healed = false;
            harmed = true;
            DateTime start = DateTime.Now;
            ch.ActualWeapon.Equip();
            while (!healed)
            {
                UO.Wait(100);
                if (DateTime.Now - start > TimeSpan.FromSeconds(6)) break;
                if (!harmed)
                {
                    UO.PrintInformation("Neni zranen");
                    Core.UnregisterServerMessageCallback(0x1C, onHeal);
                    return false;
                }
            }
            Core.UnregisterServerMessageCallback(0x1C, onHeal);
            return true;
        }

        CallbackResult onHeal(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains(" byl uspesne osetren.") || packet.Text.Contains("Osetreni se ti nepovedlo."))
            {
                healed = true;
            }
            if (packet.Text.Contains(" neni zranen."))
            {
                healed = true;
                harmed = false;
            }
            return CallbackResult.Normal;
        }
    }
}
