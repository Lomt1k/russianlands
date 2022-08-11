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
                { ResourceType.Diamonds, new ResourceDiamonds() },
                { ResourceType.Wood, new ResourceWood() },
                { ResourceType.Iron, new ResourceIron() },

                //additional
                { ResourceType.Arrows, new ResourceArrows() },
            };
        }

        public IEnumerable<ResourceType> GetGeneralResourceTypes()
        {
            yield return ResourceType.Gold;
            yield return ResourceType.Food;
            yield return ResourceType.Diamonds;
            yield return ResourceType.Wood;
            yield return ResourceType.Iron;
        }

    }
}
