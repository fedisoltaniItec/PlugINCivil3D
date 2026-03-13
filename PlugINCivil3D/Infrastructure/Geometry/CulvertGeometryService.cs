using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Microsoft.Extensions.Logging;
using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Domain.Entities;
using PlugINCivil3D.Domain.Enums;

namespace PlugINCivil3D.Infrastructure.Geometry;

public sealed class CulvertGeometryService : ICulvertGeometryService
{
    private readonly ILogger<CulvertGeometryService> _logger;

    public CulvertGeometryService(ILogger<CulvertGeometryService> logger) => _logger = logger;

    public Task<ObjectIdCollection> GenerateAsync(Culvert culvert, ObjectId axisId, CancellationToken cancellationToken = default)
    {
        var doc = Autodesk.AutoCAD.ApplicationServices.Application
            .DocumentManager
            .MdiActiveDocument;

        if (doc == null)
            throw new InvalidOperationException("No active Civil3D document.");
        var db = doc.Database;
        var ids = new ObjectIdCollection();

        using var tr = db.TransactionManager.StartTransaction();
        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        var ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
        var axis = (Curve)tr.GetObject(axisId, OpenMode.ForRead);

        var start = axis.StartPoint;
        var end = axis.EndPoint;
        var direction = (end - start).GetNormal();

        if (culvert.Type == CulvertType.Box && culvert.BoxParameters is not null)
        {
            var p = culvert.BoxParameters;
            ids.Add(AppendBox(ms, tr, p.Width * p.NumberOfVents, p.WallThicknessLeft + p.Height + p.TopSlabThickness, culvert.Length, start, direction));
        }
        else if (culvert.Type == CulvertType.Circular && culvert.CircularParameters is not null)
        {
            var p = culvert.CircularParameters;
            for (var i = 0; i < p.NumberOfPipes; i++)
            {
                var offset = Vector3d.YAxis * i * (p.InternalDiameter + p.WallThickness * 2 + 0.25);
                ids.Add(AppendCylinder(ms, tr, p.InternalDiameter / 2 + p.WallThickness, culvert.Length, start + offset, direction));
            }
        }

        tr.Commit();
        _logger.LogInformation("Generated {Count} solids for culvert {Id}", ids.Count, culvert.Id);
        return Task.FromResult(ids);
    }

    private static ObjectId AppendBox(BlockTableRecord ms, Transaction tr, double width, double height, double length, Point3d origin, Vector3d direction)
    {
        var solid = new Solid3d();
        solid.SetDatabaseDefaults();
        solid.CreateBox(length, width, height);
        var cs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, origin, direction, Vector3d.ZAxis.CrossProduct(direction), Vector3d.ZAxis);
        solid.TransformBy(cs);
        ms.AppendEntity(solid);
        tr.AddNewlyCreatedDBObject(solid, true);
        return solid.ObjectId;
    }

    private static ObjectId AppendCylinder(BlockTableRecord ms, Transaction tr, double radius, double length, Point3d origin, Vector3d direction)
    {
        var solid = new Solid3d();
        solid.SetDatabaseDefaults();
        solid.CreateFrustum(length, radius, radius, radius);
        var cs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.ZAxis, Vector3d.XAxis, Vector3d.YAxis, origin, direction, Vector3d.XAxis, Vector3d.YAxis);
        solid.TransformBy(cs);
        ms.AppendEntity(solid);
        tr.AddNewlyCreatedDBObject(solid, true);
        return solid.ObjectId;
    }
}
