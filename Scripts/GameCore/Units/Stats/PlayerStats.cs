using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

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
        }

        public void SubscribeEvents()
        {
            _player.inventory.equipped.onUpdateEquippedItems += Recalculate;
        }

        public override void OnStartBattle()
        {
            base.OnStartBattle();
            currentArrows = 3; //TODO: Add Arrows Calculation Logic
        }

        public override void OnStartMineTurn()
        {
            base.OnStartMineTurn();
            var equippedStick = _player.inventory.equipped[Items.ItemType.Stick];
            if (equippedStick != null && currentStickCharge < equippedStick.data.requiredCharge)
            {
                currentStickCharge++;
            }
        }

        public void Recalculate()
        {
            CalculateBaseValues();
            ApplyItemProperties();
            ApplyAttributes();

            currentHP = currentHP > maxHP ? maxHP : currentHP;
        }

        private void CalculateBaseValues()
        {
            var profileData = _player.session.profile.data;

            var defaultHealth = DEFAULT_HEALTH + HEALTH_AND_MANA_PER_LEVEL * (profileData.level - 1);
            maxHP = defaultHealth;

            attributeStrength = 1;
            attributeVitality = 1;
            attributeSorcery = 1;
            attributeLuck = 1;

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
            }
        }

        private void ApplyAttributes()
        {
            var physicalBonusPerVitality = (float)physicalResist / 100;
            physicalResist += (int)(physicalBonusPerVitality * attributeVitality);

            var fireBonusPerVitality = (float)fireResist / 100;
            fireResist += (int)(fireBonusPerVitality * attributeVitality);

            var coldBonusPerVitality = (float)coldResist / 100;
            coldResist += (int)(coldBonusPerVitality * attributeVitality);

            var lightningBonusPerVitality = (float)lightningResist / 100;
            lightningResist += (int)(lightningBonusPerVitality * attributeVitality);
        }

        public override string GetView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            
            sb.Append("<b>" + Localization.Get(sessionToSend, "unit_attribute_strength") + ":</b> " + attributeStrength);
            sb.AppendLine(Emojis.bigSpace + "<b>" + Localization.Get(sessionToSend, "unit_attribute_vitality") + ":</b> " + attributeVitality);
            sb.Append("<b>" + Localization.Get(sessionToSend, "unit_attribute_sorcery") + ":</b> " + attributeSorcery);
            sb.AppendLine(Emojis.bigSpace + "<b>" + Localization.Get(sessionToSend, "unit_attribute_luck") + ":</b> " + attributeLuck);

            sb.AppendLine();
            sb.AppendLine($"{Emojis.stats[Stat.Health]} {currentHP} / {maxHP}");

            sb.AppendLine();
            AppendResistsCompactView(sb, sessionToSend);

            return sb.ToString();
        }

    }
}
