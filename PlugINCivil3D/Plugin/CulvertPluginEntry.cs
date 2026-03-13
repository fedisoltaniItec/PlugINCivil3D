using Autodesk.AutoCAD.Runtime;

namespace PlugINCivil3D.Plugin;

public sealed class CulvertPluginEntry : IExtensionApplication
{
    public void Initialize()
    {
        _ = CompositionRoot.Provider;
        try
        {
            var editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument?.Editor;
            editor?.WriteMessage("\nPlugINCivil3D initialized.");
        }
        catch
        {
            // no-op to avoid load breakage in startup contexts
        }
    }

    public void Terminate()
    {
    }
}
