using Autodesk.AutoCAD.DatabaseServices;
using PlugINCivil3D.Domain.Entities;

namespace PlugINCivil3D.Application.Interfaces;

public interface ICulvertGeometryService
{
    Task<ObjectIdCollection> GenerateAsync(Culvert culvert, ObjectId axisId, CancellationToken cancellationToken = default);
}
