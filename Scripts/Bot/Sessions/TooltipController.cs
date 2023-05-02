using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.Dialogs;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;

namespace TextGameRPG.Scripts.Bot.Sessions;

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

    public bool HasTooltipToAppend(DialogBase dialog)
    {
        if (!hasTooltips)
            return false;

        return HasTooltipToAppend(dialog.GetType().Name);
    }

    public bool HasTooltipToAppend(DialogPanelBase dialogPanel)
    {
        if (!hasTooltips)
            return false;

        return HasTooltipToAppend(dialogPanel.GetType().Name);
    }

    private bool HasTooltipToAppend(string dialogType)
    {
        var tooltip = GetTooltip();
        if (tooltip == null)
            return false;

        return dialogType.Equals(tooltip.dialogType);
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

    public bool IfCurrentTooltipForPanelWithWaitingButtonClick()
    {
        var currentIndex = _nextIndex - 1;
        if (currentIndex < 0 || currentIndex >= _tooltips.Count)
            return false;

        var currentTooltip = _tooltips[currentIndex];
        return currentTooltip.isTooltipForDialogPanel && currentTooltip.buttonId > -1;
    }

}
