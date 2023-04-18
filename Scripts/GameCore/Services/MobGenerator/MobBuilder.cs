using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.MobGenerator
{
    public partial class MobBuilder
    {
        private const float healthByPlayerHealthQuotient = 1.5f;

        private static readonly GameDataHolder gameDataHolder = Services.Get<GameDataHolder>();
        private static IEnumerable<MobData> questMobs => gameDataHolder.mobs.GetAllData();

        private MobData _mobData;

        public MobBuilder(int mobLevel)
        {
            _mobData = new MobData()
            {
                id = -1,
                debugName = string.Empty,
            };
            var playerHealth = PlayerHealthByLevel.Get(mobLevel);
            _mobData.statsSettings.health = (int)(playerHealth * healthByPlayerHealthQuotient);
            _mobData.statsSettings.level = mobLevel;
        }

        public MobBuilder RandomizeHealthByPercents(byte percents)
        {
            RandomizeByPercents(ref _mobData.statsSettings.health, percents, roundBy: 10);
            return this;
        }

        public MobBuilder SetRandomVisualLevel(int minLevel, int maxLevel)
        {
            _mobData.statsSettings.level = new Random().Next(minLevel, maxLevel + 1);
            return this;
        }

        public MobBuilder CopyResistanceFromQuestMob(int minLevel, int maxLevel)
        {
            var mobs = questMobs.Where(x => x.statsSettings.level >= minLevel && x.statsSettings.level <= maxLevel).ToArray();
            var index = new Random().Next(mobs.Length);
            var exampleData = mobs[index];

            _mobData.statsSettings.physicalResist = exampleData.statsSettings.physicalResist;
            _mobData.statsSettings.fireResist = exampleData.statsSettings.fireResist;
            _mobData.statsSettings.coldResist = exampleData.statsSettings.coldResist;
            _mobData.statsSettings.lightningResist = exampleData.statsSettings.lightningResist;

            return this;
        }

        public MobBuilder ShuffleResistanceValues()
        {
            var availableValues = new List<int>()
            {
                _mobData.statsSettings.fireResist,
                _mobData.statsSettings.coldResist,
                _mobData.statsSettings.lightningResist
            };

            var random = new Random();
            var randomIndex = random.Next(availableValues.Count);
            _mobData.statsSettings.fireResist = availableValues[randomIndex];
            availableValues.RemoveAt(randomIndex);

            randomIndex = random.Next(availableValues.Count);
            _mobData.statsSettings.coldResist = availableValues[randomIndex];
            availableValues.RemoveAt(randomIndex);

            _mobData.statsSettings.lightningResist = availableValues[0];
            return this;
        }

        public MobBuilder RandomizeResistanceByPercents(byte percents)
        {
            RandomizeByPercents(ref _mobData.statsSettings.physicalResist, percents, roundBy: 5);
            RandomizeByPercents(ref _mobData.statsSettings.fireResist, percents, roundBy: 5);
            RandomizeByPercents(ref _mobData.statsSettings.coldResist, percents, roundBy: 5);
            RandomizeByPercents(ref _mobData.statsSettings.lightningResist, percents, roundBy: 5);
            return this;
        }

        public MobBuilder CopyAttacksFromQuestMob(int minLevel, int maxLevel)
        {
            var mobs = questMobs.Where(x => x.statsSettings.level >= minLevel && x.statsSettings.level <= maxLevel).ToArray();
            var index = new Random().Next(mobs.Length);
            var exampleData = mobs[index];

            foreach (var mobAttack in exampleData.mobAttacks)
            {
                _mobData.mobAttacks.Add(mobAttack.CloneForMobBuilder());
            }

            return this;
        }

        public MobBuilder ShuffleDamageValues()
        {
            var random = new Random();
            foreach (var attack in _mobData.mobAttacks)
            {
                var availableValues = new List<(int min, int max)>();
                availableValues.Add((attack.minFireDamage, attack.maxFireDamage));
                availableValues.Add((attack.minColdDamage, attack.maxColdDamage));
                availableValues.Add((attack.minLightningDamage, attack.maxLightningDamage));
                
                var randomIndex = random.Next(availableValues.Count);
                attack.minFireDamage = availableValues[randomIndex].min;
                attack.maxFireDamage = availableValues[randomIndex].max;
                availableValues.RemoveAt(randomIndex);

                randomIndex = random.Next(availableValues.Count);
                attack.minColdDamage = availableValues[randomIndex].min;
                attack.maxColdDamage = availableValues[randomIndex].max;
                availableValues.RemoveAt(randomIndex);

                attack.minLightningDamage = availableValues[0].min;
                attack.maxLightningDamage = availableValues[0].max;
            }                
            return this;
        }

        public MobBuilder RandomizeDamageValuesByPercents(byte percents)
        {
            foreach (var attack in _mobData.mobAttacks)
            {
                RandomizeByPercents(ref attack.minPhysicalDamage, percents);
                RandomizeByPercents(ref attack.maxPhysicalDamage, percents);
                RandomizeByPercents(ref attack.minFireDamage, percents);
                RandomizeByPercents(ref attack.maxFireDamage, percents);
                RandomizeByPercents(ref attack.minColdDamage, percents);
                RandomizeByPercents(ref attack.maxColdDamage, percents);
                RandomizeByPercents(ref attack.minLightningDamage, percents);
                RandomizeByPercents(ref attack.maxLightningDamage, percents);

                if (attack.minPhysicalDamage > attack.maxPhysicalDamage)
                {
                    Swap(ref attack.minPhysicalDamage, ref attack.maxPhysicalDamage);
                }
                if (attack.minFireDamage > attack.maxFireDamage)
                {
                    Swap(ref attack.minFireDamage, ref attack.maxFireDamage);
                }
                if (attack.minColdDamage > attack.maxColdDamage)
                {
                    Swap(ref attack.minColdDamage, ref attack.maxColdDamage);
                }
                if (attack.minLightningDamage > attack.maxLightningDamage)
                {
                    Swap(ref attack.minLightningDamage, ref attack.maxLightningDamage);
                }
            }
            return this;
        }

        public MobData GetResult()
        {
            return _mobData;
        }




        private void RandomizeByPercents(ref int value, byte percents, int roundBy = 0)
        {
            var randomDistance = value * percents / 100;
            if (randomDistance < 0)
                return;

            var delta = new Random().Next(randomDistance) - (randomDistance / 2);
            value += delta;

            if (roundBy > 0)
            {
                var remainder = value % roundBy;
                value = remainder < roundBy / 2 ? value - remainder : value + (roundBy - remainder);
            }
        }

        private void Swap(ref int a, ref int b)
        {
            var temp = a;
            a = b;
            b = temp;
        }



    }
}
