using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABManager : SingleTons<ABManager>
{
    private AssetBundle MainAB = null;
    private AssetBundleManifest manifest = null;
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    private string MainABName
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
        if (MainAB == null)
        {
            MainAB = AssetBundle.LoadFromFile(PathUrl + MainABName);
            manifest = MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        string[] strs = manifest.GetAllDependencies(abname);
        foreach (string str in strs)
        {
            if (!abDic.ContainsKey(str))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(PathUrl + str);
                abDic.Add(str, bundle);
            }
        }

        if (!abDic.ContainsKey(abname))
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abname);
            abDic.Add(abname, ab);
        }
    }

    public Object LoadRes(string abname, string resname)
    {
        LoadABDepend(abname);
        Object obj = abDic[abname].LoadAsset(resname);
        return obj;
    }

    public Object LoadRes(string abname, string resname, System.Type type)
    {
        LoadABDepend(abname);
        Object obj = abDic[abname].LoadAsset(resname, type);
        if (type == typeof(GameObject))
        {
            return Instantiate(obj);
        }
        return obj;
    }

    public T LoadRes<T>(string abname, string resname) where T : Object
    {
        LoadABDepend(abname);
        T obj = abDic[abname].LoadAsset<T>(resname);
        return obj;
    }

    public void LoadResAsync(string abname, string resname, UnityAction<Object> callback)
    {
        StartCoroutine(ReallyLoadResAsync(abname, resname, callback));
    }

    private IEnumerator ReallyLoadResAsync(string abname, string resname, UnityAction<Object> callback)
    {
        LoadABDepend(abname);
        AssetBundleRequest abr = abDic[abname].LoadAssetAsync(resname);
        yield return abr;
        callback(abr.asset);
    }

    public void LoadResAsync(string abname, string resname, UnityAction<Object> callback, System.Type type)
    {
        StartCoroutine(ReallyLoadResAsync(abname, resname, callback, type));
    }

    private IEnumerator ReallyLoadResAsync(string abname, string resname, UnityAction<Object> callback, System.Type type)
    {
        LoadABDepend(abname);
        AssetBundleRequest abr = abDic[abname].LoadAssetAsync(resname, type);
        yield return abr;
        callback(abr.asset); 
    }

    public void LoadResAsync<T>(string abname, string resname, UnityAction<T> callback, System.Type type) where T : Object 
    {
        StartCoroutine(ReallyLoadResAsync<T>(abname, resname, callback, type));
    }

    private IEnumerator ReallyLoadResAsync<T>(string abname, string resname, UnityAction<T> callback, System.Type type) where T : Object
    {
        LoadABDepend(abname);
        AssetBundleRequest abr = abDic[abname].LoadAssetAsync<T>(resname);
        yield return abr;
        callback(abr.asset as T); 
    }

    public void UnLoad(string abname)
    {
        if (abDic.ContainsKey(abname))
        {
            abDic[abname].Unload(false);
            abDic.Remove(abname);
        }
    }

    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        MainAB = null;
        manifest = null;
    }
}