using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public interface IBattleUnit
    {
        public string nickname { get; }
        public UnitStats unitStats { get; }
    }
}
