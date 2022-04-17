using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views
{
    public partial class GameDataLoaderView : UserControl
    {
        public GameDataLoaderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
