using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    public partial class MobDataBuilder
    {
        private const float healthByPlayerHealthQuotient = 1.5f;

        private static readonly GameDataHolder gameDataHolder = Services.Get<GameDataHolder>();
        private static IEnumerable<MobData> questMobs => gameDataHolder.mobs.GetAllData();

        private MobData _mobData;

        public MobDataBuilder(int mobLevel)
        {
            CreateDefaultMobData(mobLevel);
        }

        public MobDataBuilder(int minLevel, int maxLevel)
        {
            var mobLevel = new Random().Next(minLevel, maxLevel + 1);
            CreateDefaultMobData(mobLevel);
        }

        private void CreateDefaultMobData(int mobLevel)
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

        public MobDataBuilder RandomizeHealthByPercents(byte percents)
        {
            RandomizeByPercents(ref _mobData.statsSettings.health, percents, roundBy: 10);
            return this;
        }

        public MobDataBuilder SetRandomVisualLevel(int minLevel, int maxLevel)
        {
            _mobData.statsSettings.level = new Random().Next(minLevel, maxLevel + 1);
            return this;
        }

        public MobDataBuilder CopyResistanceFromQuestMob(int minLevel, int maxLevel)
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

        public MobDataBuilder ShuffleResistanceValues()
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

        public MobDataBuilder RandomizeResistanceByPercents(byte percents)
        {
            RandomizeByPercents(ref _mobData.statsSettings.physicalResist, percents, roundBy: 5);
            RandomizeByPercents(ref _mobData.statsSettings.fireResist, percents, roundBy: 5);
            RandomizeByPercents(ref _mobData.statsSettings.coldResist, percents, roundBy: 5);
            RandomizeByPercents(ref _mobData.statsSettings.lightningResist, percents, roundBy: 5);
            return this;
        }

        public MobDataBuilder CopyAttacksFromQuestMob(int minLevel, int maxLevel)
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

        public MobDataBuilder ShuffleDamageValues()
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
            FixSimilarAttacksAfterShuffle();
            return this;
        }

        // Исправляет ситуацию, когда после шафла появляется две очень похожие атаки (например, обе атаки на урон холодом с похожим диапозоном урона)
        private void FixSimilarAttacksAfterShuffle()
        {
            if (_mobData.mobAttacks.Count < 2)
                return;

            var attacksForDelete = new HashSet<MobAttack>();
            for (int i = 0; i < _mobData.mobAttacks.Count - 1; i++)
            {
                for (int j = 1; j < _mobData.mobAttacks.Count; j++)
                {
                    if (IsSimilarAttacks(_mobData.mobAttacks[i], _mobData.mobAttacks[j]))
                    {
                        attacksForDelete.Add(_mobData.mobAttacks[j]);
                    }
                }
            }

            foreach (var attack in attacksForDelete)
            {
                _mobData.mobAttacks.Remove(attack);
            }
        }

        private bool IsSimilarAttacks(MobAttack a, MobAttack b)
        {
            if (a.manaCost != b.manaCost)
                return false;

            // similar fire
            if (a.minFireDamage > 0 && b.minFireDamage > 0
                && a.minColdDamage == 0 && b.minColdDamage == 0
                && a.minLightningDamage == 0 && b.minLightningDamage == 0)
            {
                return true;
            }

            // similar cold
            if (a.minColdDamage > 0 && b.minColdDamage > 0
                && a.minFireDamage == 0 && b.minFireDamage == 0
                && a.minLightningDamage == 0 && b.minLightningDamage == 0)
            {
                return true;
            }

            // similar lightning
            if (a.minLightningDamage > 0 && b.minLightningDamage > 0
                && a.minFireDamage == 0 && b.minFireDamage == 0
                && a.minColdDamage == 0 && b.minColdDamage == 0)
            {
                return true;
            }

            return false;
        }

        public MobDataBuilder RandomizeDamageValuesByPercents(byte percents)
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
