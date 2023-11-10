using MarkOne.Scripts.Bot;

namespace MarkOne.Scripts.GameCore.Units;

public enum AvatarId : ushort
{
    Male_00 = 0,
    Male_01 = 1,
    Male_02 = 2,
    Male_03 = 3,
    Male_04 = 4,
    Male_05 = 5,
    Male_06 = 6,
    Male_07 = 7,
    Male_08 = 8,

    Female_00 = 9,
    Female_01 = 10,
    Female_02 = 11,
    Female_03 = 12,
    Female_04 = 13,
    Female_05 = 14,
    Female_06 = 15,
    Female_07 = 16,
    Female_08 = 17,

    BuilderA = 18,
    BuilderB = 19,
    SmirkCat = 20,
}

public static class Avatars
{
    public static Emoji GetEmoji(this AvatarId? avatar)
    {
        return avatar is null
            ? Emojis.AvatarAbstract
            : GetEmoji((AvatarId)avatar);

    }

    public static Emoji GetEmoji(this AvatarId avatar)
    {
        return avatar switch
        {
            AvatarId.Male_00 => Emojis.AvatarMale,
            AvatarId.Male_01 => Emojis.AvatarMaleB,
            AvatarId.Male_02 => Emojis.AvatarMaleC,
            AvatarId.Male_03 => Emojis.AvatarMaleD,
            AvatarId.Male_04 => Emojis.AvatarMaleE,
            AvatarId.Male_05 => Emojis.AvatarMaleF,
            AvatarId.Male_06 => Emojis.AvatarMaleG,
            AvatarId.Male_07 => Emojis.AvatarMaleH,
            AvatarId.Male_08 => Emojis.AvatarMaleI,

            AvatarId.Female_00 => Emojis.AvatarFemale,
            AvatarId.Female_01 => Emojis.AvatarFemaleB,
            AvatarId.Female_02 => Emojis.AvatarFemaleC,
            AvatarId.Female_03 => Emojis.AvatarFemaleD,
            AvatarId.Female_04 => Emojis.AvatarFemaleE,
            AvatarId.Female_05 => Emojis.AvatarFemaleF,
            AvatarId.Female_06 => Emojis.AvatarFemaleG,
            AvatarId.Female_07 => Emojis.AvatarFemaleH,
            AvatarId.Female_08 => Emojis.AvatarFemaleI,

            AvatarId.BuilderA => Emojis.AvatarBuilderA,
            AvatarId.BuilderB => Emojis.AvatarBuilderB,
            AvatarId.SmirkCat => Emojis.AvatarSmirkCat,

            _ => Emojis.AvatarAbstract
        };
    }

    public static readonly AvatarId[] defaultAvatars = new AvatarId[]
    {
        AvatarId.Male_00,
        AvatarId.Male_01,
        AvatarId.Male_02,
        AvatarId.Male_03,
        AvatarId.Male_04,
        AvatarId.Male_05,
        AvatarId.Male_06,
        AvatarId.Male_07,
        AvatarId.Male_08,

        AvatarId.Female_00,
        AvatarId.Female_01,
        AvatarId.Female_02,
        AvatarId.Female_03,
        AvatarId.Female_04,
        AvatarId.Female_05,
        AvatarId.Female_06,
        AvatarId.Female_07,
        AvatarId.Female_08,
    };

    public static readonly AvatarId[] fakeMaleAvatars = new AvatarId[]
    {
        AvatarId.Male_00,
        AvatarId.Male_01,
        AvatarId.Male_02,
        AvatarId.Male_03,
        AvatarId.Male_04,
        AvatarId.Male_05,
        AvatarId.Male_06,
        AvatarId.Male_07,
        AvatarId.Male_08,
    };

    public static readonly AvatarId[] fakeFemaleAvatars = new AvatarId[]
    {
        AvatarId.Female_00,
        AvatarId.Female_01,
        AvatarId.Female_02,
        AvatarId.Female_03,
        AvatarId.Female_04,
        AvatarId.Female_05,
        AvatarId.Female_06,
        AvatarId.Female_07,
        AvatarId.Female_08,
    };

}
