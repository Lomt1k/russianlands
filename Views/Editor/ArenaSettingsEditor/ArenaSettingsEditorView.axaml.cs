using Avalonia.Controls;
using MarkOne.ViewModels.Editor.ArenaSettingsEditor;

namespace MarkOne.Views.Editor.ArenaSettingsEditor;
public partial class ArenaSettingsEditorView : UserControl
{
    public ArenaSettingsEditorView()
    {
        InitializeComponent();
        DataContext = new ArenaSettingsEditorViewModel();
    }
}
