using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarkOne.Views.Editor.MobsEditor;

public partial class MobInspectorView : UserControl
{
    public MobInspectorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
