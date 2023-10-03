/* BPY
* This scripts demonstrates calling and using bpy from within C# through pythonnet
*/

Import(pythonnet as Python.Runtime)

Runtime.PythonDLL = "python310.dll"; // Blender supports only Python 3.10, requires installation of bpy through pip
PythonEngine.Initialize();

using (Py.GIL())
{
    string path = Path.GetFullPath("my.blend");

    dynamic bpy = Py.Import("bpy");
    bpy.ops.wm.save_as_mainfile(filepath: path);
    bpy.ops.wm.open_mainfile(filepath: path);
    bpy.context.scene.render.filepath = "//Hello_World.png";
    bpy.ops.render.render(write_still: true);
}

PythonEngine.Shutdown();