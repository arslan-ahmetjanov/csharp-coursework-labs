using System;
using System.Text;

namespace Lab29;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Лабораторная работа № 29. Уроки 9–10");
        Console.WriteLine("Строки и обработка исключений");
        Console.WriteLine(new string('-', 60));

        Console.WriteLine("Задача 1. Замена Nikolay на Oleg");
        string names = "Nikolay, Nikolay and Nikolay work here.";
        Console.WriteLine("   Было:  " + names);
        Console.WriteLine("   Стало: " + names.Replace("Nikolay", "Oleg"));

        Console.WriteLine();
        Console.WriteLine("Задача 2. Обрезка текста");
        string text = "Сегодня мы с вами рассмотрели, как работать со строками в Си-шарп. " +
                      "Были описаны основные операторы и методы, " +
                      "которые используются для работы со строками";
        string marker = "Были описаны основные операторы и методы";
        int start = text.IndexOf(marker, StringComparison.Ordinal);
        string cut = text.Substring(start, marker.Length);
        Console.WriteLine("   Исходный текст: " + text.Length + " символов");
        Console.WriteLine("   Результат:      " + cut);

        Console.WriteLine();
        Console.WriteLine("Задача 3. Логины в нижнем регистре");
        string logins = "Login1,LOgin2,login3,loGin4";
        string[] parts = logins.Split(',');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].ToLowerInvariant();
        }
        Console.WriteLine("   Было:  " + logins);
        Console.WriteLine("   Стало: " + string.Join(", ", parts));

        Console.WriteLine();
        Console.WriteLine("Задача 4. Сумма элементов по введённым номерам");
        int[] array = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
        Console.WriteLine("   Массив: " + string.Join(", ", array));
        SumByIndexes(array, "3", "5");
        SumByIndexes(array, "abc", "5");
        SumByIndexes(array, "3", "15");
        SumByIndexes(array, "0", "5");

        Console.WriteLine();
        if (!args.Contains("--no-wait"))
        {
            Console.WriteLine("Готово. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    private static void SumByIndexes(int[] array, string first, string second)
    {
        try
        {
            int firstNumber = Convert.ToInt32(first);
            int secondNumber = Convert.ToInt32(second);

            if (firstNumber < 1 || firstNumber > array.Length ||
                secondNumber < 1 || secondNumber > array.Length)
            {
                throw new IndexOutOfRangeException(
                    $"номер должен быть от 1 до {array.Length}");
            }

            int sum = array[firstNumber - 1] + array[secondNumber - 1];
            Console.WriteLine(
                $"   Номера {first} и {second}: " +
                $"{array[firstNumber - 1]} + {array[secondNumber - 1]} = {sum}");
        }
        catch (FormatException)
        {
            Console.WriteLine($"   Номера \"{first}\" и \"{second}\": " +
                              "введено не число");
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine($"   Номера \"{first}\" и \"{second}\": {ex.Message}");
        }
    }
}
