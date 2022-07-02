using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.UserControls;

namespace TextGameRPG.Views.UserControls
{
    public partial class ObjectFieldsEditorView : UserControl
    {
        public ObjectFieldsEditorViewModel vm => DataContext as ObjectFieldsEditorViewModel;

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
