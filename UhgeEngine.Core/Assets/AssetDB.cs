namespace UhgeEngine.Core.Assets;

public class AssetDB
{
    private readonly List<AssetBundle> _assetBundles = new();
    private Dictionary<string, AssetBundle> _uuidToAssetBundleCache = new();

    public void LoadAssetBundle(AssetBundle assetBundle)
    {
        _assetBundles.Add(assetBundle);
        
        
    }
}