
namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public abstract class UnitStats
    {

        public int maxHP { get; protected set; }
        public int currentHP { get; protected set; }
        public int currentMana { get; protected set; }
        public int physicalResist { get; protected set; }
        public int fireResist { get; protected set; }
        public int coldResist { get; protected set; }
        public int lightningResist { get; protected set; }

        public void SetFullHealth()
        {
            currentHP = maxHP;
        }

        public abstract string GetView();

    }
}
