using MarkOne.Scripts.GameCore.Services.GameData;
using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Units;

[JsonObject]
public class FakePlayerGenerationData : IGameDataWithId<int>
{
    public int id { get; set; }
    public string nickname { get; set; } = string.Empty;
    public bool isFemale { get; set; }

    public void OnBotAppStarted()
    {
        // nothing
    }
}
