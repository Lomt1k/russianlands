using Avalonia.Controls;
using TextGameRPG.ViewModels.Editor.LocationMobsEditor;

namespace TextGameRPG.Views.Editor.LocationMobsEditor;

public partial class LocationMobsEditorView : UserControl
{
    public LocationMobsEditorView()
    {
        InitializeComponent();
        DataContext = new LocationMobsEditorViewModel();
    }
}
