using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class TownCharacterDialogPanel : DialogPanelBase
    {
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

            lastMessage = lastMessage == null 
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        public async Task ShowAttributesInfo()
        {
            ClearButtons();
            RegisterBackButton(() => ShowUnitView());
            var text = Localization.Get(session, "dialog_character_attributes_info");

            lastMessage = await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, GetOneLineKeyboard());
        }

    }
}
