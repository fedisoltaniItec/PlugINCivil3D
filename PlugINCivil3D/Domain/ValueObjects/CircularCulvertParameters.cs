namespace PlugINCivil3D.Domain.ValueObjects;

public sealed record CircularCulvertParameters(
    double InternalDiameter,
    double WallThickness,
    int NumberOfPipes);
