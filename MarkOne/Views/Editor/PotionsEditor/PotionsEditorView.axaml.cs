using Avalonia.Controls;
using MarkOne.ViewModels.Editor.PotionsEditor;

namespace MarkOne.Views.Editor.PotionsEditor;

public partial class PotionsEditorView : UserControl
{
    public PotionsEditorView()
    {
        InitializeComponent();
        DataContext = new PotionsEditorViewModel();
    }
}
