using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views.Editor.MobsEditor
{
    public partial class MobInspectorView : UserControl
    {
        public MobInspectorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
