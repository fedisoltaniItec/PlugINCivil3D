using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using PlugINCivil3D.Application.Interfaces;

namespace PlugINCivil3D.Infrastructure.Civil3D;

public sealed class CivilObjectSelector : ICivilObjectSelector
{
    public Task<ObjectId> SelectAlignmentAsync(CancellationToken cancellationToken = default)
    {
        var options = new PromptEntityOptions("\nSelect reference alignment:");
        options.SetRejectMessage("\nOnly Alignment allowed.");
        options.AddAllowedClass(typeof(Alignment), true);
        return Task.FromResult(GetEntity(options, "Alignment not selected."));
    }

    public Task<ObjectId> SelectCulvertAxisAsync(CancellationToken cancellationToken = default)
    {
        var options = new PromptEntityOptions("\nSelect culvert axis (Polyline or Alignment):");
        options.SetRejectMessage("\nOnly Polyline or Alignment allowed.");
        options.AddAllowedClass(typeof(Polyline), true);
        options.AddAllowedClass(typeof(Alignment), true);
        return Task.FromResult(GetEntity(options, "Culvert axis not selected."));
    }

    public Task<ObjectId> SelectTinSurfaceAsync(CancellationToken cancellationToken = default)
    {
        var options = new PromptEntityOptions("\nSelect TIN Natural Ground surface:");
        options.SetRejectMessage("\nOnly TinSurface allowed.");
        options.AddAllowedClass(typeof(TinSurface), true);
        return Task.FromResult(GetEntity(options, "Surface not selected."));
    }

    private static ObjectId GetEntity(PromptEntityOptions options, string errorMessage)
    {
        var editor = Application.DocumentManager.MdiActiveDocument.Editor;
        var result = editor.GetEntity(options);
        if (result.Status != PromptStatus.OK)
        {
            throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.InvalidInput, errorMessage);
        }

        return result.ObjectId;
    }
}
