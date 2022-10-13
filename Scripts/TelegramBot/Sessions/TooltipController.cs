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

        public bool HasTooltipToAppend(DialogBase dialogType)
        {
            if (!hasTooltips)
                return false;

            return HasTooltipToAppend(dialogType.GetType().Name);
        }

        public bool HasTooltipToAppend(DialogPanelBase dialogPanelType)
        {
            if (!hasTooltips)
                return false;

            return HasTooltipToAppend(dialogPanelType.GetType().Name);
        }

        private bool HasTooltipToAppend(string dialogType)
        {
            var tooltip = GetCurrentTooltip();
            if (tooltip == null)
                return false;

            if (!dialogType.Equals(tooltip.dialogType))
                return false;

            return true;
        }

        public Tooltip? TryGetTooltip(DialogBase dialogType)
        {
            if (!hasTooltips)
                return null;

            return TryGetTooltip(dialogType.GetType().Name);
        }

        public Tooltip? TryGetTooltip(DialogPanelBase dialogPanelType)
        {
            if (!hasTooltips)
                return null;

            return TryGetTooltip(dialogPanelType.GetType().Name);
        }

        private Tooltip? TryGetTooltip(string dialogType)
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

        public bool IfNextTooltipForPanelWithWaitingButtonClick()
        {
            var nextIndex = _currentIndex + 1;
            if (nextIndex >= _tooltips.Count)
                return false;

            var nextTooltip = _tooltips[nextIndex];
            return nextTooltip.isTooltipForDialogPanel && nextTooltip.buttonId > -1;
        }

    }
}
