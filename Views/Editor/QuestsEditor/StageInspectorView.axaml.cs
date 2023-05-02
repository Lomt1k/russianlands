using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TextGameRPG.Views.Editor.QuestsEditor;

public partial class StageInspectorView : UserControl
{
    public StageInspectorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
