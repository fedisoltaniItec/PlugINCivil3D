using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace PlugINCivil3D.Application.DTOs;

public sealed record CulvertAnalysisResultDto(
    Point3d IntersectionPoint,
    double Station,
    Point3d UpstreamPoint,
    Point3d DownstreamPoint,
    double Usil,
    double Dsil,
    double Length,
    double SkewAngle,
    double RoadSurfaceElevation,
    ObjectId AlignmentId,
    ObjectId AxisId,
    ObjectId SurfaceId);
