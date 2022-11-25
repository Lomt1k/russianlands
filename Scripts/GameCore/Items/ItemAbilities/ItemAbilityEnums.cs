
namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public enum AbilityType : short
    {
        None = 0,
        DealDamage = 1,
        BlockIncomingDamageEveryTurn = 2,
        RestoreHealthEveryTurn = 3,
        AddManaEveryTurn = 4
    }

    public enum ActivationType : byte
    {
        None = 0,
        ByUser = 1,
        EveryTurn = 2        
    }


}
