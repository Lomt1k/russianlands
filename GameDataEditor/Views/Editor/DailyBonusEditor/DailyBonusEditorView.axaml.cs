using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.DailyBonusEditor;

namespace GameDataEditor.Views.Editor.DailyBonusEditor;
public partial class DailyBonusEditorView : UserControl
{
    public DailyBonusEditorView()
    {
        InitializeComponent();
        DataContext = new DailyBonusEditorViewModel();
    }
}
