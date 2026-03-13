using System.Globalization;
using PlugINCivil3D.Application.Interfaces;

namespace PlugINCivil3D.Application.Services;

public sealed class InMemoryCulvertIdGenerator : ICulvertIdGenerator
{
    private int _counter;

    public string Next() => $"CUL-{Interlocked.Increment(ref _counter).ToString("000", CultureInfo.InvariantCulture)}";
}
