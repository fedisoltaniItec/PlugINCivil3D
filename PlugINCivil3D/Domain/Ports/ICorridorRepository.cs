using PlugINCivil3D.Domain.Entities;

namespace PlugINCivil3D.Domain.Ports;

public interface ICorridorRepository
{
    IReadOnlyList<CorridorInfo> GetAll();
}
