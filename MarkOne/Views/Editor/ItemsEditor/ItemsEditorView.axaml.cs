using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MarkOne.ViewModels.Editor.ItemsEditor;

namespace MarkOne.Views.Editor.ItemsEditor;

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
