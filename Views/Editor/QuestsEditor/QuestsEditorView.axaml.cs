using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MarkOne.ViewModels.Editor.QuestsEditor;

namespace MarkOne.Views.Editor.QuestsEditor;

public partial class QuestsEditorView : UserControl
{
    public QuestsEditorView()
    {
        InitializeComponent();
        DataContext = new QuestsEditorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
