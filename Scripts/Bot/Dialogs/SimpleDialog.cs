using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs
{
    public class SimpleDialog : DialogBase
    {
        private string _text;

        public SimpleDialog(GameSession _session, string text, bool withTownButton, Dictionary<string,Func<Task>> buttons) : base(_session)
        {
            _text = text;
            foreach (var button in buttons)
            {
                RegisterButton(button.Key, button.Value);
            }
            if (withTownButton)
            {
                RegisterTownButton(isFullBack: false);
            }
        }

        public SimpleDialog(GameSession _session, string text) : base(_session)
        {
            _text = text;
            RegisterTownButton(isFullBack: false);
        }

        public override async Task Start()
        {
            await SendDialogMessage(_text, GetMultilineKeyboard());
        }
    }
}
