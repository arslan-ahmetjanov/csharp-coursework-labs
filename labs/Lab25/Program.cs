using System;
using System.Text;

namespace Lab25;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Лабораторная работа № 25. Уроки 1–2");
        Console.WriteLine("Типы данных, переменные, константы, преобразования");
        Console.WriteLine(new string('-', 60));

        // Переменные разных встроенных типов.
        byte age = 20;
        int group = 2201;
        long population = 1_000_000;
        float ratio = 0.75f;
        double pi = 3.14159;
        decimal price = 199.90m;
        char letter = 'C';
        bool isStudent = true;
        string name = "Арслан";

        Console.WriteLine("1. Переменные разных типов");
        Console.WriteLine($"   byte    age        = {age}");
        Console.WriteLine($"   int     group      = {group}");
        Console.WriteLine($"   long    population = {population}");
        Console.WriteLine($"   float   ratio      = {ratio}");
        Console.WriteLine($"   double  pi         = {pi}");
        Console.WriteLine($"   decimal price      = {price}");
        Console.WriteLine($"   char    letter     = {letter}");
        Console.WriteLine($"   bool    isStudent  = {isStudent}");
        Console.WriteLine($"   string  name       = {name}");

        Console.WriteLine();
        Console.WriteLine("2. Неявное преобразование (int -> long -> double)");
        int count = 42;
        long longCount = count;
        double doubleCount = longCount;
        Console.WriteLine($"   int {count} -> long {longCount} -> " +
                          $"double {doubleCount}");

        Console.WriteLine();
        Console.WriteLine("3. Явное преобразование (double -> int)");
        double temperature = 36.6;
        int truncated = (int)temperature;
        Console.WriteLine($"   (int)36.6 = {truncated} — " +
                          "дробная часть отбрасывается");

        Console.WriteLine();
        Console.WriteLine("4. Преобразование строки в число");
        string numberText = "123";
        int parsed = Convert.ToInt32(numberText);
        Console.WriteLine($"   Convert.ToInt32(\"{numberText}\") = {parsed}");

        Console.WriteLine();
        Console.WriteLine("5. Константа");
        const double Pi = 3.14159;
        Console.WriteLine($"   const double Pi = {Pi}");
        // Pi = 3.14; // Ошибка CS0131: константу нельзя изменить.
        Console.WriteLine("   Присваивание константе не компилируется (CS0131).");

        Console.WriteLine();
        if (!args.Contains("--no-wait"))
        {
            Console.WriteLine("Готово. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
