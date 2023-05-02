using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Skills;

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
