using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(PlugINCivil3D.Plugin.CulvertPluginEntry))]
[assembly: CommandClass(typeof(PlugINCivil3D.Plugin.CulvertCommands))]
[assembly: CommandClass(typeof(PlugINCivil3D.Plugin.DiagnosticsCommands))]

