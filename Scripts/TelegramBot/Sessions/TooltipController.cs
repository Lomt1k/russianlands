using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.TelegramBot.Dialogs;

namespace TextGameRPG.Scripts.TelegramBot.Sessions
{
    public class TooltipController
    {
        private List<Tooltip> _tooltips = new List<Tooltip>();
        private int _nextIndex;

        public bool hasTooltips => _nextIndex < _tooltips.Count;

        public TooltipController()
        {
        }

        public void SetupTooltips(List<Tooltip> tooltips)
        {
            _tooltips = tooltips;
            _nextIndex = 0;
        }

        public void SwitchToPrevious()
        {
            _nextIndex--;
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
            var tooltip = GetTooltip();
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
            var tooltip = GetTooltip();
            if (tooltip == null)
                return null;

            if (!dialogType.Equals(tooltip.dialogType))
                return null;

            _nextIndex++;
            return tooltip;
        }

        private Tooltip? GetTooltip()
        {
            return _nextIndex >= _tooltips.Count ? null : _tooltips[_nextIndex];
        }

        public bool IfNextTooltipForPanelWithWaitingButtonClick()
        {
            if (_nextIndex >= _tooltips.Count)
                return false;

            var nextTooltip = _tooltips[_nextIndex];
            return nextTooltip.isTooltipForDialogPanel && nextTooltip.buttonId > -1;
        }

    }
}
