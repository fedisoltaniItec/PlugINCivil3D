using PlugINCivil3D.Domain.Entities;

namespace PlugINCivil3D.Application.Interfaces;

public interface ICulvertOrchestrator
{
    Task<Culvert> PrepareAsync(CancellationToken cancellationToken = default);
    Task GenerateAsync(Culvert culvert, CancellationToken cancellationToken = default);
}
