using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class ColdDamageResistItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Сопротивление урону от холода";
        public override ItemPropertyType propertyType => ItemPropertyType.ColdDamageResist;

        public int value;

        public ColdDamageResistItemProperty(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return $"{debugDescription}: {value}";
        }

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.ColdDamage]} {value}";
        }

    }
}
