using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Battle
{
    public class SelectBattleActionDialog : DialogBase
    {
        private Action<List<IBattleAction>> _actionsCallback;

        public SelectBattleActionDialog(GameSession _session, Action<List<IBattleAction>> callback) : base(_session)
        {
            _actionsCallback = callback;
        }

        public override async Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
