using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GameDataEditor.ViewModels.Editor.MobsEditor;

namespace GameDataEditor.Views.Editor.MobsEditor;

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
