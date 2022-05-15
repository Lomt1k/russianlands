using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class AttributesDialogPanel : DialogPanelBase
    {
        private int? _messageId;

        public AttributesDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(dialog.session, "unit_attribute_strength")}", 
                () => Program.logger.Debug("UP Strength"));
            RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(dialog.session, "unit_attribute_vitality")}",
                () => Program.logger.Debug("UP Vitality"));
            RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(dialog.session, "unit_attribute_sorcery")}",
                () => Program.logger.Debug("UP Sorcery"));
            RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(dialog.session, "unit_attribute_luck")}",
                () => Program.logger.Debug("UP Luck"));
            RegisterButton($"На что влияют атрибуты?", null);
        }

        private string GetText()
        {
            var profileData = dialog.session.profile.data;
            var stats = (PlayerStats)dialog.session.player.unitStats;

            var sb = new StringBuilder();
            string attributeStart = $"\n{Emojis.elements[Element.SmallWhite]} ";
            AppendAttribute("unit_attribute_strength", stats.currentStrength, profileData.attributeStrength);
            AppendAttribute("unit_attribute_vitality", stats.currentVitality, profileData.attributeVitality);
            AppendAttribute("unit_attribute_sorcery", stats.currentSorcery, profileData.attributeSorcery);
            AppendAttribute("unit_attribute_luck", stats.currentLuck, profileData.attributeLuck);

            return sb.ToString();

            // --- Helpers:
            void AppendAttribute(string localizationKey, int currentStat, int profileStat)
            {
                sb.Append($"{attributeStart}{Localization.Get(dialog.session, localizationKey)}: <b>{currentStat}</b>");
                var additiveStat = currentStat - profileStat;
                if (additiveStat > 0)
                {
                    sb.Append($"{Emojis.space}<i>(+{additiveStat})</i>");
                }
            }
        }

        public override async void OnDialogClose()
        {
            if (!_messageId.HasValue)
                return;

            await messageSender.EditMessageKeyboard(dialog.session.chatId, _messageId.Value, null);
        }

        public override async Task<Message> SendAsync()
        {
            var message = await messageSender.SendTextMessage(dialog.session.chatId, GetText(), GetKeyboardWithRowSizes(2, 2, 1));
            _messageId = message.MessageId;
            return message;
        }
    }
}
