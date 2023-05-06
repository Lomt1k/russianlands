using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.Mobs;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Mobs;

namespace MarkOne.Scripts.GameCore.Commands.Cheats;

public class BattleCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    private static readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();
    private static readonly MobFactory mobFactory = ServiceLocator.Get<MobFactory>();

    public override async Task Execute(GameSession session, string[] args)
    {
        var currentBattle = battleManager.GetCurrentBattle(session.player);
        if (currentBattle != null)
            return;

        await new SimpleDialog(session, "Select an enemy", true, new Dictionary<string, Func<Task>>()
        {
            { "Dummy", () => StartBattleWithDummy(session.player) },
            { "Generated Mob",  () => StartBattleWithGeneratedMob(session.player) },
            { "Shadow Copy", () => StartBattleWithShadowCopy(session.player) },
        })
        .Start().FastAwait();
    }

    private Task StartBattleWithDummy(Player player)
    {
        var mobData = new QuestMobData();
        mobData.localizationKey = "Dummy";
        mobData.statsSettings.health = 100_000;
        mobData.mobAttacks.Add(new MobAttack());
        battleManager.StartBattle(player, mobData);
        return Task.CompletedTask;
    }

    private Task StartBattleWithGeneratedMob(Player player)
    {
        var mobData = mobFactory.GenerateMobForDebugBattle(player.level);
        battleManager.StartBattle(player, mobData);
        return Task.CompletedTask;
    }

    private Task StartBattleWithShadowCopy(Player player)
    {
        var items = player.inventory.equipped.allEquipped;
        var skills = player.skills.GetAllSkills();
        var fakePlayer = new FakePlayer(items, skills, player.level, "Shadow Copy", player.session.profile.data.IsPremiumActive());
        battleManager.StartBattle(player, fakePlayer);
        return Task.CompletedTask;
    }



    private MobAttack CreateMobAttackFromItem(Player player, InventoryItem item)
    {
        var dealDamage = (DealDamageAbility)item.data.ablitityByType[AbilityType.DealDamage];
        if (dealDamage == null)
        {
            return new MobAttack();
        }

        var attack = new MobAttack()
        {
            localizationKey = item.GetFullName(player.session).Bold(),
            minPhysicalDamage = dealDamage.minPhysicalDamage,
            maxPhysicalDamage = dealDamage.maxPhysicalDamage,
            minFireDamage = dealDamage.minFireDamage,
            maxFireDamage = dealDamage.maxFireDamage,
            minColdDamage = dealDamage.minColdDamage,
            maxColdDamage = dealDamage.maxColdDamage,
            minLightningDamage = dealDamage.minLightningDamage,
            maxLightningDamage = dealDamage.maxLightningDamage,
            manaCost = item.data.itemType == ItemType.Stick ? (sbyte)InventoryItem.requiredStickCharge : item.manaCost,
        };

        return attack;
    }

}
