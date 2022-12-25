using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class AddDamagePotionData : PotionData
    {
        public int physicalDamage;
        public int fireDamage;
        public int coldDamage;
        public int lightningDamage;

        public AddDamagePotionData(int _id) : base(_id)
        {
        }

        public DamageInfo GetValues(GameSession session)
        {
            //TODO: Учитывать атакующие навыки игрока?
            return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
        }

        public override string GetView(GameSession session)
        {
            //TODO
            return string.Empty;
        }
    }
}
