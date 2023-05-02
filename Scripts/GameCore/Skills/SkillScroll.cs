using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Skills;

internal class SkillScroll : ISkill
{
    public ItemType itemType => ItemType.Scroll;
    public ResourceId[] requiredFruits => new ResourceId[]
    {
        ResourceId.FruitPear,
        ResourceId.FruitStrawberry,
        ResourceId.FruitKiwi,
    };

    public byte GetValue(ProfileData profileData)
    {
        return profileData.skillScroll;
    }

    public void SetValue(ProfileData profileData, byte value)
    {
        profileData.skillScroll = value;
    }

    public void AddValue(ProfileData profileData, byte value)
    {
        profileData.skillScroll += value;
    }

}
