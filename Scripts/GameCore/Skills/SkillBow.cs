using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills;

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
