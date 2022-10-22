using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Quests;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.GlobalMap
{
    public class GlobalMapDialogPanel : DialogPanelBase
    {
        public GlobalMapDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowGeneralMap();
        }

        private async Task ShowGeneralMap()
        {
            ClearButtons();
            var locations = Enum.GetValues(typeof(LocationType));
            foreach (LocationType locationType in locations)
            {
                if (locationType == LocationType.None)
                    continue;

                string locationName = locationType.GetLocalization(session);
                if (locationType.IsLocked(session))
                {
                    locationName = Emojis.elements[Element.Locked] + Emojis.space + locationName;
                    RegisterButton(locationName, () => ShowLockedLocationInfo(locationType));
                    continue;
                }

                var questType = locationType.GetQuest();
                var quest = questType.HasValue ? QuestsHolder.GetQuest(questType.Value) : null;
                var hasStory = quest != null && quest.IsStarted(session) && !quest.IsCompleted(session);
                var prefix = hasStory ? Emojis.locations[MapLocation.StoryMode] : string.Empty;

                RegisterButton(prefix + Emojis.space + locationName, () => ShowLocation(locationType));
            }

            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, "dialog_map_select_location"));
            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetKeyboardWithFixedRowSize(2));
        }

        private async Task ShowLockedLocationInfo(LocationType locationType)
        {
            var sb = new StringBuilder();
            var locationName = locationType.GetLocalization(session);
            sb.AppendLine($"{Emojis.elements[Element.Locked]} <b>{locationName}</b>");

            sb.AppendLine();
            var previousLocation = (locationType - 1).GetLocalization(session);
            sb.AppendLine(string.Format(Localization.Get(session, "dialog_map_location_locked"), previousLocation));

            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_ok_button"), () => ShowGeneralMap());
            await SendPanelMessage(sb, GetOneLineKeyboard());
        }

        public async Task ShowLocation(LocationType locationType)
        {
            await new LocationMapDialog(session, locationType).Start();
        }

    }
}
