using PlugINCivil3D.Application.UseCases;
using PlugINCivil3D.Infrastructure.Civil3D;

namespace PlugINCivil3D.Bootstrap;

public static class DependencyConfig
{
    public static ListCorridorsUseCase CreateListCorridorsUseCase()
    {
        var repository = new Civil3DCorridorRepository();
        return new ListCorridorsUseCase(repository);
    }
}
