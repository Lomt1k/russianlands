using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.PotionsEditor;

namespace GameDataEditor.Views.Editor.PotionsEditor;

public partial class PotionsEditorView : UserControl
{
    public PotionsEditorView()
    {
        InitializeComponent();
        DataContext = new PotionsEditorViewModel();
    }
}
