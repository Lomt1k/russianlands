﻿using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace MarkOne.Scripts.GameCore.Arena;
public class ArenaLeagueMatchMaker
{
    private static readonly BattleManager battleManager = ServiceLocator.Get<BattleManager>();
    private static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    private readonly ArenaLeagueSettings _arenaLeagueSettings;
    private readonly CancellationToken _cancellationToken;

    private TimeSpan _estimatedTime;
    private List<MatchMakerParticipant> _participants = new();
    private bool _isRegistrationBlocked;

    public ArenaLeagueMatchMaker(ArenaLeagueSettings arenaLeagueSettings, CancellationToken cancellationToken)
    {
        _arenaLeagueSettings = arenaLeagueSettings;
        _cancellationToken = cancellationToken;
        Task.Run(PeriodicRefreshEstimatedTime, cancellationToken);
        Task.Run(PeriodicTryCreateBattles, cancellationToken);
    }

    private async Task PeriodicRefreshEstimatedTime()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            _estimatedTime = _arenaLeagueSettings.GetRandomMatchmakingTime();
            await Task.Delay(30_000).FastAwait();
        }
    }
    
    private async Task PeriodicTryCreateBattles()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            TryCreateBattles();
            await Task.Delay(1000).FastAwait();
        }
    }

    private void TryCreateBattles()
    {
        _isRegistrationBlocked = true;
        {
            var now = DateTime.UtcNow;
            for (int i = 0; i < _participants.Count; i++)
            {
                var participantA = _participants[i];
                if (TryFindOpponent(i, out int j))
                {
                    var participantB = _participants[j];
                    StartPVP(participantA.player, participantB.player);
                    _participants.RemoveAt(j);
                    _participants.RemoveAt(i);
                    i--;
                }
                else if (participantA.timeToStartFakePVP <= now)
                {
                    StartFakePVP(participantA.player);
                    _participants.RemoveAt(i);
                    i--;
                }
            }
        }
        _isRegistrationBlocked = false;
    }

    private bool TryFindOpponent(int mineIndex, out int opponentIndex)
    {
        opponentIndex = -1;
        var mineParticipant = _participants[mineIndex];
        for (int i = _participants.Count - 1; i > mineIndex; i--)
        {
            var opponent = _participants[i];
            if (opponent.CanPlayWith(mineParticipant))
            {
                opponentIndex = i;
                return true;
            }
        }
        return false;
    }

    public async Task<TimeSpan> TryRegisterPlayer(Player player)
    {
        var timeToStartFakePVP = _arenaLeagueSettings.GetRandomMatchmakingTime();
        var participant = new MatchMakerParticipant(player, timeToStartFakePVP);
        await WaitForRegistrationUnlock().FastAwait();
        {
            _participants.Add(participant);
        }
        return _estimatedTime;
    }

    public async Task<bool> TryUnregisterPlayer(Player player)
    {
        await WaitForRegistrationUnlock().FastAwait();
        {
            var participant = _participants.FirstOrDefault(x => x.player == player);
            return participant != null && _participants.Remove(participant);
        }
    }

    private async Task WaitForRegistrationUnlock()
    {
        while (_isRegistrationBlocked)
        {
            await Task.Delay(15).FastAwait();
        }
    }

    private async void StartPVP(Player playerA, Player playerB)
    {
        var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, playerA.session.cancellationToken, playerB.session.cancellationToken);
        if (linkedCancellationToken.IsCancellationRequested)
            return;

        var sendMessageA = SendStartBattleMessage(playerA, playerB);
        var sendMessageB = SendStartBattleMessage(playerB, playerA);
        await Task.WhenAll(sendMessageA, sendMessageB).FastAwait();
        await Task.Delay(5000).FastAwait();

        if (linkedCancellationToken.IsCancellationRequested)
            return;

        battleManager.StartBattle(playerA, playerB,
            onBattleEndFunc: (player, battleResult) =>
            {
                var enemyDbid = player == playerA ? playerB.dbid : playerA.dbid;
                var result = new ArenaBattleResult(battleResult, enemyDbid);
                player.profile.dynamicData.arenaProgress?.results.Add(result);
                return Task.CompletedTask;
            },
            onContinueButtonFunc: async (player, battleResult) =>
            {
                await new ArenaDialog(player.session).StartAfterBattleEnd().FastAwait();
            });
    }

    private async void StartFakePVP(Player player)
    {
        var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, player.session.cancellationToken);
        if (linkedCancellationToken.IsCancellationRequested)
            return;

        // fake player generation logic (debug version!)
        var items = player.inventory.equipped.allEquipped;
        var skills = player.skills.GetAllSkills();
        var fakePlayer = new FakePlayer(items, skills, player.level, "Shadow Copy", player.session.profile.data.IsPremiumActive());

        await SendStartBattleMessage(player, fakePlayer).FastAwait();
        await Task.Delay(5000).FastAwait();

        if (linkedCancellationToken.IsCancellationRequested)
            return;

        battleManager.StartBattle(player, fakePlayer,
            onBattleEndFunc: (player, battleResult) =>
            {
                var result = new ArenaBattleResult(battleResult);
                player.profile.dynamicData.arenaProgress?.results.Add(result);
                return Task.CompletedTask;
            },
            onContinueButtonFunc: async (player, battleResult) =>
            {
                await new ArenaDialog(player.session).StartAfterBattleEnd().FastAwait();
            });
    }

    private static async Task SendStartBattleMessage(Player player, IBattleUnit enemyForInfo)
    {
        var footerText = Localization.Get(player.session, "dialog_arena_enemy_found_footer");
        var sb = new StringBuilder()
            .AppendLine(Localization.Get(player.session, "dialog_arena_enemy_found_header"))
            .AppendLine()
            .AppendLine(enemyForInfo.GetFullUnitInfoView(player.session))
            .AppendLine(footerText.Italic());

        var replyKeyboard = new ReplyKeyboardMarkup(new KeyboardButton(Emojis.ElementHourgrlass + footerText));
        await messageSender.SendTextDialog(player.session.chatId, sb.ToString(), replyKeyboard);
    }

}
