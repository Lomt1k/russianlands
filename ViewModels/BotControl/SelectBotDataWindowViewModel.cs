using System;
using System.IO;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
using TextGameRPG.Views.BotControl;

namespace TextGameRPG.ViewModels.BotControl
{
    internal class SelectBotDataWindowViewModel : ViewModelBase
    {
        private const string botDatasPath = "botData";

        private string _selectedDataPath = string.Empty;

        private Action<string> _onBotDataSelected;
        private Action _closeWindow;

        public ObservableCollection<string> dataPathCollection { get; private set; }
        public string selectedDataPath
        {
            get => _selectedDataPath;
            set => this.RaiseAndSetIfChanged(ref _selectedDataPath, value);
        }

        public ReactiveCommand<Unit, Unit> addNewDataCommand { get; }
        public ReactiveCommand<Unit, Unit> removeSelectedDataCommand { get; }
        public ReactiveCommand<Unit, Unit> startBotCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        


        public SelectBotDataWindowViewModel(SelectBotDataWindow window, Action<string> onBotDataSelected)
        {
            _onBotDataSelected = onBotDataSelected;
            _closeWindow = () => window.Close();

            var directories = GetDirectories();
            dataPathCollection = new ObservableCollection<string>(directories);            

            addNewDataCommand = ReactiveCommand.Create(AddNewBotData);
            removeSelectedDataCommand = ReactiveCommand.Create(RemoveSelectedBotData);
            startBotCommand = ReactiveCommand.Create(StartBotWithSelectedData);
            cancelCommand = ReactiveCommand.Create(_closeWindow);
        }

        private string[] GetDirectories()
        {
            if (!Directory.Exists(botDatasPath))
            {
                Directory.CreateDirectory(botDatasPath);
            }
            return Directory.GetDirectories(botDatasPath);
        }

        private void AddNewBotData()
        {

        }

        private void RemoveSelectedBotData()
        {
            Directory.Delete(_selectedDataPath, true);
            dataPathCollection.Remove(_selectedDataPath);
        }

        private void StartBotWithSelectedData()
        {
            _onBotDataSelected(_selectedDataPath);
            _closeWindow();
        }

    }
}
