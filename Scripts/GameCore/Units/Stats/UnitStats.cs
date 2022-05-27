
namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public abstract class UnitStats
    {
        private int _maxHP;
        private int _maxMP;

        public int maxHP 
        {
            get => _maxHP;
            protected set
            {
                _maxHP = value;
                if (currentHP > value)
                {
                    currentHP = value;
                }
            }
        }
        public int maxMP
        {
            get => _maxMP;
            protected set
            {
                _maxMP = value;
                if (currentMP > value)
                {
                    currentMP = value;
                }
            }
        }

        public int currentHP { get; protected set; }
        public int currentMP { get; protected set; }
        public int physicalResist { get; protected set; }
        public int fireResist { get; protected set; }
        public int coldResist { get; protected set; }
        public int lightningResist { get; protected set; }

        public void SetFullHealth()
        {
            currentHP = maxHP;
        }

        public void SetFullMana()
        {
            currentMP = maxMP;
        }

    }
}
