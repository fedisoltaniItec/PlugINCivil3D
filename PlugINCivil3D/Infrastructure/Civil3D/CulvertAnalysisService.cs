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
    //public Task<CulvertAnalysisResultDto> AnalyzeAsync(ObjectId alignmentId, ObjectId culvertAxisId, ObjectId surfaceId, CancellationToken cancellationToken = default)
    //{
    //    var doc = Autodesk.AutoCAD.ApplicationServices.Application
    //    .DocumentManager
    //    .MdiActiveDocument;
    //    if (doc == null)
    //        throw new InvalidOperationException("No active Civil3D document.");
    //    var db = doc.Database;
    //    using var tr = db.TransactionManager.StartTransaction();

    //    var alignment = (Alignment)tr.GetObject(alignmentId, OpenMode.ForRead);
    //    var axis = (Curve)tr.GetObject(culvertAxisId, OpenMode.ForRead);
    //    var surface = (TinSurface)tr.GetObject(surfaceId, OpenMode.ForRead);

    //    var points = new Point3dCollection();
    //    alignment.GetGeCurve().IntersectWith(axis.GetGeCurve(), Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);
    //    if (points.Count == 0)
    //    {
    //        throw new InvalidOperationException("No intersection found between alignment and culvert axis.");
    //    }

    //    var intersection = points[0];
    //    double station = 0;
    //    double offset = 0;
    //    alignment.StationOffset(intersection.X, intersection.Y, ref station, ref offset);

    //    var start = axis.StartPoint;
    //    var end = axis.EndPoint;
    //    var us = start.Z > end.Z ? start : end;
    //    var ds = us == start ? end : start;

    //    var usil = surface.FindElevationAtXY(us.X, us.Y);
    //    var dsil = surface.FindElevationAtXY(ds.X, ds.Y);
    //    var length = us.DistanceTo(ds);

    //    var alignmentDir = alignment.GetFirstDerivative(intersection).GetNormal();
    //    var axisDir = (end - start).GetNormal();
    //    var skew = alignmentDir.GetAngleTo(axisDir) * (180.0 / Math.PI);

    //    var roadElevation = surface.FindElevationAtXY(intersection.X, intersection.Y);

    //    tr.Commit();

    //    var result = new CulvertAnalysisResultDto(intersection, station, us, ds, usil, dsil, length, skew, roadElevation, alignmentId, culvertAxisId, surfaceId);
    //    _logger.LogInformation("Analysis complete at station {Station}, skew {Skew}", station, skew);
    //    return Task.FromResult(result);
    //}
    public Task<CulvertAnalysisResultDto> AnalyzeAsync(
    ObjectId alignmentId,
    ObjectId culvertAxisId,
    ObjectId surfaceId,
    CancellationToken cancellationToken = default)
    {
        var doc = Autodesk.AutoCAD.ApplicationServices.Application
            .DocumentManager
            .MdiActiveDocument ?? throw new InvalidOperationException("No active Civil3D document.");

        var db = doc.Database;

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
        // -----------------------------
        // INTERSECTION DETECTION
        // -----------------------------
        var points = new Point3dCollection();
        axis.IntersectWith(alignment, Intersect.OnBothOperands, points, IntPtr.Zero, IntPtr.Zero);

        if (points.Count == 0)
            throw new InvalidOperationException("No intersection found between alignment and culvert axis.");

        // Closest intersection to axis start
        var intersection = points.Cast<Point3d>()
            .OrderBy(p => p.DistanceTo(axis.StartPoint))
            .First();

        // -----------------------------
        // STATION CALCULATION
        // -----------------------------
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
        // Clamp station within alignment bounds
        station = Math.Max(alignment.StartingStation, Math.Min(station, alignment.EndingStation));

        // -----------------------------
        // UPSTREAM / DOWNSTREAM
        // -----------------------------
        var start = axis.StartPoint;
        var end = axis.EndPoint;

        var us = start.Z > end.Z ? start : end;
        var ds = us == start ? end : start;

        // -----------------------------
        // ELEVATIONS FROM SURFACE
        // -----------------------------
        double usil = GetSafeSurfaceElevation(surface, us);
        double dsil = GetSafeSurfaceElevation(surface, ds);
        double roadElevation = GetSafeSurfaceElevation(surface, intersection);

        var length = us.DistanceTo(ds);

        // -----------------------------
        // SKEW ANGLE (3D réel)
        // -----------------------------
        Vector3d axisDir = (ds - us).GetNormal();

        Point3d? alignPoint = GetSafeAlignmentPoint(alignment, station, surface);
        Point3d? alignPoint2 = GetSafeAlignmentPoint(alignment, station + 0.1, surface);

        double skew = 0.0;
        if (alignPoint.HasValue && alignPoint2.HasValue)
        {
            Vector3d alignmentDir = (alignPoint2.Value - alignPoint.Value).GetNormal();
            skew = alignmentDir.GetAngleTo(axisDir) * (180.0 / Math.PI);
        }

        tr.Commit();

        var result = new CulvertAnalysisResultDto(
            intersection,
            station,
            us,
            ds,
            usil,
            dsil,
            length,
            skew,
            roadElevation,
            alignmentId,
            culvertAxisId,
            surfaceId
        );

        _logger.LogInformation(
            "Analysis complete at station {Station}, skew {Skew}",
            station,
            skew);

        return Task.FromResult(result);
    }

    // -----------------------------
    // Helpers sécurisés
    // -----------------------------
    private Point3d? GetSafeAlignmentPoint(Alignment alignment, double station, TinSurface surface)
    {
        try
        {
            // Clamp station
            station = Math.Max(alignment.StartingStation, Math.Min(station, alignment.EndingStation));

            double easting = 0.0;
            double northing = 0.0;
            alignment.PointLocation(station, 0.0, ref easting, ref northing);

            // Z à partir de la surface si possible
            double z = GetSafeSurfaceElevation(surface, new Point3d(easting, northing, 0));

            return new Point3d(easting, northing, z);
        }
        catch
        {
            return null;
        }
    }

    private double GetSafeSurfaceElevation(TinSurface surface, Point3d pt)
    {
        try
        {
            // Utilise la surcharge disponible à 2 arguments
            return surface.FindElevationAtXY(pt.X, pt.Y);
        }
        catch
        {
            return double.NaN;
        }
    }
}

