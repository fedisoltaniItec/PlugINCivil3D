using PlugINCivil3D.Domain.Entities;

namespace PlugINCivil3D.Application.Interfaces;

public interface ICulvertValidationService
{
    void ValidateCoverage(Culvert culvert, double roadSurfaceElevation, double minimumCover);
}
