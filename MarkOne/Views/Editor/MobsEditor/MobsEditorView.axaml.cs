using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MarkOne.ViewModels.Editor.MobsEditor;

namespace MarkOne.Views.Editor.MobsEditor;

public partial class MobsEditorView : UserControl
{
    public MobsEditorView()
    {
        InitializeComponent();
        DataContext = new MobsEditorViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
