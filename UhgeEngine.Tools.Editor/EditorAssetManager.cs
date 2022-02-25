using UhgeEngine.Core.Assets;

namespace UhgeEngine.Tools.Editor;

public class EditorAssetManager
{
    public IEnumerable<string> AssetBundleNames => _assetBundles.Keys;
    private readonly Dictionary<string, AssetBundle> _assetBundles = new();

    public void LoadAssetBundle(string name, AssetBundle assetBundle)
    {
        _assetBundles.Add(name, assetBundle);
    }

    public AssetBundle this[string name] => _assetBundles[name];
}