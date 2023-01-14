using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public enum Avatar : short
    {
        Absctract = 0,

        Male,
        MaleB,
        MaleC,
        MaleD,
        MaleE,
        MaleF,
        MaleG,
        MaleH,
        MaleI,

        Female,
        FemaleB,
        FemaleC,
        FemaleD,
        FemaleE,
        FemaleF,
        FemaleG,
        FemaleH,
        FemaleI,

        BuilderA,
        BuilderB,
    }

    public static class AvatarExtensions
    {
        public static Emoji GetEmoji(this Avatar avatar)
        {
            return avatar switch
            {
                Avatar.Absctract => Emojis.AvatarAbstract,

                Avatar.Male => Emojis.AvatarMale,
                Avatar.MaleB => Emojis.AvatarMaleB,
                Avatar.MaleC => Emojis.AvatarMaleC,
                Avatar.MaleD => Emojis.AvatarMaleD,
                Avatar.MaleE => Emojis.AvatarMaleE,
                Avatar.MaleF => Emojis.AvatarMaleF,
                Avatar.MaleG => Emojis.AvatarMaleG,
                Avatar.MaleH => Emojis.AvatarMaleH,
                Avatar.MaleI => Emojis.AvatarMaleI,

                Avatar.Female => Emojis.AvatarFemale,
                Avatar.FemaleB => Emojis.AvatarFemaleB,
                Avatar.FemaleC => Emojis.AvatarFemaleC,
                Avatar.FemaleD => Emojis.AvatarFemaleD,
                Avatar.FemaleE => Emojis.AvatarFemaleE,
                Avatar.FemaleF => Emojis.AvatarFemaleF,
                Avatar.FemaleG => Emojis.AvatarFemaleG,
                Avatar.FemaleH => Emojis.AvatarFemaleH,
                Avatar.FemaleI => Emojis.AvatarFemaleI,

                Avatar.BuilderA => Emojis.AvatarBuilderA,
                Avatar.BuilderB => Emojis.AvatarBuilderB,

                _ => Emojis.AvatarAbstract
            };
        }
    }

}
