
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
                case PhysicalDamageResistItemProperty physicalDamageResist:
                    this.physicalResist += physicalDamageResist.value;
                    break;
                case FireDamageResistItemProperty fireDamageResist:
                    this.fireResist += fireDamageResist.value;
                    break;
                case ColdDamageResistItemProperty coldDamageResist:
                    this.coldResist += coldDamageResist.value;
                    break;
                case LightningDamageResistItemProperty lightningDamageResist:
                    this.lightningResist += lightningDamageResist.value;
                    break;
            }
        }


    }
}
