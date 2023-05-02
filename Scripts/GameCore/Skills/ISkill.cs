using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Skills;

public interface ISkill
{
    public ItemType itemType { get; }
    public ResourceId[] requiredFruits { get; }

    byte GetValue(ProfileData profileData);
    void SetValue(ProfileData profileData, byte value);
    void AddValue(ProfileData profileData, byte value);
}
