using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace TextGameRPG.Scripts.GameCore.GameDataBase
{
    public class DataDictionaryWithIntegerID<T> where T : IDataWithIntegerID
    {
        public readonly string dataPath;

        public System.Action onDataChanged;

        private Dictionary<int, T> _dictionary;

        public int count => _dictionary.Count;

        public DataDictionaryWithIntegerID(Dictionary<int, T> dictionary, string path)
        {
            _dictionary = dictionary;
            dataPath = path;
        }

        public static DataDictionaryWithIntegerID<Type> LoadFromJSON<Type>(string path) where Type : IDataWithIntegerID
        {
            using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
            {
                var jsonStr = reader.ReadToEnd();
                var dictionary = JsonConvert.DeserializeObject<IEnumerable<Type>>(jsonStr).ToDictionary(x => x.id);
                return new DataDictionaryWithIntegerID<Type>(dictionary, path);
            }
        }

        public T GetData(int id)
        {
            return _dictionary[id];
        }

        public IEnumerable<T> GetAllData()
        {
            return _dictionary.Values;
        }

        public void AddData(int id, T data)
        {
            _dictionary.Add(id, data);
            OnDataChanged();
        }

        public void ChangeData(int id, T data)
        {
            _dictionary[id] = data;
            OnDataChanged();
        }

        public void RemoveData(int id)
        {
            _dictionary.Remove(id);
            OnDataChanged();
        }

        private void OnDataChanged()
        {
            Save();
            if (onDataChanged != null)
            {
                onDataChanged();
            }
        }

        public void Save()
        {
            var jsonStr = JsonConvert.SerializeObject(_dictionary.Values, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(dataPath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
        }

    }
}
