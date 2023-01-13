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

    }
}
