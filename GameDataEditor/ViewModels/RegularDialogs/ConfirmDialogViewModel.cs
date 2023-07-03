using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Reactive;

namespace GameDataEditor.ViewModels.RegularDialogs;

public class ConfirmDialogViewModel : ViewModelBase
{
    public string description { get; }
    public ReactiveCommand<Unit, Unit> confirmCommand { get; }
    public ReactiveCommand<Unit, Unit> declineCommand { get; }


    public ConfirmDialogViewModel(Window dialogView, string _description, Action onConfirm, Action? onDecline)
    {
        description = _description;
        confirmCommand = ReactiveCommand.Create(onConfirm + dialogView.Close);
        declineCommand = ReactiveCommand.Create(onDecline + dialogView.Close);
    }

}
