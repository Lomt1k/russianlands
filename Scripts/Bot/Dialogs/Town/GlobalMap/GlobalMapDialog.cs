using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap
{
    public class GlobalMapDialog : DialogBase
    {
        private GlobalMapDialogPanel _mapPanel;

        public GlobalMapDialog(GameSession _session) : base(_session)
        {
            _mapPanel = new GlobalMapDialogPanel(this, 0);
            RegisterPanel(_mapPanel);

            RegisterButton($"{Emojis.locations[MapLocation.Arena]} " + Localization.Get(session, "menu_item_arena"),
                () => messageSender.SendTextMessage(session.chatId, "Арена недоступна в текущей версии игры")); // заглушка
            RegisterTownButton(isFullBack: false);
        }

        public override async Task Start()
        {
            var header = $"{Emojis.menuItems[MenuItem.Map]} <b>{Localization.Get(session, "menu_item_map")}</b>";
            await SendDialogMessage(header, GetMultilineKeyboard())
                .ConfigureAwait(false);
            await SendPanelsAsync()
                .ConfigureAwait(false);
        }

    }
}
