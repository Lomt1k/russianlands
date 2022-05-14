using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Mob : IBattleUnit
    {
        public UnitStats unitStats { get; }

        public string nickname => throw new System.NotImplementedException();

        public Mob()
        {
            unitStats = new MobStats();
        }
    }
}
