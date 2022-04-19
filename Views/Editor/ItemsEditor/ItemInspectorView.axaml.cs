using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views.Editor.ItemsEditor
{
    public partial class ItemInspectorView : UserControl
    {
        public ItemInspectorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
