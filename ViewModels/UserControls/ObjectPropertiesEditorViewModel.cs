using System.Collections.ObjectModel;
using TextGameRPG.Models.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.UserControls
{
    public class ObjectPropertiesEditorViewModel : ViewModelBase
    {
        private ObservableCollection<ObjectPropertyModel> _properties = new ObservableCollection<ObjectPropertyModel>();

        public object editableObject {get; private set; }
        public string objectName  => editableObject.GetType().Name;
        public ObservableCollection<ObjectPropertyModel> properties => _properties;

        public ObjectPropertiesEditorViewModel(object obj)
        {
            ObjectPropertyModel.FillObservableCollection(ref _properties, obj);
            editableObject = obj;
        }

        public T GetEditableObject<T>()
        {
            return (T)editableObject;
        }

    }
}
