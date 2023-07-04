using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace MarkOne.Scripts.GameCore.Services.GameData;
[JsonObject]
public abstract class GameData
{
    [JsonIgnore]
    public string dataPath { get; private set; } = string.Empty;

    private void Init(string _dataPath)
    {
        dataPath = _dataPath;
    }

    public void Save()
    {
        if (Program.isBotAppStarted)
            return;

        var jsonStr = JsonConvert.SerializeObject(this, Formatting.Indented);
        using var writer = new StreamWriter(dataPath, false, Encoding.UTF8);
        writer.Write(jsonStr);
    }

    public static T LoadFromJSON<T>(string path) where T : GameData
    {
        if (!File.Exists(path))
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.Write("{}");
        }
        using var reader = new StreamReader(path, Encoding.UTF8);
        var jsonStr = reader.ReadToEnd();
        var obj = JsonConvert.DeserializeObject<T>(jsonStr);
        obj.Init(path);
        return obj;
    }

}
