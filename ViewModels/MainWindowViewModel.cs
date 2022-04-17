using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace TextGameRPG.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private UserControl _mainContentView;

        public UserControl mainContentView 
        {
            get => _mainContentView;
            set => this.RaiseAndSetIfChanged(ref _mainContentView, value);
        }

        public MainWindowViewModel()
        {
            _mainContentView = new Views.GameDataLoaderView();
            _mainContentView.DataContext = new GameDataLoaderViewModel(OnSuccessGameDataLoaded, OnFailGameDataLoading);
        }

        private void OnSuccessGameDataLoaded()
        {

        }

        private void OnFailGameDataLoading()
        {

        }



    }
}
