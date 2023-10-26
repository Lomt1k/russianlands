using MarkOne.Scripts.GameCore.DailyBonus;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Events.DailyBonus;
public class DailyBonusDialog : DialogBase
{
    public DailyBonusDialog(GameSession _session) : base(_session)
    {
    }

    public override Task Start()
    {
        throw new NotImplementedException();
    }

    public static bool IsDailyBonusEventAvailable(GameSession session)
    {
        return session.profile.data.lastDailyBonusId < gameDataHolder.dailyBonuses.count;
    }

}
