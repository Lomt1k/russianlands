
namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public class PlayerStats : UnitStats
    {
        private const int DEFAULT_HEALTH = 100;
        private const int HEALTH_AND_MANA_PER_LEVEL = 15;

        private Player _player;

        public PlayerStats(Player player)
        {
            _player = player;
            Recalculate();

            SetFullHealth();
            SetFullMana();
        }

        public void Recalculate()
        {
            CalculateBaseValues();
        }

        private void CalculateBaseValues()
        {
            var playerLevel = _player.session.profile.data.level;
            var defaultHealthAndMana = DEFAULT_HEALTH + HEALTH_AND_MANA_PER_LEVEL * (playerLevel - 1);
            maxHP = defaultHealthAndMana;
            maxMP = defaultHealthAndMana;
        }


    }
}
