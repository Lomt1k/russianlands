using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftBuildingDialog : DialogBase
    {
        private CraftBuildingBase _building;

        public CraftBuildingDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
            _building = building;
            RegisterBackButton(() => new BuildingInfoDialog(session, building).Start());
        }

        public override async Task Start()
        {
            //TODO
            var text = "In development";
            await SendDialogMessage(text, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }
    }
}
