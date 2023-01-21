using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourcesDictionary
    {
        private Dictionary<ResourceType, IResource> _dictionary;

        public IResource this[ResourceType resourceType] => _dictionary[resourceType];


        public ResourcesDictionary()
        {
            _dictionary = new Dictionary<ResourceType, IResource>
            {
                //general
                { ResourceType.Gold, new ResourceGold() },
                { ResourceType.Food, new ResourceFood() },
                { ResourceType.Diamond, new ResourceDiamond() },
                { ResourceType.Herbs, new ResourceHerbs() },
                { ResourceType.Wood, new ResourceWood() },

                //additional
                { ResourceType.InventoryItems, new ResourceInventoryItems() }, // регулирует размер инвентаря
                { ResourceType.CraftPiecesCommon, new ResourceCraftPiecesCommon() },
                { ResourceType.CraftPiecesRare, new ResourceCraftPiecesRare() },
                { ResourceType.CraftPiecesEpic, new ResourceCraftPiecesEpic() },
                { ResourceType.CraftPiecesLegendary, new ResourceCraftPiecesLegendary() },
                { ResourceType.FruitApple, new ResourceFruitApple() },
                { ResourceType.FruitPear, new ResourceFruitPear() },
                { ResourceType.FruitMandarin, new ResourceFruitMandarin() },
                { ResourceType.FruitCoconut, new ResourceFruitCoconut() },
                { ResourceType.FruitPineapple, new ResourceFruitPineapple() },
                { ResourceType.FruitBanana, new ResourceFruitBanana() },
                { ResourceType.FruitWatermelon, new ResourceFruitWatermelon() },
                { ResourceType.FruitStrawberry, new ResourceFruitStrawberry() },
                { ResourceType.FruitBlueberry, new ResourceFruitBlueberry() },
                { ResourceType.FruitKiwi, new ResourceFruitKiwi() },
                { ResourceType.FruitCherry, new ResourceFruitCherry() },
                { ResourceType.FruitGrape, new ResourceFruitGrape() },

                //-TODO
            };
        }

        public IEnumerable<ResourceType> GetGeneralResourceTypes()
        {
            yield return ResourceType.Gold;
            yield return ResourceType.Food;
            yield return ResourceType.Diamond;
            yield return ResourceType.Herbs;
            yield return ResourceType.Wood;
        }

        public IEnumerable<ResourceType> GetCraftResourceTypes()
        {
            yield return ResourceType.CraftPiecesCommon;
            yield return ResourceType.CraftPiecesRare;
            yield return ResourceType.CraftPiecesEpic;
            yield return ResourceType.CraftPiecesLegendary;
        }

        public IEnumerable<ResourceType> GetFruitTypes()
        {
            yield return ResourceType.FruitApple;
            yield return ResourceType.FruitPear;
            yield return ResourceType.FruitMandarin;
            yield return ResourceType.FruitCoconut;
            yield return ResourceType.FruitPineapple;
            yield return ResourceType.FruitBanana;
            yield return ResourceType.FruitWatermelon;
            yield return ResourceType.FruitStrawberry;
            yield return ResourceType.FruitBlueberry;
            yield return ResourceType.FruitKiwi;
            yield return ResourceType.FruitCherry;
            yield return ResourceType.FruitGrape;
        }

    }
}
