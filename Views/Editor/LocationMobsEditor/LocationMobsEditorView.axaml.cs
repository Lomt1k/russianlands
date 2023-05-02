using Avalonia.Controls;
using MarkOne.ViewModels.Editor.LocationMobsEditor;

namespace MarkOne.Views.Editor.LocationMobsEditor;

public partial class LocationMobsEditorView : UserControl
{
    public LocationMobsEditorView()
    {
        InitializeComponent();
        DataContext = new LocationMobsEditorViewModel();
    }
}
