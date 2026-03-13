using Microsoft.Extensions.Logging;
using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Domain.Entities;
using PlugINCivil3D.Domain.Enums;
using PlugINCivil3D.Domain.ValueObjects;

namespace PlugINCivil3D.Application.Services;

public sealed class CulvertOrchestrator : ICulvertOrchestrator
{
    private readonly ICivilObjectSelector _selector;
    private readonly ICulvertAnalysisService _analysisService;
    private readonly ICulvertIdGenerator _idGenerator;
    private readonly ILogger<CulvertOrchestrator> _logger;
    private readonly ICulvertGeometryService _geometryService;

    private Autodesk.AutoCAD.DatabaseServices.ObjectId _axisId;

    public CulvertOrchestrator(
        ICivilObjectSelector selector,
        ICulvertAnalysisService analysisService,
        ICulvertIdGenerator idGenerator,
        ICulvertGeometryService geometryService,
        ILogger<CulvertOrchestrator> logger)
    {
        _selector = selector;
        _analysisService = analysisService;
        _idGenerator = idGenerator;
        _geometryService = geometryService;
        _logger = logger;
    }

    public async Task<Culvert> PrepareAsync(CancellationToken cancellationToken = default)
    {
        var alignmentId = await _selector.SelectAlignmentAsync(cancellationToken);
        _axisId = await _selector.SelectCulvertAxisAsync(cancellationToken);
        var surfaceId = await _selector.SelectTinSurfaceAsync(cancellationToken);

        var analysis = await _analysisService.AnalyzeAsync(alignmentId, _axisId, surfaceId, cancellationToken);

        var culvert = new Culvert
        {
            Id = _idGenerator.Next(),
            Material = CulvertMaterial.Concrete,
            Type = CulvertType.Box,
            Station = analysis.Station,
            Length = analysis.Length,
            Usil = analysis.Usil,
            Dsil = analysis.Dsil,
            SkewAngle = analysis.SkewAngle,
            BoxParameters = new BoxCulvertParameters(2, 2, 0.25, 0.25, 0.25, 0.25, 1),
            CircularParameters = new CircularCulvertParameters(1.5, 0.12, 1),
            InletScourProtection = new ScourProtectionParameters(false, 3, 3, 0.25),
            OutletScourProtection = new ScourProtectionParameters(false, 3, 3, 0.25)
        };
        culvert.UpdateSlope();
        _logger.LogInformation("Prepared culvert {Id} at station {Station}", culvert.Id, culvert.Station);
        return culvert;
    }

    public Task GenerateAsync(Culvert culvert, CancellationToken cancellationToken = default)
        => _geometryService.GenerateAsync(culvert, _axisId, cancellationToken);
}
