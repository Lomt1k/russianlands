using Avalonia.Controls;
using TextGameRPG.ViewModels.Editor.PotionsEditor;

namespace TextGameRPG.Views.Editor.PotionsEditor;

public partial class PotionsEditorView : UserControl
{
    public PotionsEditorView()
    {
        InitializeComponent();
        DataContext = new PotionsEditorViewModel();
    }
}
