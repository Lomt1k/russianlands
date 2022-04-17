using System;
using System.Collections.ObjectModel;
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

        public GameDataLoaderViewModel(Action onSuccess, Action onFailed)
        {
            InitializeItems();

            //bool result = true;//TODO Loading logic
            //if (result)
            //{
            //    onSuccess();
            //}
            //else
            //{
            //    onFailed();
            //}
        }

        private void InitializeItems()
        {
            items = new ObservableCollection<GameDataLoaderStateInfo>();
            AddNext( new GameDataLoaderStateInfo("Application started.") );
        }

        private void AddNext(GameDataLoaderStateInfo stateInfo)
        {
            items.Add(stateInfo);
            currentState = stateInfo;
        }


    }
}
