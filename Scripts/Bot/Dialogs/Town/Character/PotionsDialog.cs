using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class PotionsDialog : DialogBase
    {
        private bool _backToBuilding;

        public PotionsDialog(GameSession session, bool backToBuilding = false) : base(session)
        {
            _backToBuilding = backToBuilding;
        }

        public override async Task Start()
        {
            await SendDialogMessage("In development", GetMultilineKeyboard());
        }
    }
}
