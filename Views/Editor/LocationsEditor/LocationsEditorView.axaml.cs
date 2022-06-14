using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TextGameRPG.ViewModels.Editor.LocationsEditor;

namespace TextGameRPG.Views.Editor.LocationsEditor
{
    public partial class LocationsEditorView : UserControl
    {
        public LocationsEditorView()
        {
            InitializeComponent();
            DataContext = new LocationsEditorViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
