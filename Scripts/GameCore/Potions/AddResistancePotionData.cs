using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class AddResistancePotionData : PotionData
    {
        public int physicalDamage;
        public int fireDamage;
        public int coldDamage;
        public int lightningDamage;
        public int turnsCount;

        public AddResistancePotionData(int _id) : base(_id)
        {
        }

        public DamageInfo GetValues(GameSession session)
        {
            //TODO: Учитывать защитные навыки игрока?
            return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        }

        public override string GetView(GameSession session)
        {
            //TODO
            return string.Empty;
        }
    }
}
