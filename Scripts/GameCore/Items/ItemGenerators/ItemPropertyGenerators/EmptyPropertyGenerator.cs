using System;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators
{
    class EmptyPropertyGenerator : ItemPropertyGeneratorBase
    {
        public override ItemPropertyGeneratorType propertyType => ItemPropertyGeneratorType.None;
        public override string debugDescription => "[EMPTY PROPERTY]";

        public override ItemPropertyGeneratorBase Clone()
        {
            return new EmptyPropertyGenerator();
        }

        public override ItemPropertyBase Generate()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return debugDescription;
        }
    }
}
