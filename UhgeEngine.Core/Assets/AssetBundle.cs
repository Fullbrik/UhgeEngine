using System.Diagnostics;

namespace UhgeEngine.Core.Assets;

public abstract class AssetBundle
{
    public abstract bool IsReadOnly { get; }

    public abstract IEnumerable<string> AllClaimedUUIDs { get; }

    public Asset LoadAsset(string uuid, Type assetType)
    {
        AssertIsValidAssetType(assetType);

        // Construct asset
        var constructor = assetType.GetConstructor(Array.Empty<Type>());
        if (constructor == null)
            throw new Exception(
                $"Failed to load asset of type {assetType.Name}: Cannot find asset constructor with zero parameters.");
        var asset = constructor.Invoke(Array.Empty<object>()) as Asset;
        Debug.Assert(asset != null, nameof(asset) + " != null");
        asset.UUID = uuid;

        // Insert data
        var assetFileType = CheckAssetType(uuid);

        switch (assetFileType)
        {
            case AssetFileType.Text:
                asset.DeserializeFrom(ReadAssetString(uuid));
                break;
            case AssetFileType.CompressedBytes:
                asset.DeserializeFrom(ReadAssetBytes(uuid));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return asset;
    }

    private static void AssertIsValidAssetType(Type assetType)
    {
        if (assetType.IsSubclassOf(typeof(Asset)) && !assetType.IsAbstract)
            throw new Exception($"Invalid asset type {assetType.Name}");
    }

    protected abstract AssetFileType CheckAssetType(string uuid);
    protected abstract byte[] ReadAssetBytes(string uuid);
    protected abstract string ReadAssetString(string uuid);
}

public class FsAssetBundle : AssetBundle
{
    private struct FsAssetInfo
    {
        public string Path;
        public AssetFileType AssetFileType;

        public FsAssetInfo(string path, AssetFileType assetFileType)
        {
            Path = path;
            AssetFileType = assetFileType;
        }
    }
    
    public override bool IsReadOnly => false;
    public override IEnumerable<string> AllClaimedUUIDs { get; }

    private readonly string _contentDirectory;

    private readonly Dictionary<string, FsAssetInfo> _assetInfos = new();

    public FsAssetBundle(string contentDirectory)
    {
        _contentDirectory = contentDirectory;
        LoadInfoDB();
    }

    private void LoadInfoDB()
    {
        
    }

    protected override AssetFileType CheckAssetType(string uuid)
    {
        return _assetInfos[uuid].AssetFileType;
    }

    protected override byte[] ReadAssetBytes(string uuid)
    {
        var path = GetPathFromUuid(uuid);
        return File.ReadAllBytes(path);
    }

    protected override string ReadAssetString(string uuid)
    {
        var path = GetPathFromUuid(uuid);
        return File.ReadAllText(path);
    }

    private string GetPathFromUuid(string uuid)
    {
        var info = _assetInfos[uuid];
        var path = Path.GetFullPath(Path.Combine(_contentDirectory, info.Path));
        return path;
    }
}