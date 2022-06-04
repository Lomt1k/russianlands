using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal enum Attribute { Strength, Vitality, Sorcery, Luck }

    internal class AttributesDialogPanel : DialogPanelBase
    {
        private int? _messageId;
        private bool _isInfoOpened;
        private bool _isClosing;

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
                    TryUpAttribute(Attribute.Strength));
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_vitality")}",
                    TryUpAttribute(Attribute.Vitality));
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_sorcery")}",
                    TryUpAttribute(Attribute.Sorcery));
                RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "unit_attribute_luck")}",
                    TryUpAttribute(Attribute.Luck));
            }
            RegisterButton($"{Emojis.elements[Element.Info]} {Localization.Get(session, "dialog_attributes_tooltip")}", ShowInfo());

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
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_button")}", ShowAttributes());

            var text = Localization.Get(session, "dialog_attributes_info");

            await messageSender.EditTextMessage(session.chatId, _messageId.Value, text, GetOneLineKeyboard());
        }

        private string GetText()
        {
            var profileData = session.profile.data;
            var stats = (PlayerStats)session.player.unitStats;

            var sb = new StringBuilder();
            string attributeStart = $"\n{Emojis.elements[Element.SmallWhite]} ";
            AppendAttribute("unit_attribute_strength", stats.attributeStrength, profileData.attributeStrength);
            AppendAttribute("unit_attribute_vitality", stats.attributeVitality, profileData.attributeVitality);
            AppendAttribute("unit_attribute_sorcery", stats.attributeSorcery, profileData.attributeSorcery);
            AppendAttribute("unit_attribute_luck", stats.attributeLuck, profileData.attributeLuck);

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
                    sb.Append($"{Emojis.bigSpace}<i>(+{additiveStat})</i>");
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

        private async Task TryUpAttribute(Attribute attribute)
        {
            if (session.profile.data.attributePoints > 0)
            {
                switch (attribute)
                {
                    case Attribute.Strength: session.profile.data.attributeStrength++; break;
                    case Attribute.Vitality: session.profile.data.attributeVitality++; break;
                    case Attribute.Sorcery: session.profile.data.attributeSorcery++; break;
                    case Attribute.Luck: session.profile.data.attributeLuck++; break;
                }
                session.profile.data.attributePoints--;
            }
            await RecalculateStatsAndShowAttributes();
        }

        private async Task RecalculateStatsAndShowAttributes()
        {
            var playerStats = (PlayerStats)session.player.unitStats;
            playerStats.Recalculate();
            await ShowAttributes();
        }


    }
}
