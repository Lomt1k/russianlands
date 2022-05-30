using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class FireDamageResistItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону от огня";
        public override ItemPropertyType propertyType => ItemPropertyType.FireDamageResist;

        public int value;

        public FireDamageResistItemProperty(int value)
        {
            this.value = value;
        }

        public override void ApplyItemLevel(byte level)
        {
            float bonusPerLevel = value / 10 > 0 ? (float)value / 10 : 1;
            value += (int)(bonusPerLevel * level);
        }

        public override string ToString()
        {
            return $"{debugDescription}: {value}";
        }

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.FireDamage]} {value}";
        }

    }
}
