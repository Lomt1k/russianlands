using Avalonia.Controls;
using ReactiveUI;
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
            _mainContentView.DataContext = new GameDataLoaderViewModel(LaunchEditor);
        }

        private void LaunchEditor()
        {
            mainContentView = new Views.Editor.MainEditorView();
            mainContentView.DataContext = new ViewModels.Editor.MainEditorViewModel();
            MainWindow.instance.WindowState = WindowState.Maximized;
        }



    }
}
