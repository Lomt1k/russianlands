using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameDataEditor.Views.Editor.QuestsEditor;

public partial class StageWithReplicaView : UserControl
{
    public StageWithReplicaView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
