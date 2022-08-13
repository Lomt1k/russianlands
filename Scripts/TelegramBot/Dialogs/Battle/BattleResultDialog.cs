using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Battle
{
    public class BattleResultDialog : DialogBase
    {
        public BattleResultDialog(GameSession _session, BattleResultData data) : base(_session)
        {
        }

        public override Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
