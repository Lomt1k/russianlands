using Avalonia.Controls;
using MarkOne.ViewModels.Editor.ArenaLeaguesEdittor;

namespace MarkOne.Views.Editor.ArenaLeaguesEditor;
public partial class FakePlayerGenerationSettingsView : UserControl
{
    public FakePlayerGenerationSettingsViewModel vm { get; }

    public FakePlayerGenerationSettingsView()
    {
        InitializeComponent();
        DataContext = vm = new FakePlayerGenerationSettingsViewModel();
    }
}
