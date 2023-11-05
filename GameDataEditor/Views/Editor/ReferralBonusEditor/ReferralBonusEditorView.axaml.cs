using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.ReferralBonusEditor;

namespace GameDataEditor.Views.Editor.ReferralBonusEditor;
public partial class ReferralBonusEditorView : UserControl
{
    public ReferralBonusEditorView()
    {
        InitializeComponent();
        DataContext = new ReferralBonusEditorViewModel();
    }
}
