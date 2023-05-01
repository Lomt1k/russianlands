using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.UserControls;

namespace TextGameRPG.Views.UserControls
{
    public partial class ObjectPropertiesEditorView : UserControl
    {
        public ObjectPropertiesEditorViewModel vm => DataContext as ObjectPropertiesEditorViewModel;

        public ObjectPropertiesEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
