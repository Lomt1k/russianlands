using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class TownCharacterDialogPanel : DialogPanelBase
    {
        private Message? _lastMessage;

        public TownCharacterDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowUnitView();
        }

        public async Task ShowUnitView()
        {
            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Info]} {Localization.Get(session, "dialog_character_attributes_tooltip")}", () => ShowAttributesInfo());
            var text = session.player.unitStats.GetView(session);

            _lastMessage = _lastMessage == null 
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        public async Task ShowAttributesInfo()
        {
            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_button")}", () => ShowUnitView());
            var text = Localization.Get(session, "dialog_character_attributes_info");

            _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetOneLineKeyboard());
        }

        public override void OnDialogClose()
        {
        }
    }
}
