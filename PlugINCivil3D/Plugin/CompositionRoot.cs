using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Application.Services;
using PlugINCivil3D.Infrastructure.Civil3D;
using PlugINCivil3D.Infrastructure.Geometry;

namespace PlugINCivil3D.Plugin;

public static class CompositionRoot
{
    private static ServiceProvider? _provider;

    public static ServiceProvider Provider => _provider ??= BuildProvider();

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddDebug());
        services.AddSingleton<ICivilObjectSelector, CivilObjectSelector>();
        services.AddSingleton<ICulvertAnalysisService, CulvertAnalysisService>();
        services.AddSingleton<ICulvertValidationService, CulvertValidationService>();
        services.AddSingleton<ICulvertGeometryService, CulvertGeometryService>();
        services.AddSingleton<ICulvertIdGenerator, InMemoryCulvertIdGenerator>();
        services.AddSingleton<ICulvertReportService, CulvertReportService>();
        services.AddSingleton<ICulvertOrchestrator, CulvertOrchestrator>();
        return services.BuildServiceProvider();
    }
}
