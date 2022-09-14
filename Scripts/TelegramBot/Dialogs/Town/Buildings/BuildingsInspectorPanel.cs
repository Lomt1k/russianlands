using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings
{
    public class BuildingsInspectorPanel : DialogPanelBase
    {
        private ProfileBuildingsData buildingsData => session.profile.buildingsData;

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
            var buildings = session.player.buildings.GetBuildingsByCategory(category);
            var sortedBuildings = buildings.OrderBy(x => x.IsBuilt(buildingsData)).ThenBy(x => x.buildingData.levels[0].requiredTownHall);

            foreach (var building in sortedBuildings)
            {
                var name = GetPrefix(building, buildingsData) + building.GetLocalizedName(session, buildingsData);
                RegisterButton(name, () => ShowBuildingInfo(building));
            }
            RegisterButton($"{Emojis.menuItems[MenuItem.Buildings]} {Localization.Get(session, "menu_item_buildings")}",
                () => ShowNotifications());

            lastMessage = asNewMessage || lastMessage == null
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private string GetPrefix(BuildingBase building, ProfileBuildingsData data)
        {
            if (building.IsUnderConstruction(data))
                return Emojis.elements[Element.Construction] + Emojis.space;

            if (building.IsBuilt(data))
                return string.Empty;

            return building.IsNextLevelUnlocked(data)
                ? Emojis.elements[Element.Plus] + Emojis.space
                : Emojis.elements[Element.Locked] + Emojis.space;
        }

        private async Task ShowBuildingInfo(BuildingBase building)
        {
            if (building.IsUnderConstruction(buildingsData))
            {
                await ShowConstructionProgressInfo(building);
                return;
            }
            if (!building.IsBuilt(buildingsData))
            {
                await ShowConstructionAvailableInfo(building);
                return;
            }

            await ShowBuildingCurrentLevelInfo(building);
        }

        private async Task ShowBuildingCurrentLevelInfo(BuildingBase building)
        {
            if (lastMessage == null)
                return;

            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{building.GetLocalizedName(session, buildingsData)}</b>");
            sb.AppendLine();
            sb.AppendLine(building.GetCurrentLevelInfo(session, buildingsData));

            var updates = building.GetUpdates(session, buildingsData);
            if (updates.Count > 0)
            {
                sb.AppendLine();
                foreach (var update in updates)
                {
                    sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} {update}");
                }
            }            

            if (!building.IsMaxLevel(buildingsData))
            {
                RegisterButton($"{Emojis.elements[Element.LevelUp]} {Localization.Get(session, "building_construction_button")}",
                    () => ShowConstructionAvailableInfo(building));
            }

            lastMessage = await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, sb.ToString());
        }

        private async Task ShowConstructionAvailableInfo(BuildingBase building)
        {

        }

        private async Task ShowConstructionProgressInfo(BuildingBase building)
        {
            if (lastMessage == null)
                return;

            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{building.GetLocalizedName(session, buildingsData)}</b>");
            sb.AppendLine();

            var updates = building.GetUpdates(session, buildingsData);
            foreach (var update in updates)
            {
                sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} {update}");
            }

            // Постройка здания могла быть завершена в момент выполнения этого кода
            bool isUnderConstruction = building.IsUnderConstruction(buildingsData);
            // TODO: Register buttons

            lastMessage = await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, sb.ToString());
        }

    }
}
