using Autodesk.AutoCAD.Runtime;

namespace PlugINCivil3D.Plugin;

public sealed class CulvertPluginEntry : IExtensionApplication
{
    public void Initialize() => _ = CompositionRoot.Provider;

    public void Terminate()
    {
    }
}
