namespace PlugINCivil3D.Domain.ValueObjects;

public sealed record BoxCulvertParameters(
    double Width,
    double Height,
    double WallThicknessLeft,
    double WallThicknessRight,
    double TopSlabThickness,
    double BottomSlabThickness,
    int NumberOfVents);
