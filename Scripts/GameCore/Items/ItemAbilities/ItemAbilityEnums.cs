
namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public enum AbilityType : short
    {
        None = 0,

        // --- Абилки, которые активируются при применении самого предмета
        DealDamage = 1,
        RestoreHealth = 2,

        // --- Абилки, которые активируются каждый ход (когда просто экипированы)
        BlockIncomingDamageEveryTurn = 100,
        RestoreHealthEveryTurn = 101,
        AddManaEveryTurn = 102
    }

    public enum ActivationType : byte
    {
        None = 0,
        ByUser = 1,
        EveryTurn = 2        
    }


}
