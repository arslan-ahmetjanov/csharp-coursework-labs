# Языки программирования: лабораторные и курсовая

Учебные работы по дисциплине «Языки программирования»: УГНТУ, группа БИЦз-22-01, направление 09.03.03 «Прикладная информатика». Шесть лабораторных по основам C# — от типов данных до строк и исключений — и курсовая: консольная система учёта заявок на ИТ-поддержку с JSON-хранилищем и демонстрационным режимом.

![.NET 10](https://img.shields.io/badge/.NET-10-512bd4)
![C#](https://img.shields.io/badge/C%23-239120)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-net10.0--windows-0078d4)

## Что внутри

- `labs/` — шесть отдельных консольных проектов, каждый печатает ход решения задач; `Lab31` — форма Windows Forms.
- `coursework/RequestRegistry` — курсовая: модели и перечисления, репозиторий с фильтрами и статистикой, печать таблиц, сохранение и загрузка JSON.
- `CSharpLabsCoursework.slnx` — одно решение, все проекты собираются одной командой.

## Стек

| Слой | Технологии |
| --- | --- |
| Язык / платформа | C#, .NET 10 |
| Интерфейс | Консоль; Windows Forms в Lab31 |
| Хранение | `System.Text.Json`, файл `requests.json` |
| Выборки | LINQ |
| Работа с файлами | `async`/`await`, `FileStream` |

## Состав

```
CSharpLabsCoursework.slnx        # решение: 6 лабораторных и курсовая
labs/                            # Lab25, 26, 27, 28, 29, 31
coursework/RequestRegistry/
├── Models/                      # Request, RequestPriority, RequestStatus
├── Services/                    # RequestRepository, RequestPrinter
└── Program.cs                   # меню, демо-режим, разбор ввода
```

## Сборка и запуск

```powershell
dotnet build
dotnet run --project labs/Lab25
dotnet run --project coursework/RequestRegistry
```

Без флагов курсовая открывает меню, с `--demo` показывает всё без ручного ввода:

```powershell
dotnet run --project coursework/RequestRegistry -- --demo
```

## Известные ограничения

- `Lab31` собирается только под Windows; окно фиксированного размера (`FixedSingle`).
- Данные курсовой лежат в `requests.json` рядом с исполняемым файлом и пропадают после `dotnet clean`.
- Лабораторной № 30 в наборе нет; автотестов тоже нет.

## Автор

Ахметжанов Арслан Ринатович, группа БИЦз-22-01, УГНТУ, направление 09.03.03 «Прикладная информатика».

## Лицензия

Код распространяется по лицензии MIT — см. файл `LICENSE`.
