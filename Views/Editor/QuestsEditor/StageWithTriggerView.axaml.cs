using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views.Editor.QuestsEditor
{
    public partial class StageWithTriggerView : UserControl
    {
        public StageWithTriggerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
