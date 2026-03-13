using PlugINCivil3D.Domain.Enums;
using PlugINCivil3D.Domain.ValueObjects;

namespace PlugINCivil3D.Domain.Entities;

public sealed class Culvert
{
    public required string Id { get; set; }
    public CulvertMaterial Material { get; set; }
    public CulvertType Type { get; set; }
    public double Station { get; set; }
    public double Length { get; set; }
    public double Usil { get; set; }
    public double Dsil { get; set; }
    public double Slope { get; private set; }
    public double SkewAngle { get; set; }
    public double CoverHeight { get; set; }
    public bool Warning { get; set; }
    public double HeadwallHeight { get; set; }
    public double HeadwallHorizontalDistance { get; set; }
    public BoxCulvertParameters? BoxParameters { get; set; }
    public CircularCulvertParameters? CircularParameters { get; set; }
    public ScourProtectionParameters? InletScourProtection { get; set; }
    public ScourProtectionParameters? OutletScourProtection { get; set; }

    public void UpdateSlope() => Slope = Length <= 0 ? 0 : (Usil - Dsil) / Length;
}
