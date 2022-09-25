using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirius.Data.Resource;
using Sirius.Engine;

public class ResourceManager : Singleton<ResourceManager>
{
    private string ResourcePath => Sirius.Engine.Framework.FileUtil.Manager.StreamingAssetsPath + "/MekimekiResources/Resources";
    private Dictionary<string, Sirius.Data.Resource.SpriteInfo> SpriteInfoCache;
    private Dictionary<string, object> _cache = new Dictionary<string, object>();
    public ResourceManager()
    {
        SpriteInfoCache = new Dictionary<string, SpriteInfo>();
    }

    public SpriteInfo LoadImage(string path)
    {
        var awaiter = ResourceManager.Instance.LoadImageAsync(path).GetAwaiter();
        while (!awaiter.IsCompleted)
        {
        }
        return awaiter.GetResult();
    }
    
    public async UniTask<SpriteInfo> LoadImageAsync(string path)
    {
        if (SpriteInfoCache.ContainsKey(path))
        {
            return SpriteInfoCache[path];
        }
        
        var info = await Sirius.Engine.Framework.Resource.Manager
            .ReadTextureBuffer($"{ResourcePath}/{path}");
        if (!SpriteInfoCache.ContainsKey(path))
        {
            SpriteInfoCache.Add(path, info);
        }
        return info;
    }

    public async UniTask<SpriteInfo> LoadImageFullPath(string path)
    {
        var info = await Sirius.Engine.Framework.Resource.Manager
            .ReadTextureBuffer(path);
        return info;
    }

    public T LoadFromJson<T>(string resourcePath) where T : class, new()
    {
        resourcePath = $"{ResourcePath}/{resourcePath}";
        if (_cache.ContainsKey(resourcePath))
        {
            return (T)_cache[resourcePath];
        }

        string json = "";
        try
        {
            json = File.ReadAllText(resourcePath);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
            Logger.Error(e.StackTrace);
            return null;
        }
        if (string.IsNullOrEmpty(json)) return new T();
        var obj = JsonSerializer.Deserialize<T>(json);
        _cache.Add(resourcePath, obj);

        if (typeof(T) == typeof(Image))
        {
            var image = obj as Image;
            image.Cache();
        }
        return obj;
    }

    public string LoadText(string path)
    {
        var text = File.ReadAllText($"{ResourcePath}/{path}");
        return text;
    }
}