using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class PotionsProductionDialog : DialogBase
    {
        private PotionsProductionDialogPanel _productionPanel;

        public PotionsProductionDialog(GameSession session) : base(session)
        {
            _productionPanel = new PotionsProductionDialogPanel(this, 0);
            RegisterPanel(_productionPanel);
        }

        public override Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
