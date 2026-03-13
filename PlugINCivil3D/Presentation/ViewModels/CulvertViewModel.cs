using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Domain.Entities;
using PlugINCivil3D.Domain.Enums;
using PlugINCivil3D.Domain.ValueObjects;
using PlugINCivil3D.Presentation.Commands;

namespace PlugINCivil3D.Presentation.ViewModels;

public sealed class CulvertViewModel : INotifyPropertyChanged
{
    private readonly ICulvertValidationService _validationService;
    private readonly ICulvertOrchestrator _orchestrator;
    private readonly ICulvertReportService _reportService;
    private readonly double _roadSurfaceElevation;

    public CulvertViewModel(Culvert culvert, double roadSurfaceElevation, ICulvertValidationService validationService, ICulvertOrchestrator orchestrator, ICulvertReportService reportService)
    {
        Model = culvert;
        _roadSurfaceElevation = roadSurfaceElevation;
        _validationService = validationService;
        _orchestrator = orchestrator;
        _reportService = reportService;
        GenerateCommand = new RelayCommand(async _ => await _orchestrator.GenerateAsync(Model), _ => !Model.Warning);
        ExportReportCommand = new RelayCommand(async _ => await _reportService.ExportAsync(Model, Path.GetTempPath()));
        ValidateCommand = new RelayCommand(_ => Recalculate());
    }

    public Culvert Model { get; }
    public ICommand GenerateCommand { get; }
    public ICommand ExportReportCommand { get; }
    public ICommand ValidateCommand { get; }

    public bool IsBoxVisible => Model.Type == CulvertType.Box;
    public bool IsCircularVisible => Model.Type == CulvertType.Circular;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetType(CulvertType type)
    {
        Model.Type = type;
        ApplyDefaultScourThickness();
        Notify(nameof(IsBoxVisible), nameof(IsCircularVisible));
    }

    public void SetUsil(double value)
    {
        Model.Usil = value;
        Recalculate();
    }

    public void SetDsil(double value)
    {
        Model.Dsil = value;
        Recalculate();
    }

    public void SetLength(double value)
    {
        Model.Length = value;
        Recalculate();
    }

    public void SetBoxParameters(BoxCulvertParameters parameters)
    {
        Model.BoxParameters = parameters;
        ApplyDefaultScourThickness();
        Recalculate();
    }

    public void SetCircularParameters(CircularCulvertParameters parameters)
    {
        Model.CircularParameters = parameters;
        ApplyDefaultScourThickness();
        Recalculate();
    }

    private void Recalculate()
    {
        Model.UpdateSlope();
        _validationService.ValidateCoverage(Model, _roadSurfaceElevation, minimumCover: 0.6);
        Notify(nameof(Model));
    }

    private void ApplyDefaultScourThickness()
    {
        var thickness = Model.Type == CulvertType.Box
            ? Model.BoxParameters?.BottomSlabThickness ?? 0.2
            : Model.CircularParameters?.WallThickness ?? 0.2;

        Model.InletScourProtection = Model.InletScourProtection with { ApronThickness = thickness };
        Model.OutletScourProtection = Model.OutletScourProtection with { ApronThickness = thickness };
    }

    private void Notify(params string[] names)
    {
        foreach (var name in names)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
