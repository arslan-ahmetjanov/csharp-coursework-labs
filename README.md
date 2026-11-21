# Языки программирования: лабораторные и курсовая

Учебные работы по дисциплине «Языки программирования»: УГНТУ, группа БИЦз-22-01, направление 09.03.03 «Прикладная информатика». Шесть лабораторных по основам C# — от типов данных до строк и исключений — и курсовая: консольная система учёта заявок на ИТ-поддержку с JSON-хранилищем и демонстрационным режимом.

![.NET 10](https://img.shields.io/badge/.NET-10-512bd4)
![C#](https://img.shields.io/badge/C%23-239120)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-net10.0--windows-0078d4)

## Что внутри

- `labs/` — шесть отдельных консольных проектов, каждый печатает ход решения задач; `Lab31` — форма Windows Forms.
- `coursework/RequestRegistry` — курсовая: модели и перечисления, репозиторий с фильтрами и статистикой, печать таблиц, сохранение и загрузка JSON.
- `tests/RequestRegistry.Tests` — xUnit-тесты репозитория и JSON-хранилища.
- `CSharpLabsCoursework.slnx` — одно решение, все проекты собираются одной командой.
- `Directory.Build.props` — общие свойства C# для всех проектов (`LangVersion`, `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`).

## Стек

| Слой | Технологии |
| --- | --- |
| Язык / платформа | C#, .NET 10 |
| Интерфейс | Консоль; Windows Forms в Lab31 |
| Хранение | `System.Text.Json`, файл `requests.json` в `%LOCALAPPDATA%\RequestRegistry` |
| Выборки | LINQ |
| Работа с файлами | `async`/`await`, `FileStream`, атомарная запись через временный файл |
| Тесты | xUnit, `Microsoft.NET.Test.Sdk` |

## Состав

```
CSharpLabsCoursework.slnx        # решение: 6 лабораторных, курсовая, тесты
Directory.Build.props            # общие свойства сборки
labs/                            # Lab25, 26, 27, 28, 29, 31
coursework/RequestRegistry/
├── Models/                      # Request, RequestPriority, RequestStatus
├── Services/                    # RequestRepository, RequestPrinter
└── Program.cs                   # меню, демо-режим, разбор ввода
tests/RequestRegistry.Tests/     # тесты репозитория и хранилища
```

## Сборка и запуск

```powershell
dotnet build
dotnet test
dotnet run --project labs/Lab25
dotnet run --project coursework/RequestRegistry
```

Без флагов курсовая открывает меню, с `--demo` показывает всё без ручного ввода:

```powershell
dotnet run --project coursework/RequestRegistry -- --demo
```

Данные курсовой лежат в `%LOCALAPPDATA%\RequestRegistry`: меню работает с `requests.json`,
демо-режим пишет в отдельный `requests.demo.json` и рабочий файл не трогает.

## Известные ограничения

- `Lab31` собирается на любой ОС (`EnableWindowsTargeting`), но запускается только под Windows; окно фиксированного размера (`FixedSingle`).
- JSON-файл курсовой хранится в профиле пользователя (`%LOCALAPPDATA%\RequestRegistry`), поэтому не пропадает после `dotnet clean`; если файла нет, список заявок начинается с пустого.
- Сохранение атомарное: данные пишутся во временный файл и подменяют целевой, но одновременная запись из двух процессов не поддерживается.
- Лабораторной № 30 в наборе нет.
- Тесты запускаются в CI после сборки: `dotnet build` и `dotnet test` в одном workflow; локально — `dotnet test`.

## Автор

Ахметжанов Арслан Ринатович, группа БИЦз-22-01, УГНТУ, направление 09.03.03 «Прикладная информатика».

## Лицензия

Код распространяется по лицензии MIT — см. файл `LICENSE`.
