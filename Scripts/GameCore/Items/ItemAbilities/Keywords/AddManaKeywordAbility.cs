﻿using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords
{
    public class AddManaKeywordAbility : ItemAbilityBase
    {
        public override string debugDescription => "Даёт дополнительное очко маны";
        public override AbilityType abilityType => AbilityType.AddManaKeyword;

        public override string GetView(GameSession session)
        {
            return $"{Emojis.stats[Stat.Mana]} {Localization.Get(session, "ability_add_mana_percentage", chanceToSuccessPercentage)}";
        }
    }
}
