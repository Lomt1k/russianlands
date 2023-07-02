using System;
using System.Collections.Generic;

namespace MarkOne.ConsoleMode;

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

    public static T SelectFromVariants<T>(string question, Dictionary<string, T> variants)
    {
        Console.WriteLine(question + ": ");
        var variantsByNumber = new Dictionary<int, T>();
        var index = 0;
        foreach (var (text, value) in variants)
        {
            index++;
            variantsByNumber.Add(index, value);
            Console.WriteLine($"{index} - {text}");
        }
        Console.Write($"Enter a number (1 - {index}): ");

        while (true)
        {
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var number) || number < 1 || number > index)
            {
                Console.WriteLine("Incorrent input!");
                Console.Write($"Enter a number (1 - {index}): ");
                continue;
            }
            return variantsByNumber[number];
        }
    }


}
