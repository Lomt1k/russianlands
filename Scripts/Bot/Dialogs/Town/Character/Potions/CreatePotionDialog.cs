using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.Bot.Dialogs.Resources;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public class CreatePotionDialog : DialogBase
    {
        private PotionData _data;

        public CreatePotionDialog(GameSession session, PotionData data) : base(session)
        {
            _data = data;

            var countLimit = Math.Min(GetFreeSlotsCount(), 5); 
            for (int i = 1; i <= countLimit; i++)
            {
                var amountForDelegate = i; //it is important!
                RegisterButton(i.ToString(), () => TryCraft(amountForDelegate));
            }
            RegisterBackButton(() => new PotionsProductionDialog(session).Start());
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>" + _data.GetName(session) + "</b>");

            sb.AppendLine();
            sb.AppendLine(_data.GetDescription(session, session));

            sb.AppendLine();            
            var requiredResources = GetCraftCost();
            sb.Append(ResourceHelper.GetPriceView(session, requiredResources));
            var dtNow = DateTime.UtcNow;
            var timeSpan = (dtNow.AddSeconds(GetCraftTimeInSeconds()) - dtNow);
            sb.AppendLine(timeSpan.GetView(session, withCaption: true));

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_ours"));
            var ourResources = new Dictionary<ResourceType, int>()
            {
                {ResourceType.Herbs, session.player.resources.GetValue(ResourceType.Herbs)}
            };
            sb.Append(ResourceHelper.GetResourcesView(session, ourResources));

            sb.AppendLine();
            sb.Append(Localization.Get(session, "dialog_potions_select_potions_amount"));

            await SendDialogMessage(sb, GetSpecialKeyboard())
                .ConfigureAwait(false);
        }

        private int GetFreeSlotsCount()
        {
            return session.player.potions.GetFreeSlotsCount(session);
        }

        private Dictionary<ResourceType, int> GetCraftCost()
        {
            var alchemyLab = (AlchemyLabBuilding)BuildingType.AlchemyLab.GetBuilding();
            return alchemyLab.GetCurrentCraftCost(session.profile.buildingsData);
        }

        private int GetCraftTimeInSeconds()
        {
            var alchemyLab = (AlchemyLabBuilding)BuildingType.AlchemyLab.GetBuilding();
            return alchemyLab.GetCurrentCraftTimeInSeconds(session.profile.buildingsData);
        }

        private ReplyKeyboardMarkup GetSpecialKeyboard()
        {
            return GetKeyboardWithRowSizes(buttonsCount - 1, 1);
        }

        private async Task TryCraft(int amount)
        {
            var requiredResources = GetCraftCost();
            if (amount > 1)
            {
                requiredResources[ResourceType.Herbs] *= amount;
            }

            var playerResources = session.player.resources;
            var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
            if (successfullPurchase)
            {
                StartCraft(amount);
                await new PotionsDialog(session).Start()
                    .ConfigureAwait(false);
                return;
            }

            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
                onSuccess: async () => await new CreatePotionDialog(session, _data).TryCraft(amount).ConfigureAwait(false),
                onCancel: async () => await new CreatePotionDialog(session, _data).Start().ConfigureAwait(false));
            await buyResourcesDialog.Start().ConfigureAwait(false);
        }

        private void StartCraft(int amount)
        {
            var craftEndTime = DateTime.UtcNow.AddSeconds(GetCraftTimeInSeconds()).Ticks;
            for (int i = 0; i < amount; i++)
            {
                var potionItem = new PotionItem(_data.id, craftEndTime);
                session.player.potions.Add(potionItem);
            }
        }

    }
}
