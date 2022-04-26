﻿using System;
using TextGameRPG.Views.RegularDialogs;
using ReactiveUI;
using System.Reactive;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.ViewModels.RegularDialogs
{
    internal class AskValueDialogViewModel<T> : ViewModelBase
    {
        private string _inputValue = string.Empty;
        private bool _isValidInput;
        private T _parsedValue;
        private Action<T> _onEntered;

        public string description { get; }
        public string inputValue
        {
            get => _inputValue;
            set
            {
                this.RaiseAndSetIfChanged(ref _inputValue, value);
                Validation();
            }
        }
        public bool isValidInput
        {
            get => _isValidInput;
            set => this.RaiseAndSetIfChanged(ref _isValidInput, value);
        }

        public Action closeWindow { get; }
        public ReactiveCommand<Unit, Unit> acceptCommand { get;  }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public AskValueDialogViewModel(AskValueDialog window, string description, Action<T> onEntered)
        {
            this.description = description;
            _onEntered = onEntered;

            closeWindow = () => window.Close();
            acceptCommand = ReactiveCommand.Create(OnAccept);
            cancelCommand = ReactiveCommand.Create(closeWindow);
        }

        private void OnAccept()
        {
            _onEntered(_parsedValue);
            closeWindow();
        }

        private void Validation()
        {
            isValidInput = _inputValue.TryParse(out _parsedValue);
        }

    }
}
