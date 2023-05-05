using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Buildings.Training;
using MarkOne.Scripts.GameCore.Inventory;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.ActionHandlers;
using MarkOne.Scripts.GameCore.Units.Stats;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Units;
public class FakePlayer : IBattleUnit
{
    public EquippedItems equippedItems { get; }
    public FakePlayerSkills skills { get; }
    public byte level { get; }
    public byte townhallLevel { get; }
    public string nickname { get; }
    public UnitStats unitStats { get; }
    public IBattleActionHandler actionHandler { get; }
    public bool isPremium { get; }
    public Emoji avatar { get; }

    public FakePlayer(IEnumerable<InventoryItem> _items, Dictionary<ItemType, byte> _skills, byte _level, string _nickname,
        bool _isPremium = false, Emoji? _avatar = null)
    {
        equippedItems = new EquippedItems(_items);
        skills = new FakePlayerSkills(this, _skills); // before unitStats!
        level = _level;
        townhallLevel = GetPlayerTownhallLevel(_level);
        nickname = _nickname;
        unitStats = new FakePlayerStats(this);
        actionHandler = new FakePlayerActionHandler(this);
        isPremium = _isPremium;
        avatar = _avatar ?? Emojis.AvatarMale;
    }

    private byte GetPlayerTownhallLevel(byte playerLevel)
    {
        var trainingBuilding = (WarriorTrainingBuilding)BuildingId.WarriorTraining.GetBuilding();
        var prevMaxLevel = 1;
        var playerTownhall = 1;
        foreach (TrainingLevelInfo trainingLevelInfo in trainingBuilding.buildingData.levels)
        {
            if (playerLevel > prevMaxLevel)
            {
                playerTownhall = trainingLevelInfo.requiredTownHall;
            }
            prevMaxLevel = trainingLevelInfo.maxUnitLevel;
        }
        return (byte)playerTownhall;
    }

    public string GetGeneralUnitInfoView(GameSession sessionToSend)
    {
        return new StringBuilder()
            .AppendLine(Emojis.AvatarMale + nickname.Bold() + (isPremium ? Emojis.StatPremium : Emojis.Empty))
            .AppendLine(Localization.Get(sessionToSend, "unit_view_level", level))
            .ToString();
    }

    public string GetFullUnitInfoView(GameSession sessionToSend, bool withHealth = true)
    {
        var sb = new StringBuilder()
            .Append(GetGeneralUnitInfoView(sessionToSend))
            .AppendLine()
            .AppendLine(unitStats.GetView(sessionToSend, withHealth));

        if (IsSkillsAvailable())
        {
            sb.AppendLine();
            sb.AppendLine(skills.GetShortView(sessionToSend));
        }

        return sb.ToString();
    }

    public bool IsSkillsAvailable()
    {
        var requiredTownhall = BuildingId.ElixirWorkshop.GetBuilding().buildingData.levels[0].requiredTownHall;
        return townhallLevel >= requiredTownhall;
    }

    public Task OnStartBattle(Battle battle)
    {
        unitStats.OnStartBattle();
        return Task.CompletedTask;
    }

    public Task OnEnemyBattleTurnTimeEnd()
    {
        // ignored
        return Task.CompletedTask;
    }

    public void OnMineBattleTurnAlmostEnd()
    {
        // ignored
    }

    public Task OnMineBatteTurnTimeEnd()
    {
        // ignored
        return Task.CompletedTask;
    }

    public Task OnStartEnemyTurn(BattleTurn battleTurn)
    {
        // ignored
        return Task.CompletedTask;
    }
}
