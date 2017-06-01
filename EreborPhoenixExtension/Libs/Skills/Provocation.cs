using Phoenix;
using Phoenix.WorldData;

namespace EreborPhoenixExtension.Libs.Skills
{
    public class Provocation
    {
        private UOCharacter cil1;
        private UOCharacter cil2;

        [Command, BlockMultipleExecutions]
        public void setProvo()
        {
            UO.Print("Zamer 1 k provokovani");
            cil1 = new UOCharacter(UIManager.TargetObject());

            UO.Print("Zamer 2 k provokovani");
            cil2 = new UOCharacter(UIManager.TargetObject());
        }

        [Command("provo")]
        public void provokace()
        {
            if (cil1.Distance < 18 && cil2.Distance < 18)
            {
                cil1.WaitTarget();
                UO.Say(".provo");
                cil2.WaitTarget();
            }
            else UO.Print("Nejsou v dosahu");
        }
    }
}
