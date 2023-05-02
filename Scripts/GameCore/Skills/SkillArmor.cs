using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Skills;

internal class SkillArmor : ISkill
{
    public ItemType itemType => ItemType.Armor;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitMandarin,
        ResourceId.FruitPineapple,
        ResourceId.FruitGrape,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillArmor;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillArmor = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillArmor += value;
    }

}
