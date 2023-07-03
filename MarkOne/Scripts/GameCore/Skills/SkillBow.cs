using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Skills;

internal class SkillBow : ISkill
{
    public ItemType itemType => ItemType.Bow;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitApple,
        ResourceId.FruitBanana,
        ResourceId.FruitKiwi,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillBow;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillBow = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillBow += value;
    }

}
