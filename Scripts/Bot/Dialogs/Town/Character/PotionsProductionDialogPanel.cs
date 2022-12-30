using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class PotionsProductionDialogPanel : DialogPanelBase
    {
        public PotionsProductionDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            var alchemyLab = (AlchemyLabBuilding)BuildingType.AlchemyLab.GetBuilding();
            var potionsList = alchemyLab.GetPotionsForCurrentLevel(session.profile.buildingsData);
            foreach (var potionData in potionsList)
            {
                RegisterButton(potionData.GetLocalizedName(session), () => OpenCreatePotionDialog(potionData));
            }
        }

        public override async Task SendAsync()
        {
            var text = Localization.Get(session, "dialog_potions_select_potion_to_produce");
            await SendPanelMessage(text, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        private async Task OpenCreatePotionDialog(PotionData potion)
        {
            //TODO
        }

    }
}
