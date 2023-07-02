using GameDataEditor.ViewModels.RegularDialogs;
using GameDataEditor.Views.RegularDialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameDataEditor.Models.RegularDialogs;

public static class RegularDialogHelper
{
    public static Task ShowConfirmDialog(string description, Action onConfirm, Action? onDecline = null)
    {
        var dialog = new ConfirmDialog();
        dialog.DataContext = new ConfirmDialogViewModel(dialog, description, onConfirm, onDecline);
        return dialog.ShowDialog(App.mainWindow);
    }

    public static Task ShowAskValueDialog<T>(string description, Action<T> onEntered)
    {
        var dialog = new AskValueDialog();
        dialog.DataContext = new AskValueDialogViewModel<T>(dialog, description, onEntered);
        return dialog.ShowDialog(App.mainWindow);
    }

    public static Task ShowItemSelectionDialog(string description, Dictionary<string, Action> itemsWithCallbacks)
    {
        var dialog = new ItemSelectionDialog();
        dialog.DataContext = new ItemSelectionDialogViewModel(dialog, description, itemsWithCallbacks);
        return dialog.ShowDialog(App.mainWindow);
    }

}
