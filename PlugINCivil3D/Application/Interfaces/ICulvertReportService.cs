using PlugINCivil3D.Application.DTOs;
using PlugINCivil3D.Domain.Entities;

namespace PlugINCivil3D.Application.Interfaces;

public interface ICulvertReportService
{
    Task<CulvertReportDto> ExportAsync(Culvert culvert, string directory, CancellationToken cancellationToken = default);
}
