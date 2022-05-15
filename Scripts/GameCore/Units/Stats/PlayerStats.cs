
namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public class PlayerStats : UnitStats
    {
        private const int DEFAULT_HEALTH = 100;
        private const int HEALTH_AND_MANA_PER_LEVEL = 15;

        private Player _player;

        public int currentStrength { get; protected set; }
        public int currentVitality { get; protected set; }
        public int currentSorcery { get; protected set; }
        public int currentLuck { get; protected set; }

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
            var profileData = _player.session.profile.data;

            var defaultHealthAndMana = DEFAULT_HEALTH + HEALTH_AND_MANA_PER_LEVEL * (profileData.level - 1);
            maxHP = defaultHealthAndMana;
            maxMP = defaultHealthAndMana;

            currentStrength = profileData.attributeStrength;
            currentVitality = profileData.attributeVitality;
            currentSorcery = profileData.attributeSorcery;
            currentLuck = profileData.attributeLuck;
        }


    }
}
