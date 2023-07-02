using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameDataEditor.Models;

public class EnumValueModel<T> where T : Enum
{
    public string name;
    public T value;

    public override string ToString()
    {
        return name;
    }

    public static ObservableCollection<EnumValueModel<T>> CreateCollection(IEnumerable<T> excludeValues)
    {
        var names = Enum.GetNames(typeof(T));
        var values = Enum.GetValues(typeof(T));

        var list = new List<EnumValueModel<T>>(names.Length);
        for (var i = 0; i < names.Length; i++)
        {
            var item = new EnumValueModel<T>
            {
                name = names[i],
                value = (T)values.GetValue(i)
            };
            list.Add(item);
        }

        foreach (var excludeValue in excludeValues)
        {
            var elementToRemove = list.Where(x => x.value.Equals(excludeValue)).First();
            list.Remove(elementToRemove);
        }

        return new ObservableCollection<EnumValueModel<T>>(list);
    }

    public static ObservableCollection<EnumValueModel<T>> CreateCollection(T excludeValue)
    {
        return CreateCollection(new T[] { excludeValue });
    }

    public static ObservableCollection<EnumValueModel<T>> CreateCollection()
    {
        return CreateCollection(new T[] { });
    }

    public static EnumValueModel<T> GetModel(ObservableCollection<EnumValueModel<T>> collection, T enumValue)
    {
        foreach (var element in collection)
        {
            if (element.value.Equals(enumValue))
            {
                return element;
            }
        }
        return null;
    }


}
