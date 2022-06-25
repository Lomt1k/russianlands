using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.UserControls;

namespace TextGameRPG.Views.Editor.UserControls
{
    public partial class ObjectFieldsEditorView : UserControl
    {
        public ObjectFieldsEditorViewModel viewModel => DataContext as ObjectFieldsEditorViewModel;

        public ObjectFieldsEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
