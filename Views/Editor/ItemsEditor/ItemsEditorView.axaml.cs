using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.Editor.ItemsEditor;

namespace TextGameRPG.Views.Editor.ItemsEditor
{
    public partial class ItemsEditorView : UserControl
    {
        public ItemsEditorView()
        {
            InitializeComponent();
            DataContext = new ItemsEditorViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
