using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills;

internal class SkillHelmet : ISkill
{
    public ItemType itemType => ItemType.Helmet;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitCoconut,
        ResourceId.FruitBanana,
        ResourceId.FruitGrape,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillHelmet;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillHelmet = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillHelmet += value;
    }

}
