using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABManager : SingleTons<ABManager>
{
    private AssetBundle MainAB = null; // 主AB包
    private AssetBundleManifest manifest = null; // AB包清单
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>(); // 字典存储AB包

    private string PathUrl // AB包路径
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    private string MainABName // 主AB包名称（不同平台）
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    private void LoadABDepend(string abname)
    {
        if (MainAB == null) // 加载主AB包
        {
            MainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        string[] strs = manifest.GetAllDependencies(abname);
        foreach (string str in strs) // 加载当前AB包的依赖
        {
            if (!abDic.ContainsKey(str))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(PathUrl + str);
                abDic.Add(str, bundle);
            }
        }

        if (!abDic.ContainsKey(abname)) // 加载当前AB包
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abname);
            abDic.Add(abname, ab);
        }
    }

    public Object LoadRes(string abname, string resname) // 同步加载资源
    {
        LoadABDepend(abname);
        Object obj = abDic[abname].LoadAsset(resname);
        return obj;
    }

    public Object LoadRes(string abname, string resname, System.Type type) // 同步加载资源（泛型）
    {
        LoadABDepend(abname);
        Object obj = abDic[abname].LoadAsset(resname, type);
        if (type == typeof(GameObject))
        {
            return Instantiate(obj);
        }
        return obj;
    }

    public T LoadRes<T>(string abname, string resname) where T : Object // 同步加载资源
    {
        LoadABDepend(abname);
        T obj = abDic[abname].LoadAsset<T>(resname); // 泛型加载
        return obj;
    }

    public void LoadResAsync(string abname, string resname, UnityAction<Object> callback) // 异步加载资源
    {
        StartCoroutine(ReallyLoadResAsync(abname, resname, callback));
    }

    private IEnumerator ReallyLoadResAsync(string abname, string resname, UnityAction<Object> callback)
    {
        LoadABDepend(abname);
        AssetBundleRequest abr = abDic[abname].LoadAssetAsync(resname);
        yield return abr;
        callback(abr.asset); // 回调
    }

    public void LoadResAsync(string abname, string resname, UnityAction<Object> callback, System.Type type) // 异步加载资源
    {
        StartCoroutine(ReallyLoadResAsync(abname, resname, callback, type));
    }

    private IEnumerator ReallyLoadResAsync(string abname, string resname, UnityAction<Object> callback, System.Type type)
    {
        LoadABDepend(abname);
        AssetBundleRequest abr = abDic[abname].LoadAssetAsync(resname, type);
        yield return abr;
        callback(abr.asset); // 回调
    }

    public void LoadResAsync<T>(string abname, string resname, UnityAction<T> callback, System.Type type) where T : Object // 异步加载资源
    {
        StartCoroutine(ReallyLoadResAsync<T>(abname, resname, callback, type));
    }

    private IEnumerator ReallyLoadResAsync<T>(string abname, string resname, UnityAction<T> callback, System.Type type) where T : Object // 异步加载资源
    {
        LoadABDepend(abname);
        AssetBundleRequest abr = abDic[abname].LoadAssetAsync<T>(resname);
        yield return abr;
        callback(abr.asset as T); // 回调
    }

    public void UnLoad(string abname) // 卸载资源
    {
        if (abDic.ContainsKey(abname))
        {
            abDic[abname].Unload(false);
            abDic.Remove(abname);
        }
    }

    public void ClearAB() // 清除所有资源
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        MainAB = null;
        manifest = null;
    }
}