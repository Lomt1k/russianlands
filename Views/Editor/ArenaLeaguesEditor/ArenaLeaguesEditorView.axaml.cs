using Avalonia.Controls;
using MarkOne.ViewModels.Editor.ArenaLeaguesEdittor;

namespace MarkOne.Views.Editor.ArenaLeaguesEditor;
public partial class ArenaLeaguesEditorView : UserControl
{
    public ArenaLeaguesEditorView()
    {
        InitializeComponent();
        DataContext = new ArenaLeaguesEditorViewModel();
    }
}
