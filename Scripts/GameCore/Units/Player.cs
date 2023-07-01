using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.CallbackData;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Inventory;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Skills;
using MarkOne.Scripts.GameCore.Units.ActionHandlers;
using MarkOne.Scripts.GameCore.Units.Stats;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Profiles;
using FastTelegramBot.DataTypes.Keyboards;

namespace MarkOne.Scripts.GameCore.Units;

public class Player : IBattleUnit
{
    private static readonly MessageSender messageSender = Services.ServiceLocator.Get<MessageSender>();

    public GameSession session { get; }
    public UnitStats unitStats { get; }
    public IBattleActionHandler actionHandler { get; }
    public PlayerResources resources { get; }
    public PlayerBuildings buildings { get; }
    public PlayerSkills skills { get; }
    public HealthRegenerationController healhRegenerationController { get; }
    public Profile profile => session.profile;
    public PlayerInventory inventory => profile.dynamicData.inventory;
    public List<PotionItem> potions => profile.dynamicData.potions;
    public string nickname => profile.data.nickname;
    public byte level => profile.data.level;
    public long dbid => profile.data.dbid;

    public Player(GameSession _session)
    {
        session = _session;
        skills = new PlayerSkills(this); // before unitStats!
        unitStats = new PlayerStats(this);
        actionHandler = new PlayerActionHandler(this);
        resources = new PlayerResources(_session);
        buildings = new PlayerBuildings(_session);

        healhRegenerationController = new HealthRegenerationController(this);
    }

    public string GetGeneralUnitInfoView(GameSession sessionToSend)
    {
        var isPremium = session.profile.data.IsPremiumActive();
        return new StringBuilder()
            .AppendLine(Emojis.AvatarMale + nickname.Bold() + (isPremium ? Emojis.StatPremium : Emojis.Empty))
            .AppendLine(Localization.Get(sessionToSend, "unit_view_level", level))
            .ToString();
    }

    public string GetFullUnitInfoView(GameSession sessionToSend)
    {
        var sb = new StringBuilder()
            .Append(GetGeneralUnitInfoView(sessionToSend))
            .AppendLine()
            .AppendLine(unitStats.GetView(sessionToSend));

        if (IsSkillsAvailable())
        {
            sb.AppendLine();
            sb.AppendLine(skills.GetShortView(sessionToSend));
        }

        return sb.ToString();
    }

    public bool IsSkillsAvailable()
    {
        return buildings.HasBuilding(BuildingId.ElixirWorkshop);
    }

    public async Task OnStartBattle(Battle battle)
    {
        unitStats.OnStartBattle();

        var sb = new StringBuilder();
        sb.AppendLine(Emojis.ButtonBattle + Localization.Get(session, "battle_start"));
        sb.AppendLine(Localization.Get(session, "battle_your_turn_" + (this == battle.firstUnit ? "first" : "second")));
        sb.AppendLine();
        sb.AppendLine(battle.GetStatsView(session));

        var keyboard = BattleToolipHelper.GetStatsKeyboard(session);
        await messageSender.SendTextMessage(session.chatId, sb.ToString(), keyboard, cancellationToken: session.cancellationToken).FastAwait();
    }

    public void OnBattleEnd(Battle battle, BattleResult battleResult)
    {
        unitStats.OnBattleEnd();
        healhRegenerationController.SetLastRegenTimeAsNow();
    }

    public async Task OnStartEnemyTurn(BattleTurn battleTurn)
    {
        var sb = new StringBuilder();
        if (battleTurn.isLastChance)
        {
            sb.AppendLine(Emojis.ElementBrokenHeart + Localization.Get(session, "battle_enemy_turn_start_last_chance"));
            sb.AppendLine();
        }

        var waitingText = Emojis.ElementHourgrlass + Localization.Get(session, "battle_enemy_turn_start");
        sb.AppendLine(waitingText);
        var keyboard = new ReplyKeyboardMarkup(waitingText);
        await messageSender.SendTextDialog(session.chatId, sb.ToString(), keyboard, silent: true, cancellationToken: session.cancellationToken).FastAwait();
    }

    public async void OnMineBattleTurnAlmostEnd()
    {
        var text = Emojis.ElementWarningGrey + Localization.Get(session, "battle_mine_turn_almost_end");
        await messageSender.SendTextMessage(session.chatId, text, silent: true, cancellationToken: session.cancellationToken).FastAwait();
    }

    public async Task OnMineBatteTurnTimeEnd()
    {
        var text = Localization.Get(session, "battle_mine_turn_time_end") + Emojis.SmileSad;
        await messageSender.SendTextMessage(session.chatId, text, silent: true, cancellationToken: session.cancellationToken).FastAwait();
    }

    public async Task OnEnemyBattleTurnTimeEnd()
    {
        var text = Localization.Get(session, "battle_enemy_turn_time_end");
        await messageSender.SendTextMessage(session.chatId, text, silent: true, cancellationToken: session.cancellationToken).FastAwait();
    }

}
