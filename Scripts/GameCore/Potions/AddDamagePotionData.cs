using Newtonsoft.Json;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class AddDamagePotionData : PotionData
    {
        public int physicalDamage { get; set; }
        public int fireDamage { get; set; }
        public int coldDamage { get; set; }
        public int lightningDamage { get; set; }

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
