using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class AttributesDialogPanel : DialogPanelBase
    {
        private int? _messageId;
        private bool _isInfoOpened;
        private bool _isClosing;
        private InlineKeyboardMarkup _lastKeyboard;

        public AttributesDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {            
        }

        private async Task ShowAttributes()
        {
            _isInfoOpened = false;
            ClearButtons();

            if (!_isClosing && session.profile.data.attributePoints > 0)
            {
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_strength")}",
                () => TryUpAttribute(ref session.profile.data.attributeStrength));
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_vitality")}",
                    () => TryUpAttribute(ref session.profile.data.attributeVitality));
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_sorcery")}",
                    () => TryUpAttribute(ref session.profile.data.attributeSorcery));
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_luck")}",
                    () => TryUpAttribute(ref session.profile.data.attributeLuck));
            }
            RegisterButton($"{Emojis.elements[Element.Info]} {Localization.Get(session, "dialog_attributes_tooltip")}",
                () => Task.Run(ShowInfo));

            var keyboard = buttonsCount > 1 ? GetKeyboardWithRowSizes(2, 2, 1) : GetOneLineKeyboard();
            if (_messageId.HasValue)
            {
                await messageSender.EditTextMessage(session.chatId, _messageId.Value, GetText(), keyboard);
            }
            else
            {
                var message = await messageSender.SendTextMessage(session.chatId, GetText(), keyboard);
                _messageId = message.MessageId;
            }
        }

        private async Task ShowInfo()
        {
            _isInfoOpened = true;
            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_backButton")}", () => Task.Run(ShowAttributes) );

            var text = Localization.Get(session, "dialog_attributes_info");

            await messageSender.EditTextMessage(session.chatId, _messageId.Value, text, GetOneLineKeyboard());
        }

        private string GetText()
        {
            var profileData = session.profile.data;
            var stats = (PlayerStats)session.player.unitStats;

            var sb = new StringBuilder();
            string attributeStart = $"\n{Emojis.elements[Element.SmallWhite]} ";
            AppendAttribute("unit_attribute_strength", stats.currentStrength, profileData.attributeStrength);
            AppendAttribute("unit_attribute_vitality", stats.currentVitality, profileData.attributeVitality);
            AppendAttribute("unit_attribute_sorcery", stats.currentSorcery, profileData.attributeSorcery);
            AppendAttribute("unit_attribute_luck", stats.currentLuck, profileData.attributeLuck);

            var attributePoints = session.profile.data.attributePoints;
            if (!_isClosing && attributePoints > 0)
            {
                sb.Append("\n\n" + string.Format(Localization.Get(session, "dialog_attributes_available_points"), attributePoints) );
            }

            return sb.ToString();

            // --- Helpers:
            void AppendAttribute(string localizationKey, int currentStat, int profileStat)
            {
                sb.Append($"{attributeStart}{Localization.Get(session, localizationKey)}: <b>{currentStat}</b>");
                var additiveStat = currentStat - profileStat;
                if (additiveStat > 0)
                {
                    sb.Append($"{Emojis.space}<i>(+{additiveStat})</i>");
                }
            }
        }

        public override void OnDialogClose()
        {
            if (!_messageId.HasValue)
                return;

            if (_isInfoOpened)
            {
                _isClosing = true;
                messageSender.EditTextMessage(session.chatId, _messageId.Value, GetText(), null);
            }
            else
            {
                messageSender.EditMessageKeyboard(session.chatId, _messageId.Value, null);
            }
        }

        public override async Task SendAsync()
        {
            await ShowAttributes();
        }

        private void TryUpAttribute(ref short attribute)
        {
            if (session.profile.data.attributePoints > 0)
            {
                attribute++;
                session.profile.data.attributePoints--;
            }
            RecalculateStatsAndShowAttributes();
        }

        private void RecalculateStatsAndShowAttributes()
        {
            var playerStats = (PlayerStats)session.player.unitStats;
            playerStats.Recalculate();
            ShowAttributes();
        }


    }
}
