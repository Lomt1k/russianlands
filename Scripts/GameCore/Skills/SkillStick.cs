using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Skills;

internal class SkillStick : ISkill
{
    public ItemType itemType => ItemType.Stick;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitPear,
        ResourceId.FruitWatermelon,
        ResourceId.FruitCherry,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillStick;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillStick = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillStick += value;
    }

}
