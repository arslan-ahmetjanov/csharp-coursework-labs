using System;
using System.IO;

namespace RequestRegistry.Tests;

/// <summary>Временный каталог для тестов сохранения и загрузки JSON.</summary>
internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Root = Path.Combine(Path.GetTempPath(), "RequestRegistry.Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public string FilePath(string fileName) => Path.Combine(Root, fileName);

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
        catch (IOException)
        {
            // Каталог мог остаться занятым (антивирус, индексатор) — на результат теста это не влияет.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
