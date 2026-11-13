using System;
using System.Text;

namespace Lab26;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Лабораторная работа № 26. Уроки 3–4");
        Console.WriteLine("Арифметические и логические операции, условия");
        Console.WriteLine(new string('-', 60));

        Console.WriteLine("Задача 1. Прямоугольный треугольник");
        double a = 5;
        double b = 7;
        double area = a * b / 2;
        double hypotenuse = Math.Sqrt(a * a + b * b);
        double byCosines = HypotenuseByCosines(a, b, 90);
        Console.WriteLine($"   Катеты: a = {a}, b = {b}");
        Console.WriteLine($"   Площадь: S = a * b / 2 = {area:F2}");
        Console.WriteLine($"   Гипотенуза по теореме Пифагора: c = {hypotenuse:F4}");
        Console.WriteLine($"   Гипотенуза по теореме косинусов: c = {byCosines:F4}");

        Console.WriteLine();
        Console.WriteLine("Задача 2. Результат футбольного матча");
        PrintFootballResult(2, 1);
        PrintFootballResult(0, 0);
        PrintFootballResult(1, 3);

        Console.WriteLine();
        Console.WriteLine("Задача 3. Кратность 3 и 7 одновременно");
        CheckDivisibility(21);
        CheckDivisibility(22);
        CheckDivisibility(42);
        CheckDivisibility(63);

        Console.WriteLine();
        if (!args.Contains("--no-wait"))
        {
            Console.WriteLine("Готово. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    private static double HypotenuseByCosines(double a, double b, double angleDegrees)
    {
        double angleRadians = angleDegrees * Math.PI / 180;
        return Math.Sqrt(a * a + b * b - 2 * a * b * Math.Cos(angleRadians));
    }

    private static void PrintFootballResult(int home, int away)
    {
        string result = home > away ? "победили хозяева"
            : home < away ? "победили гости"
            : "ничья";
        Console.WriteLine($"   Счёт {home}:{away} — {result}");
    }

    private static void CheckDivisibility(int number)
    {
        bool divisible = number % 3 == 0 && number % 7 == 0;
        string answer = divisible
            ? "делится на 3 и на 7"
            : "не делится на 3 и на 7 одновременно";
        Console.WriteLine($"   {number} — {answer}");
    }
}
