using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace PlugINCivil3D.Plugin;

public static class DiagnosticsCommands
{
    [CommandMethod("C3DHELLO", CommandFlags.Session)]
    public static void Hello()
    {
        var editor = Application.DocumentManager.MdiActiveDocument.Editor;
        editor.WriteMessage("\nC3DHELLO OK - command registration works.");
    }
}
