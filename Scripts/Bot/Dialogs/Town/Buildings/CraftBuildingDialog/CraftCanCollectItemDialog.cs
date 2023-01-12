using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftCanCollectItemDialog : DialogBase
    {
        private CraftBuildingBase _building;

        private ProfileBuildingsData buildingsData => session.profile.buildingsData;

        public CraftCanCollectItemDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
            _building = building;
        }

        public override async Task Start()
        {
            //TODO

            RegisterBackButton(() => new BuildingInfoDialog(session, _building).Start());
            await SendDialogMessage("Can Collect Item!", GetMultilineKeyboard())
                .ConfigureAwait(false);
        }
    }
}
