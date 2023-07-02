using Avalonia.Controls;
using MarkOne.ViewModels.Editor.BuildingsEditor;

namespace MarkOne.Views.Editor.BuildingsEditor;

public partial class BuildingsEditorView : UserControl
{
    public BuildingsEditorView()
    {
        InitializeComponent();
        DataContext = new BuildingsEditorViewModel();
    }
}
