using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameDataEditor.Views.Editor;

public partial class MainEditorView : UserControl
{
    public MainEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
