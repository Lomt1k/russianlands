using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public class HealthRestorePotionData : PotionData
    {
        public int healthAmount;

        public HealthRestorePotionData(int _id) : base(_id)
        {
        }

        public override string GetView(GameSession session)
        {
            //TODO
            return string.Empty;
        }
    }
}
