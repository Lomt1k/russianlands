using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using System;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public class PlayerAttackAction : IBattleAction
    {
        public BattleActionPriority priority => BattleActionPriority.OnAttack;

        private readonly InventoryItem? _item;
        private DamageInfo _damageInfo;
        private DamageInfo _resultDamage;

        /// <summary>
        /// Конструктор для атаки мечом / луком / посохом / свитком
        /// если item == null, то это атака кулаком
        /// </summary>
        public PlayerAttackAction(Player attacker, InventoryItem? item)
        {
            var dealDamageAbility = item?.data.ablitityByType[AbilityType.DealDamage] as DealDamageAbility;
            if (dealDamageAbility == null)
            {
                _damageInfo = new DamageInfo(10, 0, 0, 0); // Урон кулаком: 10 единиц (меньше любого оружия)
                return;
            }
            _item = item;
            _damageInfo = dealDamageAbility.GetRandomValues();
            AppendDamageBonusByAttributes(attacker, item);
        }

        private void AppendDamageBonusByAttributes(Player attacker, InventoryItem? item)
        {
            if (item == null)
                return;

            var playerStats = (PlayerStats)attacker.unitStats;
            var itemType = item.data.itemType;
            var attrValue = itemType == ItemType.Stick || itemType == ItemType.Scroll
                ? playerStats.attributeSorcery : playerStats.attributeStrength;

            var damageInfo = _damageInfo;
            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
            {
                var bonusPerAttributePoint = (float)damageInfo[damageType] / 500; // +0.02% к урону за очко силы / колдовства
                damageInfo[damageType] += (int)(bonusPerAttributePoint * attrValue);
            }
            _damageInfo = damageInfo;
        }

        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
            _resultDamage = stats.TryDealDamage(_damageInfo);
        }

        public string GetLocalization(GameSession session)
        {
            var sb = new StringBuilder();
            var totalDamage = _resultDamage.GetTotalValue();
            var itemName = _item != null
                ? _item.GetFullName(session) 
                : $"{Emojis.stats[Stat.PhysicalDamage]} {Localization.Get(session, "battle_attack_fists")}";

            sb.AppendLine($"<b>{itemName}</b>");
            sb.Append(string.Format(Localization.Get(session, "battle_action_attack_description"), totalDamage));
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
