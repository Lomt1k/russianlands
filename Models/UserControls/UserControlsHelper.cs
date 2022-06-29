using TextGameRPG.ViewModels.UserControls;
using TextGameRPG.Views.Editor.UserControls;

namespace TextGameRPG.Models.UserControls
{
    public static class UserControlsHelper
    {
        public static ObjectFieldsEditorView CreateObjectFieldsEditor(object obj)
        {
            var view = new ObjectFieldsEditorView();
            view.DataContext = new ObjectFieldsEditorViewModel(obj);
            return view;
        }

    }
}
