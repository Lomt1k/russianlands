using Avalonia.Controls;
using GameDataEditor.ViewModels.Editor.BuildingsEditor;

namespace GameDataEditor.Views.Editor.BuildingsEditor;

public partial class BuildingsEditorView : UserControl
{
    public BuildingsEditorView()
    {
        InitializeComponent();
        DataContext = new BuildingsEditorViewModel();
    }
}
