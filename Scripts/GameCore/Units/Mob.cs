using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Mob : IBattleUnit
    {
        public GameSession session { get; }
        public UnitStats unitStats { get; }
        public MobData mobData { get; }
        public int? grade { get; }
        public float gradeMult { get; } = 1.0f;

        public string nickname => Localizations.Localization.Get(session, mobData.localizationKey);

        public Mob(GameSession _session, MobData _data)
        {
            session = _session;
            mobData = _data;
            if (_data.withGrade)
            {
                var randGrade = Randomizer.GetGrade();
                grade = randGrade;
                gradeMult = (randGrade - 5) / 40f + 1; //от 0.9 до 1.125 (сам grade от 1 - 10)
            }

            unitStats = new MobStats(this);
        }

        public string GetStartTurnView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}</b>");

            sb.AppendLine(unitStats.GetView(session));
            return sb.ToString();
        }

        public async Task<List<IBattleAction>> GetActionsForBattleTurn(int maxSeconds)
        {
            var availableAttacks = GetAvailableAttacks();
            if (availableAttacks.Count == 0)
            {
                //TODO: return skip turn action
            }

            var attackIndex = new Random().Next(availableAttacks.Count);
            var attackAction = new MobAttackAction(availableAttacks[attackIndex]);

            return new List<IBattleAction>() { attackAction };
        }

        private List<MobAttack> GetAvailableAttacks()
        {
            var result = new List<MobAttack>();
            foreach (var attack in mobData.mobAttacks)
            {
                if (attack.manaCost <= unitStats.currentMana)
                {
                    result.Add(attack);
                }
            }
            return result;
        }

    }
}
