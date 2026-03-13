using Autodesk.AutoCAD.DatabaseServices;

namespace PlugINCivil3D.Application.Interfaces;

public interface ICivilObjectSelector
{
    Task<ObjectId> SelectAlignmentAsync(CancellationToken cancellationToken = default);
    Task<ObjectId> SelectCulvertAxisAsync(CancellationToken cancellationToken = default);
    Task<ObjectId> SelectTinSurfaceAsync(CancellationToken cancellationToken = default);
}
