using PlugINCivil3D.Domain.Entities;
using PlugINCivil3D.Domain.Ports;

namespace PlugINCivil3D.Infrastructure.Civil3D;

public sealed class Civil3DCorridorRepository : ICorridorRepository
{
    public IReadOnlyList<CorridorInfo> GetAll()
    {
        // TODO: Replace with Civil 3D API implementation.
        return Array.Empty<CorridorInfo>();
    }
}
