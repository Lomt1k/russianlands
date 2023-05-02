using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextGameRPG.Scripts.GameCore.Services.GameData;

public class GameDataDictionary<TId, TData> where TData : IGameDataWithId<TId>
{
    public readonly string dataPath;

    public Action onDataChanged;

    private readonly Dictionary<TId, TData> _dictionary;

    public int count => _dictionary.Count;

    public TData this[TId id] => _dictionary[id];

    public GameDataDictionary(Dictionary<TId, TData> dictionary, string path)
    {
        _dictionary = dictionary;
        dataPath = path;

        Program.onSetupAppMode += OnSetupAppMode;
    }

    public static GameDataDictionary<TId, TData> LoadFromJSON<TId, TData>(string path) where TData : IGameDataWithId<TId>
    {
        if (!File.Exists(path))
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.Write("[]");
            }
        }
        using (var reader = new StreamReader(path, Encoding.UTF8))
        {
            var jsonStr = reader.ReadToEnd();
            var dictionary = JsonConvert.DeserializeObject<IEnumerable<TData>>(jsonStr).ToDictionary(x => x.id);
            return new GameDataDictionary<TId, TData>(dictionary, path);
        }
    }

    public bool ContainsKey(TId id)
    {
        return _dictionary.ContainsKey(id);
    }

    public bool TryGetValue(TId id, out TData value)
    {
        return _dictionary.TryGetValue(id, out value);
    }

    public IEnumerable<TData> GetAllData()
    {
        return _dictionary.Values;
    }

    public void AddData(TId id, TData data)
    {
        _dictionary.Add(id, data);
        OnDataChanged();
    }

    public void ChangeData(TId id, TData data)
    {
        _dictionary[id] = data;
        OnDataChanged();
    }

    public void RemoveData(TId id)
    {
        _dictionary.Remove(id);
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        onDataChanged?.Invoke();
    }

    public void Save()
    {
        if (Program.appMode != AppMode.Editor)
            return;

        var jsonStr = JsonConvert.SerializeObject(_dictionary.Values, Formatting.Indented);
        using (var writer = new StreamWriter(dataPath, false, Encoding.UTF8))
        {
            writer.Write(jsonStr);
        }
    }

    private void OnSetupAppMode(AppMode appMode)
    {
        if (appMode != AppMode.PlayMode)
            return;

        foreach (var item in _dictionary.Values)
        {
            item.OnSetupAppMode(appMode);
        }
    }


}
