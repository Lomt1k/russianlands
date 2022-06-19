using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.Editor.QuestsEditor;

namespace TextGameRPG.Views.Editor.QuestsEditor
{
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
}
