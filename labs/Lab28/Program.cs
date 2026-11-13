using System;
using System.Collections.Generic;
using System.Text;

namespace Lab28;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Лабораторная работа № 28. Уроки 7–8");
        Console.WriteLine("Цикл foreach, функции, оператор return");
        Console.WriteLine(new string('-', 60));

        Console.WriteLine("Задача 1. Элементы массива больше 20 и меньше 50");
        int[] numbers = { 4, 7, 13, 20, 33, 23, 54, 41, 19, 50 };
        Console.WriteLine("   Массив: " + string.Join(", ", numbers));
        Console.WriteLine("   Подходят: " +
                          string.Join(", ", FilterBetween(numbers, 20, 50)));

        Console.WriteLine();
        Console.WriteLine("Задача 2. Замена указанного значения на противоположное");
        int[] values = { 5, 1, 5, -3, 7, 5, 0 };
        int target = 5;
        Console.WriteLine("   Было:  " + string.Join(", ", values));
        Console.WriteLine($"   Меняем {target} на {-target}: " +
                          string.Join(", ", NegateValue(values, target)));

        Console.WriteLine();
        Console.WriteLine("Задача 3. Минимум из трёх чисел");
        Console.WriteLine($"   min(8, 3, 5) = {MinOfThree(8, 3, 5)}");
        Console.WriteLine($"   min(-2, -7, -1) = {MinOfThree(-2, -7, -1)}");
        Console.WriteLine($"   min(4, 4, 4) = {MinOfThree(4, 4, 4)}");

        Console.WriteLine();
        Console.WriteLine("Задача 4. Элемент ряда Фибоначчи");
        Console.Write("   Первые 10 чисел: ");
        for (int i = 1; i <= 10; i++)
        {
            Console.Write(Fibonacci(i) + (i < 10 ? ", " : Environment.NewLine));
        }
        Console.WriteLine($"   Fibonacci(10) = {Fibonacci(10)}");
        Console.WriteLine($"   Fibonacci(20) = {Fibonacci(20)}");

        Console.WriteLine();
        if (!args.Contains("--no-wait"))
        {
            Console.WriteLine("Готово. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    private static List<int> FilterBetween(int[] numbers, int min, int max)
    {
        var result = new List<int>();
        foreach (int number in numbers)
        {
            if (number > min && number < max)
            {
                result.Add(number);
            }
        }
        return result;
    }

    private static int[] NegateValue(int[] numbers, int value)
    {
        int[] result = new int[numbers.Length];
        for (int i = 0; i < numbers.Length; i++)
        {
            result[i] = numbers[i] == value ? -numbers[i] : numbers[i];
        }
        return result;
    }

    private static int MinOfThree(int a, int b, int c)
    {
        int min = a;
        if (b < min)
        {
            min = b;
        }
        if (c < min)
        {
            min = c;
        }
        return min;
    }

    private static long Fibonacci(int position)
    {
        if (position < 1)
        {
            return 0;
        }

        long previous = 1;
        long current = 1;
        for (int i = 3; i <= position; i++)
        {
            long next = previous + current;
            previous = current;
            current = next;
        }
        return current;
    }
}
