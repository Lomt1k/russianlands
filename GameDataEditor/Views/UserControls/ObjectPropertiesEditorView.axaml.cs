using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MarkOne.ViewModels.UserControls;

namespace GameDataEditor.Views.UserControls;

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
