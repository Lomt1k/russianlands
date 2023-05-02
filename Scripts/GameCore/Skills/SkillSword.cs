using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Skills;

internal class SkillSword : ISkill
{
    public ItemType itemType => ItemType.Sword;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitApple,
        ResourceId.FruitPineapple,
        ResourceId.FruitBlueberry,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillSword;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillSword = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillSword += value;
    }

}
