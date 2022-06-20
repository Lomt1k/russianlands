using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;

namespace TextGameRPG.ViewModels.RegularDialogs
{
    internal class ConfirmDialogViewModel : ViewModelBase
    {
        public string description { get; }
        public ReactiveCommand<Unit, Unit> confirmCommand { get; }
        public ReactiveCommand<Unit, Unit> declineCommand { get; }


        public ConfirmDialogViewModel(Window dialogView, string _description, Action onConfirm, Action? onDecline)
        {
            description = _description;
            confirmCommand = ReactiveCommand.Create(dialogView.Close + onConfirm);
            declineCommand = ReactiveCommand.Create(dialogView.Close + onDecline);
        }

    }
}
