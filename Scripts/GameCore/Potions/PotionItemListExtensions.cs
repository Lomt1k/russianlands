using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Potions
{
    public static class PotionItemListExtensions
    {
        public static int GetMaxCount(this List<PotionItem> potions, GameSession session)
        {
            return session.profile.data.IsPremiumActive() ? 14 : 7;
        }

        public static bool IsFull(this List<PotionItem> potions, GameSession session)
        {
            return potions.Count >= potions.GetMaxCount(session);
        }

        public static bool HasReadyPotions(this List<PotionItem> potions)
        {
            foreach (var item in potions)
            {
                if (item.IsReady())
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<PotionItem> GetReadyPotions(this List<PotionItem> potions)
        {
            foreach (var item in potions)
            {
                if (item.IsReady())
                {
                    yield return item;
                }
            }
        }

    }
}
