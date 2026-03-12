using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using PlugINCivil3D.Bootstrap;

namespace PlugINCivil3D.Presentation.Commands;

public sealed class CorridorCommands
{
    [CommandMethod("PI_LIST_CORRIDORS")]
    public void ListCorridors()
    {
        var useCase = DependencyConfig.CreateListCorridorsUseCase();
        var corridors = useCase.Execute();

        var editor = Application.DocumentManager.MdiActiveDocument.Editor;
        editor.WriteMessage($"\\nFound {corridors.Count} corridor(s).");

        foreach (var corridor in corridors)
        {
            editor.WriteMessage($"\\n- {corridor.Name} (Baselines: {corridor.BaselineCount})");
        }
    }
}
