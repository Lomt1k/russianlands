using MarkOne.Scripts.GameCore.Items;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class FakePlayerGenerationSettings
{
    public byte minPlayerLevel { get; set; }
    public byte maxPlayerLevel { get; set; }
    public byte minSkillLevel { get; set; }
    public byte maxSkillLevel { get; set; }
    public byte minItemLevel { get; set; }
    public List<WeightedRarity> itemRarities { get; set; } = new();
}
