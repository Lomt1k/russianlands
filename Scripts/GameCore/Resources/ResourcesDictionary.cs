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
                //-- TODO
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

    }
}
