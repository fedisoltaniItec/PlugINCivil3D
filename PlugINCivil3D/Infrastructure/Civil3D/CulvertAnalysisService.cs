using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Microsoft.Extensions.Logging;
using PlugINCivil3D.Application.DTOs;
using PlugINCivil3D.Application.Interfaces;

namespace PlugINCivil3D.Infrastructure.Civil3D;

public sealed class CulvertAnalysisService : ICulvertAnalysisService
{
    private readonly ILogger<CulvertAnalysisService> _logger;

    public CulvertAnalysisService(ILogger<CulvertAnalysisService> logger) => _logger = logger;

    public Task<CulvertAnalysisResultDto> AnalyzeAsync(ObjectId alignmentId, ObjectId culvertAxisId, ObjectId surfaceId, CancellationToken cancellationToken = default)
    {
        var db = Application.DocumentManager.MdiActiveDocument.Database;
        using var tr = db.TransactionManager.StartTransaction();

        var alignment = (Alignment)tr.GetObject(alignmentId, OpenMode.ForRead);
        var axis = (Curve)tr.GetObject(culvertAxisId, OpenMode.ForRead);
        var surface = (TinSurface)tr.GetObject(surfaceId, OpenMode.ForRead);

        var points = new Point3dCollection();
        alignment.GetGeCurve().IntersectWith(axis.GetGeCurve(), Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
        if (points.Count == 0)
        {
            throw new InvalidOperationException("No intersection found between alignment and culvert axis.");
        }

        var intersection = points[0];
        double station = 0;
        double offset = 0;
        alignment.StationOffset(intersection.X, intersection.Y, ref station, ref offset);

        var start = axis.StartPoint;
        var end = axis.EndPoint;
        var us = start.Z > end.Z ? start : end;
        var ds = us == start ? end : start;

        var usil = surface.FindElevationAtXY(us.X, us.Y);
        var dsil = surface.FindElevationAtXY(ds.X, ds.Y);
        var length = us.DistanceTo(ds);

        var alignmentDir = alignment.GetFirstDerivative(intersection).GetNormal();
        var axisDir = (end - start).GetNormal();
        var skew = alignmentDir.GetAngleTo(axisDir) * (180.0 / Math.PI);

        var roadElevation = surface.FindElevationAtXY(intersection.X, intersection.Y);

        tr.Commit();

        var result = new CulvertAnalysisResultDto(intersection, station, us, ds, usil, dsil, length, skew, roadElevation, alignmentId, culvertAxisId, surfaceId);
        _logger.LogInformation("Analysis complete at station {Station}, skew {Skew}", station, skew);
        return Task.FromResult(result);
    }
}
