using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    internal class LightningDamageItemProperty : ItemPropertyBase
    {
        public override string debugDescription => "Урон молнией";
        public override ItemPropertyType propertyType => ItemPropertyType.LightningDamage;

        public int minDamage;
        public int maxDamage;

        public LightningDamageItemProperty(int minDamage, int maxDamage)
        {
            this.minDamage = minDamage;
            this.maxDamage = maxDamage;
        }

        public int GetRandomValue()
        {
            return Randomizer.random.Next(minDamage, maxDamage + 1);
        }

        public override string ToString()
        {
            return $"{debugDescription}: {GetStringValue()}";
        }

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.LightningDamage]} {GetStringValue()}";
        }

        private string GetStringValue()
        {
            return minDamage == maxDamage ? minDamage.ToString() : $"{minDamage} - {maxDamage}";
        }

    }
}
