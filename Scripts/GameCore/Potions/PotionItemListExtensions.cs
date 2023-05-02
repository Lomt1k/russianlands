using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Potions;

public static class PotionItemListExtensions
{
    public static int GetMaxCount(this List<PotionItem> potions, GameSession session)
    {
        return session.profile.data.IsPremiumActive() ? 14 : 7;
    }

    public static int GetFreeSlotsCount(this List<PotionItem> potions, GameSession session)
    {
        return potions.GetMaxCount(session) - potions.Count;
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

    public static bool HasPotionsInProduction(this List<PotionItem> potions)
    {
        foreach (var item in potions)
        {
            if (!item.IsReady())
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

    public static IEnumerable<PotionItem> GetPotionsInProduction(this List<PotionItem> potions)
    {
        foreach (var item in potions)
        {
            if (!item.IsReady())
            {
                yield return item;
            }
        }
    }

    public static int GetReadyPotionsCount(this List<PotionItem> potions)
    {
        var count = 0;
        foreach (var item in potions)
        {
            if (item.IsReady())
            {
                count++;
            }
        }
        return count;
    }

    public static int GetPotionsInProductionCount(this List<PotionItem> potions)
    {
        var count = 0;
        foreach (var item in potions)
        {
            if (!item.IsReady())
            {
                count++;
            }
        }
        return count;
    }

    public static void SortByPreparationTime(this List<PotionItem> potions)
    {
        potions.Sort((x, y) => x.preparationTime.CompareTo(y.preparationTime));
    }

}