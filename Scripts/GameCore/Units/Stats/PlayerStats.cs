
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public class PlayerStats : UnitStats
    {
        private const int DEFAULT_HEALTH = 100;
        private const int HEALTH_AND_MANA_PER_LEVEL = 15;

        private Player _player;

        // Attributes
        public int attributeStrength { get; protected set; }
        public int attributeVitality { get; protected set; }
        public int attributeSorcery { get; protected set; }
        public int attributeLuck { get; protected set; }

        public PlayerStats(Player player)
        {
            _player = player;
            SubscribeEvents();
            Recalculate();

            SetFullHealth();
            SetFullMana();
        }

        public void SubscribeEvents()
        {
            _player.inventory.equipped.onUpdateEquippedItems += Recalculate;
        }

        public void Recalculate()
        {
            CalculateBaseValues();
            ApplyItemProperties();

            currentHP = currentHP > maxHP ? maxHP : currentHP;
            currentMP = currentMP > maxMP ? maxMP : currentMP;
        }

        private void CalculateBaseValues()
        {
            var profileData = _player.session.profile.data;

            var defaultHealthAndMana = DEFAULT_HEALTH + HEALTH_AND_MANA_PER_LEVEL * (profileData.level - 1);
            maxHP = defaultHealthAndMana;
            maxMP = defaultHealthAndMana;

            attributeStrength = profileData.attributeStrength;
            attributeVitality = profileData.attributeVitality;
            attributeSorcery = profileData.attributeSorcery;
            attributeLuck = profileData.attributeLuck;

            physicalResist = 0;
            fireResist = 0;
            coldResist = 0;
            lightningResist = 0;
        }

        private void ApplyItemProperties()
        {
            var equippedItems = _player.inventory.equipped.allEquipped;
            foreach (var item in equippedItems)
            {
                foreach (var property in item.data.properties)
                {
                    ApplyProperty(property);
                }
            }
        }

        private void ApplyProperty(ItemPropertyBase property)
        {
            switch (property)
            {
                case DamageResistProperty resistProperty:
                    this.physicalResist += resistProperty.physicalDamage;
                    this.fireResist += resistProperty.fireDamage;
                    this.coldResist += resistProperty.coldDamage;
                    this.lightningResist += resistProperty.lightningDamage;
                    break;
                case IncreaseAttributeStrengthProperty increaseStrength:
                    this.attributeStrength += increaseStrength.value;
                    break;
                case IncreaseAttributeVitalityProperty increaseVitality:
                    this.attributeVitality += increaseVitality.value;
                    break;
                case IncreaseAttributeSorceryProperty increaseSorcery:
                    this.attributeSorcery += increaseSorcery.value;
                    break;
                case IncreaseAttributeLuckProperty increaseLuck:
                    this.attributeLuck += increaseLuck.value;
                    break;
                case IncreaseMaxHealthProperty increaseMaxHealth:
                    this.maxHP += increaseMaxHealth.value;
                    break;
                case IncreaseMaxManaProperty increaseMaxMana:
                    this.maxMP += increaseMaxMana.value;
                    break;
            }
        }


    }
}
