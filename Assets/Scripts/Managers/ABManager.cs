using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Networking;

public class ABManager : SingleTons<ABManager>
{
    private class ABInfo
    {
        public string name;
        public long size;
        public string MD5;
        public ABInfo(string name,string sizeStr,string MD5)
        {
            this.name = name;
            this.size = long.Parse(sizeStr);
            this.MD5 = MD5;
        }
        public override bool Equals(object obj)
        {
            return obj is ABInfo other &&
            MD5 == other.MD5;
        }
        public override int GetHashCode()
        {
            return MD5.GetHashCode();
        }
    }
    private AssetBundle MainAB = null;
    private AssetBundleManifest manifest = null;
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
    private Dictionary<string, ABInfo> RemoteABInfoDic = new Dictionary<string, ABInfo>();
    private Dictionary<string, ABInfo> LocalABInfoDic = new Dictionary<string, ABInfo>();
    private List<string> DownLoadList = new List<string>();

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
    public void CheckUpdate(UnityAction<bool> callback = null)
    {
        RemoteABInfoDic.Clear();
        LocalABInfoDic.Clear();
        DownLoadList.Clear();
        DowmLoadABCompareFile((IsSuccess) =>
        {
            if(IsSuccess)
            {
                GetLocalABCompareFile(() =>
                {
                    foreach(string abname in RemoteABInfoDic.Keys)
                    {
                        if(!LocalABInfoDic.ContainsKey(abname))
                        {
                            DownLoadList.Add(abname);
                        }
                        else
                        {
                            if(RemoteABInfoDic[abname].MD5 != LocalABInfoDic[abname].MD5)
                            {
                                DownLoadList.Add(abname);
                            }
                            LocalABInfoDic.Remove(abname);
                        }
                    }
                    foreach(string abname in LocalABInfoDic.Keys)
                    {
                        if(File.Exists(Application.persistentDataPath + "/" + abname))
                        {
                            File.Delete(Application.persistentDataPath + "/" + abname);
                        }
                    }
                    DownLoadABFile((IsSuccess) =>
                    {
                        if(IsSuccess)
                        {
                            File.WriteAllText(Application.persistentDataPath + "/ABCompareInfo.txt",File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt"));
                        }
                        Debug.Log("更新完成");
                        callback?.Invoke(IsSuccess);
                    });
                });
            }
            else
            {
                callback?.Invoke(IsSuccess);
            }
        });
    }
    public async void DowmLoadABCompareFile(UnityAction<bool> callback = null)
    {
        int MaxDownLoadCount = 5;
        bool isSuccess = false;
        string path = Application.persistentDataPath + "/";
        while(!isSuccess && MaxDownLoadCount > 0)
        {
            await Task.Run(() =>
            {
                isSuccess = DownLoadFile("ABCompareInfo.txt",path + "ABCompareInfo_TMP.txt");
            });
            MaxDownLoadCount--;
        }
        string compareInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
        string[] compareInfos = compareInfo.Split('\n');
        string[] Datainfos = null;
        for(int i = 0;i<compareInfos.Length - 1;i++)
        {
            Datainfos = compareInfos[i].Split(' ');
            RemoteABInfoDic.Add(Datainfos[0],new ABInfo(Datainfos[0],Datainfos[1],Datainfos[2]));
        }
        callback?.Invoke(isSuccess);
    }
    public void GetLocalABCompareFile(UnityAction callback = null)
    {
        string datapath = Application.persistentDataPath + "/ABCompareInfo.txt";
        string streamingpath = Application.streamingAssetsPath + "/ABCompareInfo.txt";
        if(File.Exists(datapath))
        {
            StartCoroutine(GetLocalABCompareFileInfo(datapath,callback));
        }
        else if(File.Exists(streamingpath))
        {
            StartCoroutine(GetLocalABCompareFileInfo(streamingpath,callback));
        }
        else
        {
            callback?.Invoke();
        }
    }
    private IEnumerator GetLocalABCompareFileInfo(string path,UnityAction callback = null)
    {
        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();
        string compareInfo = req.downloadHandler.text;
        string[] compareInfos = compareInfo.Split('\n');
        string[] Datainfos = null;
        for(int i = 0;i<compareInfos.Length - 1;i++)
        {
            Datainfos = compareInfos[i].Split(' ');
            LocalABInfoDic.Add(Datainfos[0],new ABInfo(Datainfos[0],Datainfos[1],Datainfos[2]));
        }
        callback?.Invoke();
    }
    public async void DownLoadABFile(UnityAction<bool> callback = null)
    {
        string path = Application.persistentDataPath + "/";
        List<string> SuccessList = new List<string>();
        int MaxDownLoadCount = 5;
        while(DownLoadList.Count > 0 && MaxDownLoadCount > 0)
        {
            for(int i = 0;i < DownLoadList.Count;i++)
            {
                bool isSuccess = false;
                await Task.Run(() =>
                {
                    isSuccess = DownLoadFile(DownLoadList[i],path + DownLoadList[i]);
                });
                if(isSuccess)
                {
                    SuccessList.Add(DownLoadList[i]);
                }
            }
            for(int i = 0;i < SuccessList.Count;i++)
            {
                DownLoadList.Remove(SuccessList[i]);
            }
            MaxDownLoadCount--;
        }
        callback?.Invoke(DownLoadList.Count == 0);
    }
    private bool DownLoadFile(string fileName,string localpath)
    {
        try
        {
            FtpWebRequest req = FtpWebRequest.Create(new System.Uri("ftp://120.26.166.41/PC/" + fileName)) as FtpWebRequest;
            NetworkCredential credentials = new NetworkCredential("XFuture", "wyh.is.xf.91");
            req.Credentials = credentials;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Proxy = null;
            req.KeepAlive = false;
            req.UseBinary = true;
            FtpWebResponse resp = req.GetResponse() as FtpWebResponse;
            Stream respStream = resp.GetResponseStream();
            using(FileStream fileStream = File.Create(localpath))
            {
                byte[] buffer = new byte[2048];
                int contentLength = respStream.Read(buffer, 0, buffer.Length);
                while(contentLength > 0)
                {
                    fileStream.Write(buffer, 0, contentLength);
                    contentLength = respStream.Read(buffer, 0, buffer.Length);
                }
                respStream.Close();
                fileStream.Close();
            }
            return true;
        }
        catch(WebException e)
        {
            Debug.LogError("下载文件：" + fileName + "失败，错误信息：" + e.Message);
            return false;
        }
    }
}