using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Skills;

internal class SkillBoots : ISkill
{
    public ItemType itemType => ItemType.Boots;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitCoconut,
        ResourceId.FruitWatermelon,
        ResourceId.FruitBlueberry,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillBoots;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillBoots = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillBoots += value;
    }

}
