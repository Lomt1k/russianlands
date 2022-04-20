using System;
using TextGameRPG.ViewModels.Editor.RegularDialogs;
using TextGameRPG.Views.RegularDialogs;

namespace TextGameRPG.Models.RegularDialogs
{
    internal static class RegularDialogHelper
    {
        public static void ShowConfirmDialog(string description, Action onConfirm, Action onDecline = null)
        {
            var dialog = new ConfirmDialog();
            dialog.DataContext = new ConfirmDialogViewModel(dialog, description, onConfirm, onDecline);
            dialog.ShowDialog(Program.mainWindow);
        }

    }
}
