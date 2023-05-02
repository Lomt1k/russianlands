using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MarkOne.Views.Editor.QuestsEditor;

public partial class StageWithBattleView : UserControl
{
    public StageWithBattleView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
