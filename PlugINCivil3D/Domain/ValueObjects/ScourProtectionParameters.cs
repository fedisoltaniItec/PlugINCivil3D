namespace PlugINCivil3D.Domain.ValueObjects;

public sealed record ScourProtectionParameters(
    bool IsEnabled,
    double ApronLength,
    double ApronWidth,
    double ApronThickness);
