using PlugINCivil3D.Domain.Entities;
using PlugINCivil3D.Domain.Ports;

namespace PlugINCivil3D.Application.UseCases;

public sealed class ListCorridorsUseCase(ICorridorRepository corridorRepository)
{
    public IReadOnlyList<CorridorInfo> Execute()
    {
        return corridorRepository.GetAll();
    }
}
