using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Battle
{
    public class SelectBattleActionDialog : DialogBase
    {
        private Action<IBattleAction> _selectedActionCallback;
        private BattleTurn _battleTurn;

        public SelectBattleActionDialog(GameSession _session, BattleTurn battleTurn, Action<IBattleAction> callback) : base(_session)
        {
            _battleTurn = battleTurn;
            _selectedActionCallback = callback;
        }

        public override async Task Start()
        {
            ClearButtons();
            var keyboardRows = new List<List<KeyboardButton>>();
            AppendSingleSlotItems(ref keyboardRows);
            AppendMultiSlotItems(ref keyboardRows);
            var keyboard = new ReplyKeyboardMarkup(keyboardRows);

            var sb = new StringBuilder();
            sb.Append($"{Emojis.menuItems[MenuItem.Battle]} {Localization.Get(session, "battle_mine_turn_start")}");
            if (session.player.inventory.equipped.HasItem(ItemType.Bow))
            {
                sb.Append($" {Emojis.stats[Stat.Arrows]} {session.player.unitStats.currentArrows}");
            }
            if (session.player.inventory.equipped.HasItem(ItemType.Scroll))
            {
                sb.Append($" {Emojis.stats[Stat.Mana]} {session.player.unitStats.currentMana}");
            }
            sb.AppendLine();
            sb.AppendLine();

            if (_battleTurn.isLastChance)
            {
                sb.AppendLine($"{Emojis.elements[Element.BrokenHeart]} {Localization.Get(session, "battle_mine_turn_start_last_chance")}");
                sb.AppendLine();
            }

            var equippedStick = session.player.inventory.equipped[ItemType.Stick];
            if (equippedStick != null)
            {
                var currentCharge = session.player.unitStats.currentStickCharge;
                var requiredCharge = equippedStick.data.requiredCharge;
                if (currentCharge < requiredCharge)
                {
                    var localization = Localization.Get(session, "battle_mine_turn_stick_charge");
                    sb.AppendLine(string.Format(localization, Emojis.items[ItemType.Stick], currentCharge, requiredCharge));
                    sb.AppendLine();
                }
            }

            sb.AppendLine(Localization.Get(session, "battle_mine_turn_start_select_item"));

            await SendDialogMessage(sb, keyboard);
        }

        public void AppendSingleSlotItems(ref List<List<KeyboardButton>> keyboardRows)
        {
            var equipped = session.player.inventory.equipped;
            var unitStats = session.player.unitStats;

            var swordItem = equipped[ItemType.Sword];
            var swordAttackText = swordItem != null ? swordItem.GetFullName(session) 
                : $"{Emojis.stats[Stat.PhysicalDamage]} {Localization.Get(session, "battle_attack_fists")}";
            RegisterButton(swordAttackText, () => OnCategorySelected(ItemType.Sword));
            keyboardRows.Add(new List<KeyboardButton> { swordAttackText });

            var bowItem = equipped[ItemType.Bow];
            if (bowItem != null && unitStats.currentArrows > 0)
            {
                var bowAttakText = bowItem.GetFullName(session);
                RegisterButton(bowAttakText, () => OnCategorySelected(ItemType.Bow));
                keyboardRows.Add(new List<KeyboardButton> { bowAttakText });
            }

            var stickItem = equipped[ItemType.Stick];
            if (stickItem != null && unitStats.currentStickCharge >= stickItem.data.requiredCharge)
            {
                var stickAttackText = stickItem.GetFullName(session);
                RegisterButton(stickAttackText, () => OnCategorySelected(ItemType.Stick));
                keyboardRows.Add(new List<KeyboardButton> { stickAttackText });
            }
        }

        public void AppendMultiSlotItems(ref List<List<KeyboardButton>> keyboardRows)
        {
            var equipped = session.player.inventory.equipped;
            var multiRow = new List<KeyboardButton>();

            if (equipped.HasItem(ItemType.Poison))
            {
                var poisonsButtonText = $"{Emojis.items[ItemType.Poison]} {Localization.Get(session, "menu_item_poisons")}";
                RegisterButton(poisonsButtonText, () => OnCategorySelected(ItemType.Poison));
                multiRow.Add(poisonsButtonText);
            }
            if (equipped.HasItem(ItemType.Scroll))
            {
                var scrollsButtonText = $"{Emojis.items[ItemType.Scroll]} {Localization.Get(session, "menu_item_scrolls")}";
                RegisterButton(scrollsButtonText, () => OnCategorySelected(ItemType.Scroll));
                multiRow.Add(scrollsButtonText);
            }

            if (multiRow.Count > 0)
            {
                keyboardRows.Add(multiRow);
            }
        }


        public async Task OnCategorySelected(ItemType category)
        {
            if (!_battleTurn.isWaitingForActions)
                return;

            var equippedItems = session.player.inventory.equipped;
            switch (category)
            {
                case ItemType.Sword:
                case ItemType.Bow:
                case ItemType.Stick:
                    var item = equippedItems[category];
                    var attackWithItem = new PlayerAttackAction(session.player, item);
                    _selectedActionCallback(attackWithItem);
                    if (item != null)
                    {
                        session.player.unitStats.OnUseItemInBattle(item);
                    }
                    break;
                case ItemType.Scroll:
                    await ShowScrollsCategory();
                    break;
                case ItemType.Poison:
                    //TODO: Select poison dialog
                    break;
            }
        }

        private async Task ShowScrollsCategory()
        {
            var equipped = session.player.inventory.equipped;
            var unitStats = session.player.unitStats;

            ClearButtons();
            var sb = new StringBuilder();
            sb.Append($"{Emojis.items[ItemType.Scroll]} <b>{Localization.Get(session, "menu_item_scrolls")}</b>");
            sb.AppendLine($" {Emojis.stats[Stat.Mana]} {unitStats.currentMana}");
            sb.AppendLine();

            foreach (var scrollItem in GetAllEquippedScrolls())
            {
                if (scrollItem == null)
                    continue;

                sb.AppendLine($"<b>{scrollItem.GetFullName(session)}</b>");
                var manaCost = Emojis.stats[Stat.Mana] + ' ' + scrollItem.manaCost;
                sb.AppendLine(string.Format(Localization.Get(session, "item_view_cost_of_use").RemoveHtmlTags(), manaCost));
                if (scrollItem.data.ablitityByType.TryGetValue(AbilityType.DealDamage, out var dealDamage))
                {
                    var simpleDamageView = ((DealDamageAbility)dealDamage).GetSimpleView(session);
                    sb.AppendLine(simpleDamageView.RemoveHtmlTags());
                }
                sb.AppendLine();

                if (unitStats.currentMana >= scrollItem.manaCost)
                {
                    RegisterButton(scrollItem.GetFullName(session), () => SelectScrollItem(scrollItem));
                }
            }
            if (buttonsCount == 0)
            {
                sb.AppendLine($"{Emojis.elements[Element.WarningGrey]} {Localization.Get(session, "battle_not_enough_mana")}");
            }

            RegisterBackButton(() => Start());
            await SendDialogMessage(sb, GetMultilineKeyboard());
        }

        private Task SelectScrollItem(InventoryItem scrollItem)
        {
            var attackWithItem = new PlayerAttackAction(session.player, scrollItem);
            _selectedActionCallback(attackWithItem);
            session.player.unitStats.OnUseItemInBattle(scrollItem);
            return Task.CompletedTask;
        }

        private IEnumerable<InventoryItem?> GetAllEquippedScrolls()
        {
            var equipped = session.player.inventory.equipped;
            for (int i = 0; i < ItemType.Scroll.GetSlotsCount(); i++)
            {
                yield return equipped[ItemType.Scroll, i];
            }
        }

    }
}
