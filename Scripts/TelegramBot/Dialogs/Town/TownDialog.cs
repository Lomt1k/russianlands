using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town
{
    public enum TownEntryReason
    {
        StartNewSession,
        BackFromInnerDialog,
        FromQuestAction
    }

    public class TownDialog : DialogBase
    {
        private TownEntryReason _reason;
        private ReplyKeyboardMarkup _keyboard;

        public TownDialog(GameSession _session, TownEntryReason reason) : base(_session)
        {
            _reason = reason;

            RegisterButton($"{Emojis.menuItems[MenuItem.Map]} " + Localization.Get(session, "menu_item_map"),
                () => new GlobalMap.GlobalMapDialog(session).Start());

            var buildingsLocalization = $"{Emojis.menuItems[MenuItem.Buildings]} " + Localization.Get(session, "menu_item_buildings")
                + (session.player.buildings.HasImportantUpdates() ? $"{Emojis.elements[Element.WarningRed]}" : string.Empty);
            RegisterButton(buildingsLocalization, () => new Buildings.BuildingsDialog(session).Start());

            RegisterButton($"{Emojis.characters[CharIcon.Male]} " + Localization.Get(session, "menu_item_character"),
                () => new Character.TownCharacterDialog(session).Start());
            RegisterButton($"{Emojis.menuItems[MenuItem.Quests]} " + Localization.Get(session, "menu_item_quests"),
                () => messageSender.SendTextMessage(session.chatId, "Задания недоступны в текущей версии игры")); //заглушка
            RegisterButton($"{Emojis.menuItems[MenuItem.Mail]} " + Localization.Get(session, "menu_item_mail"),
                () => messageSender.SendTextMessage(session.chatId, "Почта недоступна в текущей версии игры")); //заглушка
            RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} " + Localization.Get(session, "menu_item_shop"),
                () => new Shop.ShopDialog(session).Start());

            _keyboard = GetKeyboardWithRowSizes(1, 2, 3);
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Town]} <b>" + Localization.Get(session, "menu_item_town") + "</b>");
            sb.AppendLine();

            if (_reason == TownEntryReason.StartNewSession)
            {
                sb.AppendLine(Localization.Get(session, "dialog_town_text_newSession"));
                sb.AppendLine();
            }
            string resources = session.player.resources.GetGeneralResourcesView();
            sb.AppendLine(resources);

            bool withTooltip = TryAppendTooltip(sb);
            if (!withTooltip)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_town_text_backFromInnerDialog"));
            }

            await SendDialogMessage(sb, _keyboard);
        }

    }
}
