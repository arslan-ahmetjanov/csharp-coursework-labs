using System;
using System.Collections.Generic;
using System.Text;

namespace Lab27;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Лабораторная работа № 27. Уроки 5–6");
        Console.WriteLine("Массивы, список List, циклы");
        Console.WriteLine(new string('-', 60));

        Console.WriteLine("Задача 1. Первые 20 элементов: 1, 4, 7, 10, ...");
        var sequence = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            sequence.Add(1 + 3 * i);
        }
        Console.WriteLine("   " + string.Join(", ", sequence));

        Console.WriteLine();
        Console.WriteLine("Задача 2. Запрос пароля до ввода правильного");
        string[] attempts = { "admin", "1234", "root" };
        foreach (string attempt in attempts)
        {
            Console.WriteLine($"   Попытка: {attempt}");
            if (attempt == "root")
            {
                Console.WriteLine("   Пароль верный, доступ разрешён.");
                break;
            }
            Console.WriteLine("   Неверный пароль, повторите ввод.");
        }

        Console.WriteLine();
        Console.WriteLine("Задача 3. Поэлементная сумма двух массивов");
        int[] first = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int[] second = { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        int[] sum = new int[10];
        for (int i = 0; i < sum.Length; i++)
        {
            sum[i] = first[i] + second[i];
        }
        Console.WriteLine("   Первый массив:  " + string.Join(", ", first));
        Console.WriteLine("   Второй массив:  " + string.Join(", ", second));
        Console.WriteLine("   Сумма:          " + string.Join(", ", sum));

        Console.WriteLine();
        if (!args.Contains("--no-wait"))
        {
            Console.WriteLine("Готово. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
