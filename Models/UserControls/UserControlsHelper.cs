using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.ViewModels.UserControls;
using TextGameRPG.Views.Editor.UserControls;

namespace TextGameRPG.Models.UserControls
{
    public static class UserControlsHelper
    {
        public static ObjectFieldsEditorView CreateObjectEditorView(object obj)
        {
            var view = new ObjectFieldsEditorView();
            view.DataContext = new ObjectFieldsEditorViewModel(obj);
            return view;
        }

        public static void RefillObjectEditorsCollection(ObservableCollection<ObjectFieldsEditorView> collection, IEnumerable<object> objects)
        {
            collection.Clear();
            foreach (var element in objects)
            {
                var editorView = CreateObjectEditorView(element);
                collection.Add(editorView);
            }
        }

    }
}
