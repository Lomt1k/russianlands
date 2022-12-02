namespace TextGameRPG.Scripts.GameCore.Units.Stats
{
    public static class PlayerHealthByLevel
    {
        private static int[] healthByLevel = new int[]
        {
            300, // Level 0

            300, // Level 1,
            330, // Level 2
            360, // Level 3
            390, // Level 4

            420, // Level 5
            450, // Level 6
            480, // Level 7

            520, // Level 8
            560, // Level 9
            600, // Level 10

            650, // Level 11
            700, // Level 12
            750, // Level 13
            800, // Level 14
            850, // Level 15

            920, // Level 16
            990, // Level 17
            1060, // Level 18
            1130, // Level 19
            1200, // Level 20

            1300, // Level 21
            1400, // Level 22
            1500, // Level 23
            1600, // Level 24
            1700, // Level 25
            1800, // Level 26
            1900, // Level 27
            2000, // Level 28

            2140, // Level 29
            2280, // Level 30
            2420, // Level 31
            2560, // Level 32
            2700, // Level 33
            2850, // Level 34
            3000, // Level 35

            3170, // Level 36
            3340, // Level 37
            3520, // Level 38
            3700, // Level 39
            3880, // Level 40
            4060, // Level 41
            4250, // Level 42

            4460, // Level 43
            4680, // Level 44
            4900, // Level 45
            5120, // Level 46
            5340, // Level 47
            5560, // Level 48
            5780, // Level 49
            6000, // Level 50
        };

        public static int Get(int level)
        {
            if (level < 0)
                return healthByLevel[0];

            if (level >= healthByLevel.Length)
                return healthByLevel[level - 1];

            return healthByLevel[level];
        }

    }
}
