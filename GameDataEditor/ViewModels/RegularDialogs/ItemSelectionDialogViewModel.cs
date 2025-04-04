﻿using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;

namespace GameDataEditor.ViewModels.RegularDialogs;

public class ItemSelectionDialogViewModel : ViewModelBase
{
    private readonly Window _window;
    private readonly Dictionary<string, Action> _itemsWithCallbacks;
    private string? _selectedItem;

    public string description { get; }
    public ObservableCollection<string> items { get; }
    public string? selectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public ReactiveCommand<Unit, Unit> confirmSelectionCommand { get; }
    public ReactiveCommand<Unit, Unit> closeCommand { get; }

    public ItemSelectionDialogViewModel(Window dialogView, string description, Dictionary<string, Action> itemsWithCallbacks)
    {
        _window = dialogView;
        _itemsWithCallbacks = itemsWithCallbacks;
        this.description = description;
        items = new ObservableCollection<string>();
        foreach (var item in _itemsWithCallbacks.Keys)
        {
            items.Add(item);
        }
        confirmSelectionCommand = ReactiveCommand.Create(ConfirmSelection);
        closeCommand = ReactiveCommand.Create(_window.Close);
    }

    public void ConfirmSelection()
    {
        if (selectedItem == null)
            return;

        _itemsWithCallbacks[selectedItem]?.Invoke();
        _window.Close();
    }

}
