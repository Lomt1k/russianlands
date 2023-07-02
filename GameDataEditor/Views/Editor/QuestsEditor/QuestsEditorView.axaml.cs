using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GameDataEditor.ViewModels.Editor.QuestsEditor;

namespace GameDataEditor.Views.Editor.QuestsEditor;

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
