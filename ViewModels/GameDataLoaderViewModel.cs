using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using TextGameRPG.Models;

namespace TextGameRPG.ViewModels
{
    public class GameDataLoaderViewModel : ViewModelBase
    {
        private GameDataLoaderStateInfo _currentState;

        public ObservableCollection<GameDataLoaderStateInfo> items { get; private set; }
        public GameDataLoaderStateInfo currentState
        {
            get => _currentState;
            set => this.RaiseAndSetIfChanged(ref _currentState, value);
        }
        public bool isGameDataLoaded { get; private set; }

        public ReactiveCommand<Unit, Unit> launchEditorCommand { get; }
        public ReactiveCommand<Unit, Unit> launchBotCommand { get; }

        public GameDataLoaderViewModel(Action launchEditor, Action launchBotDataSelector)
        {
            items = new ObservableCollection<GameDataLoaderStateInfo>();
            launchEditorCommand = ReactiveCommand.Create(launchEditor);
            launchBotCommand = ReactiveCommand.Create(launchBotDataSelector);

            AddNext("Application started");
            LoadGameData();
        }

        private void LoadGameData()
        {
            Scripts.GameCore.GameDataBase.GameDataBase.LoadAllData(this);
        }

        public void AddNext(string stateInfo)
        {
            var state = new GameDataLoaderStateInfo(stateInfo);
            items.Add(state);
            currentState = state;
            Program.logger.Info(stateInfo);
        }

        public void AddInfoToCurrentState(string text)
        {
            currentState?.AddInfo(text);
        }

        public void OnGameDataLoaded()
        {
            isGameDataLoaded = true;
            AddNext("All Game Data successfully loaded");
        }


    }
}
