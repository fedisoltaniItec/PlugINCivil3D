using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Domain.Entities;
using PlugINCivil3D.Domain.Enums;

namespace PlugINCivil3D.Application.Services;

public sealed class CulvertValidationService : ICulvertValidationService
{
    public void ValidateCoverage(Culvert culvert, double roadSurfaceElevation, double minimumCover)
    {
        var top = culvert.Type switch
        {
            CulvertType.Box => culvert.Usil + (culvert.BoxParameters?.Height ?? 0) + (culvert.BoxParameters?.TopSlabThickness ?? 0),
            CulvertType.Circular => culvert.Usil + (culvert.CircularParameters?.InternalDiameter ?? 0) + (culvert.CircularParameters?.WallThickness ?? 0),
            _ => culvert.Usil
        };

        culvert.CoverHeight = roadSurfaceElevation - top;
        culvert.Warning = culvert.CoverHeight < minimumCover;
    }
}
