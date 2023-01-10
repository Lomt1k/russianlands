using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftInProgressDialog : DialogBase
    {
        public CraftInProgressDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
        }

        public override Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
