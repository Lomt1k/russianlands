using Avalonia.Controls;
using TextGameRPG.ViewModels.Editor.BuildingsEditor;

namespace TextGameRPG.Views.Editor.BuildingsEditor
{
    public partial class BuildingsEditorView : UserControl
    {
        public BuildingsEditorView()
        {
            InitializeComponent();
            DataContext = new BuildingsEditorViewModel();
        }
    }
}
