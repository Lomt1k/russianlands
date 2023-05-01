using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourcesDictionary
    {
        private Dictionary<ResourceId, IResource> _dictionary;

        public IResource this[ResourceId resourceId] => _dictionary[resourceId];


        public ResourcesDictionary()
        {
            _dictionary = new Dictionary<ResourceId, IResource>
            {
                //general
                { ResourceId.Gold, new ResourceGold() },
                { ResourceId.Food, new ResourceFood() },
                { ResourceId.Diamond, new ResourceDiamond() },
                { ResourceId.Herbs, new ResourceHerbs() },
                { ResourceId.Wood, new ResourceWood() },

                //additional
                { ResourceId.InventoryItems, new ResourceInventoryItems() }, // регулирует размер инвентаря
                { ResourceId.CraftPiecesCommon, new ResourceCraftPiecesCommon() },
                { ResourceId.CraftPiecesRare, new ResourceCraftPiecesRare() },
                { ResourceId.CraftPiecesEpic, new ResourceCraftPiecesEpic() },
                { ResourceId.CraftPiecesLegendary, new ResourceCraftPiecesLegendary() },
                { ResourceId.FruitApple, new ResourceFruitApple() },
                { ResourceId.FruitPear, new ResourceFruitPear() },
                { ResourceId.FruitMandarin, new ResourceFruitMandarin() },
                { ResourceId.FruitCoconut, new ResourceFruitCoconut() },
                { ResourceId.FruitPineapple, new ResourceFruitPineapple() },
                { ResourceId.FruitBanana, new ResourceFruitBanana() },
                { ResourceId.FruitWatermelon, new ResourceFruitWatermelon() },
                { ResourceId.FruitStrawberry, new ResourceFruitStrawberry() },
                { ResourceId.FruitBlueberry, new ResourceFruitBlueberry() },
                { ResourceId.FruitKiwi, new ResourceFruitKiwi() },
                { ResourceId.FruitCherry, new ResourceFruitCherry() },
                { ResourceId.FruitGrape, new ResourceFruitGrape() },
                { ResourceId.ArenaTicket, new ResourceArenaTicket() },
            };
        }

        public IEnumerable<ResourceId> GetGeneralResourceIds()
        {
            yield return ResourceId.Gold;
            yield return ResourceId.Food;
            yield return ResourceId.Diamond;
            yield return ResourceId.Herbs;
            yield return ResourceId.Wood;
        }

        public IEnumerable<ResourceId> GetCraftResourceIds()
        {
            yield return ResourceId.CraftPiecesCommon;
            yield return ResourceId.CraftPiecesRare;
            yield return ResourceId.CraftPiecesEpic;
            yield return ResourceId.CraftPiecesLegendary;
        }

        public IEnumerable<ResourceId> GetFruitTypes()
        {
            yield return ResourceId.FruitApple;
            yield return ResourceId.FruitPear;
            yield return ResourceId.FruitMandarin;
            yield return ResourceId.FruitCoconut;
            yield return ResourceId.FruitPineapple;
            yield return ResourceId.FruitBanana;
            yield return ResourceId.FruitWatermelon;
            yield return ResourceId.FruitStrawberry;
            yield return ResourceId.FruitBlueberry;
            yield return ResourceId.FruitKiwi;
            yield return ResourceId.FruitCherry;
            yield return ResourceId.FruitGrape;
        }

    }
}
