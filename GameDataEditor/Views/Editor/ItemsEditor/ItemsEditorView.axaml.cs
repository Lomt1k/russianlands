using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GameDataEditor.ViewModels.Editor.ItemsEditor;

namespace GameDataEditor.Views.Editor.ItemsEditor;

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
