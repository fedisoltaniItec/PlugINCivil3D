using System.IO;

using System.Text;
using PlugINCivil3D.Application.DTOs;
using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Domain.Entities;

namespace PlugINCivil3D.Application.Services;

public sealed class CulvertReportService : ICulvertReportService
{
    public async Task<CulvertReportDto> ExportAsync(Culvert culvert, string directory, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, $"{culvert.Id}_report.txt");
        var content = new StringBuilder()
            .AppendLine($"ID: {culvert.Id}")
            .AppendLine($"Type: {culvert.Type}")
            .AppendLine($"Material: {culvert.Material}")
            .AppendLine($"Station: {culvert.Station:F3}")
            .AppendLine($"Length: {culvert.Length:F3}")
            .AppendLine($"USIL: {culvert.Usil:F3}")
            .AppendLine($"DSIL: {culvert.Dsil:F3}")
            .AppendLine($"Slope: {culvert.Slope:P4}")
            .AppendLine($"Skew: {culvert.SkewAngle:F3}")
            .AppendLine($"Cover: {culvert.CoverHeight:F3}")
            .AppendLine($"Warning: {culvert.Warning}")
            .ToString();

        await File.WriteAllTextAsync(filePath, content, cancellationToken);
        return new CulvertReportDto(filePath, content);
    }
}
