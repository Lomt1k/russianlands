using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.ArenaSettingsEditor;

namespace GameDataEditor.Views.Editor.ArenaSettingsEditor;
public partial class ArenaSettingsEditorView : UserControl
{
    public ArenaSettingsEditorView()
    {
        InitializeComponent();
        DataContext = new ArenaSettingsEditorViewModel();
    }
}
