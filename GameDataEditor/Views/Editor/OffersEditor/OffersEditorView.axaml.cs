using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.OffersEditor;

namespace GameDataEditor.Views.Editor.OffersEditor;
public partial class OffersEditorView : UserControl
{
    public OffersEditorView()
    {
        InitializeComponent();
        DataContext = new OffersEditorViewModel();
    }
}
