using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Microsoft.Extensions.DependencyInjection;
using PlugINCivil3D.Application.Interfaces;
using PlugINCivil3D.Presentation.ViewModels;
using PlugINCivil3D.Presentation.Views;

namespace PlugINCivil3D.Plugin;

public sealed class CulvertCommands
{
    [CommandMethod("CULVERTPING")]
    public void Ping()
    {
        var editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
        editor.WriteMessage("\nPlugINCivil3D loaded. CULVERTCREATE is available.");
    }

    [CommandMethod("CULVERTCREATE")]
    public async void CreateCulvert()
    {
        try
        {
            var provider = CompositionRoot.Provider;
            var orchestrator = provider.GetRequiredService<ICulvertOrchestrator>();
            var validator = provider.GetRequiredService<ICulvertValidationService>();
            var report = provider.GetRequiredService<ICulvertReportService>();

            var culvert = await orchestrator.PrepareAsync();
            validator.ValidateCoverage(culvert, culvert.Usil + 2.0, 0.6);

            var vm = new CulvertViewModel(culvert, culvert.Usil + 2.0, validator, orchestrator, report);
            var window = new CulvertWindow(vm);
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(window);
        }
        catch (System.Exception ex)
        {
            var editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            editor.WriteMessage($"\nCULVERTCREATE failed: {ex.Message}");
            _logger.LogError(ex, "Failed to create culvert.");
            Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(
                "Erreur lors de la création du culvert : " + ex.Message);
        }
    }
}
