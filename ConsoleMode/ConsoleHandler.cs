using System;

namespace TextGameRPG.ConsoleMode
{
    public class ConsoleHandler
    {
        public void Start(string[] args)
        {
            Console.WriteLine("Started with console...");
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"arg {i}: {args[i]}");
            }
            Console.ReadLine();
            // TODO!
            Console.WriteLine("Shutdown...");
        }
    }
}
