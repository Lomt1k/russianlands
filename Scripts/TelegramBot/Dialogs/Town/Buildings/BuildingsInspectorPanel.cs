using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings
{
    public class BuildingsInspectorPanel : DialogPanelBase
    {
        public BuildingsInspectorPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowNotifications();
        }

        public async Task ShowNotifications()
        {
            ClearButtons();
            var sb = new StringBuilder();
            var playerBuildings = session.player.buildings;
            if (playerBuildings.HasImportantUpdates())
            {
                var allBuildings = playerBuildings.GetAllBuildings();
                foreach (var building in allBuildings)
                {
                    var updates = building.GetUpdates(session, session.profile.buildingsData);
                    if (updates.Count < 1)
                        continue;

                    var header = $"<pre>{building.GetLocalizedName(session, session.profile.buildingsData)}:</pre>";
                    sb.AppendLine(header);

                    foreach (var update in updates)
                    {
                        sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} {update}");
                    }
                    sb.AppendLine();
                }
            }

            // TODO: Инфа о добытых ресурсах
            sb.AppendLine("<pre>Добытые ресурсы:</pre>");
            sb.AppendLine("[Инфа о добытых ресурсах]");

            RegisterButton(Localization.Get(session, "dialog_buildings_get_resources"), null);

            lastMessage = lastMessage == null 
                ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, sb.ToString(), GetMultilineKeyboard());
        }

        public async Task ShowBuildingsList(BuildingCategory category, bool asNewMessage)
        {
            await RemoveKeyboardFromLastMessage();
            var text = $"<b>{category.GetLocalization(session)}</b>";
            var data = session.profile.buildingsData;
            var buildings = session.player.buildings.GetBuildingsByCategory(category);
            var sortedBuildings = buildings.OrderBy(x => x.IsBuilt(data)).ThenBy(x => x.buildingData.levels[0].requiredTownHall);

            foreach (var building in sortedBuildings)
            {
                RegisterButton(building.GetLocalizedName(session, data), null);
            }
            RegisterButton($"{Emojis.menuItems[MenuItem.Buildings]} {Localization.Get(session, "menu_item_buildings")}",
                () => ShowNotifications());

            lastMessage = asNewMessage || lastMessage == null
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, GetMultilineKeyboard());
        }

    }
}
