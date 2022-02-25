using System.Diagnostics;
using UhgeEngine.Core.Assets;

namespace UhgeEngine.Tools.Editor;

public class EditorProject
{
    public static EditorProject Load(string path)
    {
        var projectFilePath = Path.GetFullPath(path);
        var projectAssetBundle = LoadProjectAssetBundle(projectFilePath);

        var project = new EditorProject("Game"); // TODO: Replace with project's name
        project.LoadAssetBundle("Game", projectAssetBundle);
        return project;
    }
    
    private static AssetBundle LoadProjectAssetBundle(string projectFilePath)
    {
        projectFilePath = Path.GetFullPath(projectFilePath);
        
        Debug.Assert(File.Exists(projectFilePath), "Unable to find project file {0}", projectFilePath);

        var projectFolderPath = Path.GetDirectoryName(projectFilePath)?? throw new Exception("Unable to get directory of project file.");
        
        var assetBundle = new FsAssetBundle(Path.Combine(projectFilePath, "Content"));

        return assetBundle;
    }
    
    public string Name { get; set; }
    public IEnumerable<string> AssetBundleNames => _assetManager.AssetBundleNames;

    private readonly EditorAssetManager _assetManager = new ();

    public EditorProject(string name)
    {
        Name = name;
    }

    public void LoadAssetBundle(string name, AssetBundle assetBundle) => _assetManager.LoadAssetBundle(name, assetBundle);

    public AssetBundle GetAssetBundle(string name) => _assetManager[name];
}