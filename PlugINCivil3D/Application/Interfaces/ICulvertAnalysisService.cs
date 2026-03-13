using Autodesk.AutoCAD.DatabaseServices;
using PlugINCivil3D.Application.DTOs;

namespace PlugINCivil3D.Application.Interfaces;

public interface ICulvertAnalysisService
{
    Task<CulvertAnalysisResultDto> AnalyzeAsync(ObjectId alignmentId, ObjectId culvertAxisId, ObjectId surfaceId, CancellationToken cancellationToken = default);
}
