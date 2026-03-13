using System.Runtime.ExceptionServices;
using Autodesk.AutoCAD.ApplicationServices;

namespace PlugINCivil3D.Plugin;

internal static class PluginDiagnostics
{
    private static int _isInitialized;
    private static readonly string _logDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PlugINCivil3D");

    public static string LogFilePath => Path.Combine(_logDirectory, "plugin.log");

    public static void Initialize()
    {
        if (Interlocked.Exchange(ref _isInitialized, 1) == 1)
        {
            return;
        }

        Directory.CreateDirectory(_logDirectory);
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            Log("UnhandledException", args.ExceptionObject as Exception);

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            Log("UnobservedTaskException", args.Exception);
            args.SetObserved();
        };

        AppDomain.CurrentDomain.FirstChanceException += (_, args) =>
        {
            if (args.Exception is not null)
            {
                Log("FirstChanceException", args.Exception);
            }
        };

        Log("PluginDiagnostics initialized");
    }

    public static void Log(string message, Exception? ex = null)
    {
        var line = $"[{DateTime.Now:O}] {message}";
        if (ex is not null)
        {
            line += Environment.NewLine + ex + Environment.NewLine;
        }

        try
        {
            File.AppendAllText(LogFilePath, line + Environment.NewLine);
        }
        catch
        {
            // intentionally swallow to avoid recursive failures
        }
    }

    public static void WriteToEditor(string message)
    {
        try
        {
            var editor = Application.DocumentManager.MdiActiveDocument?.Editor;
            editor?.WriteMessage("\n" + message);
        }
        catch
        {
            // no-op
        }
    }
}
