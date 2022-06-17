using System;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    [Serializable]
    public class ItemGenerationSettings
    {
        public int basisPoints;
        public ushort levelForCommonItems;
        public ushort levelForRareItems;
        public ushort levelForEpicItems;
        public ushort levelForLegendaryItems;
    }
}
