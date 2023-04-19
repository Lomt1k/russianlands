using TextGameRPG.Scripts.GameCore.Services.MobGenerator;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Services
{
    public class MobFactory : Service
    {
        public MobData GenerateMobForDebugBattle(byte playerLevel)
        {
            return new MobDataBuilder(playerLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(playerLevel - 1, playerLevel + 1)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(playerLevel - 1, playerLevel + 1)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName()
                .GetResult();
        }

    }
}
