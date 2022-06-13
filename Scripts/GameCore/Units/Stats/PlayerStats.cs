using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot;

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

        public override string GetView()
        {
            var sb = new StringBuilder();
            
            sb.Append("<b>" + Localization.Get(_player.session, "unit_attribute_strength") + ":</b> " + attributeStrength);
            sb.AppendLine(Emojis.bigSpace + "<b>" + Localization.Get(_player.session, "unit_attribute_vitality") + ":</b> " + attributeVitality);
            sb.Append("<b>" + Localization.Get(_player.session, "unit_attribute_sorcery") + ":</b> " + attributeSorcery);
            sb.Append(Emojis.bigSpace + "<b>" + Localization.Get(_player.session, "unit_attribute_luck") + ":</b> " + attributeLuck);

            sb.AppendLine();
            sb.AppendLine($"\n{Emojis.stats[Stat.Health]} {currentHP} / {maxHP}");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(_player.session, "unit_view_total_resistance"));
            AppendResistsCompactView(sb);

            return sb.ToString();
        }

        private void AppendResistsCompactView(StringBuilder sb)
        {
            bool hasBigValues = physicalResist > 999 
                || fireResist > 999
                || coldResist > 999
                || lightningResist > 999;

            if (hasBigValues)
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} " + physicalResist);
                sb.AppendLine(Emojis.bigSpace + $"{Emojis.stats[Stat.FireDamage]} " + fireResist);
                sb.Append($"{Emojis.stats[Stat.ColdDamage]} " + coldResist);
                sb.AppendLine(Emojis.bigSpace + $"{Emojis.stats[Stat.LightningDamage]} " + lightningResist);
            }
            else
            {
                sb.Append($"{Emojis.stats[Stat.PhysicalDamage]} " + physicalResist);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.FireDamage]} " + fireResist);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.ColdDamage]} " + coldResist);
                sb.Append(Emojis.middleSpace + $"{Emojis.stats[Stat.LightningDamage]} " + lightningResist);
            }
        }

    }
}
