using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MarkOne.ViewModels.UserControls;

namespace MarkOne.Views.UserControls;

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
