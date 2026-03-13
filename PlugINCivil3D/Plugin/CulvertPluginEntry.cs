using Autodesk.AutoCAD.Runtime;

namespace PlugINCivil3D.Plugin;

public sealed class CulvertPluginEntry : IExtensionApplication
{
    public void Initialize()
    {
        PluginDiagnostics.Initialize();
        PluginDiagnostics.Log("CulvertPluginEntry.Initialize start");

        try
        {
            _ = CompositionRoot.Provider;
            PluginDiagnostics.WriteToEditor("PlugINCivil3D initialized.");
            PluginDiagnostics.Log("CulvertPluginEntry.Initialize success");
        }
        catch (System.Exception ex)
        {
            PluginDiagnostics.Log("CulvertPluginEntry.Initialize failure", ex);
            PluginDiagnostics.WriteToEditor($"PlugINCivil3D init failed: {ex.Message}");
            throw;
        }
    }

    public void Terminate()
    {
    }
}
