using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views.BotControl
{
    public partial class BotControl : UserControl
    {
        public BotControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
