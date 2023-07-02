using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.ArenaLeaguesEdittor;

namespace GameDataEditor.Views.Editor.ArenaLeaguesEditor;
public partial class ArenaLeaguesEditorView : UserControl
{
    public ArenaLeaguesEditorView()
    {
        InitializeComponent();
        DataContext = new ArenaLeaguesEditorViewModel();
    }
}
