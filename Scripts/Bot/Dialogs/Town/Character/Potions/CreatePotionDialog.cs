using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Potions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public class CreatePotionDialog : DialogBase
    {
        private PotionData _data;

        public CreatePotionDialog(GameSession session, PotionData data) : base(session)
        {
            _data = data;
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>" + _data.GetName(session) + "</b>");

            sb.AppendLine();
            sb.Append(_data.GetDescription(session));

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }
    }
}
