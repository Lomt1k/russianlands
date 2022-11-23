﻿using Newtonsoft.Json;
using JsonKnownTypes;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    [JsonConverter(typeof(JsonKnownTypesConverter<ItemAbilityBase>))]
    public abstract class ItemAbilityBase
    {
        [JsonIgnore] public abstract string debugDescription { get; }
        [JsonIgnore] public abstract AbilityType abilityType { get; }
        [JsonIgnore] public abstract ActivationType activationType { get; }

        public float chanceToSuccessPercentage = 100;
        public int manaCost;

        public ItemAbilityBase Clone()
        {
            return (ItemAbilityBase)MemberwiseClone();
        }

        public bool TryChance()
        {
            return Randomizer.TryPercentage(chanceToSuccessPercentage);
        }

        public abstract string GetView(GameSession session);

    }
}
