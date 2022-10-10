using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.TelegramBot.Dialogs;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class TooltipController
    {
        private List<Tooltip> _tooltips = new List<Tooltip>();
        private int _currentIndex;

        public bool hasTooltips => _currentIndex < _tooltips.Count;

        public TooltipController()
        {
        }

        public void SetupTooltips(List<Tooltip> tooltips)
        {
            _tooltips = tooltips;
            _currentIndex = 0;
        }

        public void SwitchToPrevious()
        {
            _currentIndex--;
        }

        public Tooltip? TryGetNext(DialogBase dialogType)
        {
            if (!hasTooltips)
                return null;

            return TryGetNext(dialogType.GetType().Name);
        }

        public Tooltip? TryGetNext(DialogPanelBase dialogPanelType)
        {
            if (!hasTooltips)
                return null;

            return TryGetNext(dialogPanelType.GetType().Name);
        }

        private Tooltip? TryGetNext(string dialogType)
        {
            var tooltip = GetCurrentTooltip();
            if (tooltip == null)
                return null;

            if (!dialogType.Equals(tooltip.dialogType))
                return null;

            _currentIndex++;
            return tooltip;
        }

        private Tooltip? GetCurrentTooltip()
        {
            return _currentIndex >= _tooltips.Count ? null : _tooltips[_currentIndex];
        }

    }
}
