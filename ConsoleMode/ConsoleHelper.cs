using System;

namespace TextGameRPG.ConsoleMode
{
    public static class ConsoleHelper
    {
        public static bool AskYesNo(string question)
        {
            Console.Write(question + " (Y/N): ");
            while (true)
            {
                var keyInfo = Console.ReadKey();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Y:
                        Console.WriteLine();
                        return true;
                    case ConsoleKey.N:
                        Console.WriteLine();
                        return false;
                }
            }
        }


    }
}
