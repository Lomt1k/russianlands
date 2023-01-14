using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    public class PlayerAttackAction : IBattleAction
    {
        public DamageInfo damageInfo;

        private readonly InventoryItem? _item;        
        private DamageInfo _resultDamage;

        /// <summary>
        /// Конструктор для атаки мечом / луком / посохом / свитком
        /// если item == null, то это атака кулаком
        /// </summary>
        public PlayerAttackAction(Player attacker, InventoryItem? item)
        {
            _item = item;
            damageInfo = item != null && item.data.ablitityByType.TryGetValue(AbilityType.DealDamage, out var dealDamageAbility)
                ? ((DealDamageAbility)dealDamageAbility).GetRandomValues()
                : new DamageInfo(10, 0, 0, 0); // Урон кулаком: 10 единиц (меньше любого оружия)
        }

        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
            _resultDamage = stats.TryDealDamage(damageInfo);
        }

        public string GetHeader(GameSession session)
        {
            var itemName = _item != null
                ? _item.GetFullName(session)
                : $"{Emojis.stats[Stat.PhysicalDamage]} {Localization.Get(session, "battle_attack_fists")}";

            return $"<b>{itemName}</b>";
        }

        public string GetDescription(GameSession session)
        {
            var sb = new StringBuilder();
            var totalDamage = _resultDamage.GetTotalValue();
            sb.Append(Localization.Get(session, "battle_action_attack_description", totalDamage));

            var resultDamageView = _resultDamage.GetCompactView();
            if (resultDamageView != null)
            {
                sb.AppendLine();
                sb.Append(resultDamageView);
            }
            return sb.ToString();
        }

    }
}
