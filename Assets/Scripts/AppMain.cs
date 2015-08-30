using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Object = UnityEngine.Object;

public class AppMain : MonoBehaviour {
    public static List<string> ResourcePaths = new List<string>(){"Assets/ExternalResources/"};

    public void Awake()
    {
        Debug.Log("Application awake...");
        Debug.Log("Data path: " + Application.dataPath + ", persistent data path: " + Application.persistentDataPath);
    }

    public void Start()
    {
        Debug.Log("Application start...");
        StreamFromAllResources<GameObject>("Main Camera").Subscribe(x => Instantiate(x),
            ex => Debug.Log(ex.Message));
    }

    private IObservable<T> StreamFromAllResources<T>(string name) where T : Object
    {
        return Observable.Amb<T>(StreamFromExternalResources<T>(name));
    }

    private IObservable<T> StreamFromResources<T>(string name) where T : Object
    {
        return Resources.LoadAsync<T>(name)
            .AsAsyncOperationObservable()
            .Select(x => x.asset as T);
    }

    private IObservable<T> StreamFromExternalResources<T>(string name) where T : Object
    {
        var url = "file://" + Application.dataPath + "/ExternalResources/resources.unity3d";
        Debug.Log("Loading asset from path: " + url);
        return ObservableWWW.LoadFromCacheOrDownload(url, 1).Select(x => x.LoadAsset<T>(name));
    }

    private IObservable<T> LoadFromExternalResources<T>(string prefabName) where T : Object
    {
        return Models.Get<AssetBundleModel>().LoadFromBundleStream<T>(prefabName);
    }
}

public class AssetBundleModel : Model
{
    private readonly Dictionary<string, AssetBundleDto> _assetBundleDtos = new Dictionary<string, AssetBundleDto>();
    private readonly Dictionary<string, GameObject> _loadedResources = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, Object> _loadedDependencies = new Dictionary<string, Object>();

    public IObservable<T> LoadFromBundleStream<T>(string prefabname) where T : Object
    {
        var assetBundleDto = _assetBundleDtos[prefabname];
        if (assetBundleDto == null) return Observable.Empty<T>();

        return Observable.Create<T>(obs =>
        {
            return DependencyStream(prefabname).Subscribe(
                _ => {},
                obs.OnError,
                () => Download(assetBundleDto.Guid, assetBundleDto.Hash).Subscribe(
                    x => x.LoadAssetWithSubAssetsAsync<T>(prefabname)
                        .AsAsyncOperationObservable()
                        .Subscribe(y => obs.OnNext(y.asset as T))
                )
            );
        });
    }

    private IObservable<Object> DependencyStream(string resourceName)
    {
        var dependencies = _assetBundleDtos[resourceName].Dependencies.Select(x =>
        {
            var dependency = _loadedDependencies[x.Hash];
            return dependency != null ? Observable.Return(new[]{dependency}.ToList()) : DownloadDependency(x.Hash, x.Guid);
        }).ToArray();
        return Observable.Create<Object>(obs =>
        {
            return Observable.WhenAll(dependencies).ObserveOnMainThread().Subscribe(
                x => x.SelectMany(y => y.Select(z => z)).ToList().ForEach(obs.OnNext), 
                obs.OnError,
                obs.OnCompleted
            );
        });
    }

    private IObservable<List<Object>> DownloadDependency(string hash, string guid)
    {
        return Observable.Create<List<Object>>(obs =>
        {
            return Download(guid,hash).Subscribe(
                x => x.LoadAllAssetsAsync().AsAsyncOperationObservable().Subscribe(y => obs.OnNext(y.allAssets.ToList()), obs.OnError, obs.OnCompleted),
                obs.OnError
            );
        });
    }

    private IObservable<AssetBundle> Download(string guid, string hash)
    {
        return ObservableWWW.LoadFromCacheOrDownload(guid + "_" + hash + ".bundle", new Hash128());
    } 
}

public class AssetBundleDto
{
    public string Hash { get; set; }
    public string Guid { get; set; }
    public List<AssetBundleDto> Dependencies { get; set; }

}


public class Model
{
}

public static class Models
{
    public static T Get<T>() where T : Model, new ()
    {
        return new T();
    }
}
