using Avalonia.Controls;
using ReactiveUI;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Views;

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
            _mainContentView.DataContext = new GameDataLoaderViewModel(LaunchEditor, LaunchBotDataSelector);
        }

        private void LaunchEditor()
        {
            Program.SetupAppMode(AppMode.Editor);
            mainContentView = new Views.Editor.MainEditorView();
            mainContentView.DataContext = new Editor.MainEditorViewModel();
            MainWindow.instance.WindowState = WindowState.Maximized;
        }

        private void LaunchBotDataSelector()
        {
            var selectBotWindow = new Views.BotControl.SelectBotDataWindow();
            selectBotWindow.DataContext = new BotControl.SelectBotDataWindowViewModel(selectBotWindow, (botDataPath) => LauchBot(botDataPath));
            selectBotWindow.ShowDialog(Program.mainWindow);
        }

        private void LauchBot(string botDataPath)
        {
            Program.SetupAppMode(AppMode.Bot);
            BotController.Init(botDataPath);
            mainContentView = new Views.BotControl.BotControl();
            mainContentView.DataContext = new BotControl.BotControlViewModel();
        }



    }
}
