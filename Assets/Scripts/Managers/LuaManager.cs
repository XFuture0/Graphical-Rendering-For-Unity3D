using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;

public class LuaManager : SingleTons<LuaManager>
{
   private LuaEnv luaEnv;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    public void Init()
    {
        if(luaEnv != null)
        {
            return;
        }
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(RefreshLoadPath);
        luaEnv.AddLoader(RefreshABLoadPath);
    }
    public LuaTable Global
    {
        get
        {
            return luaEnv.Global;
        }
    }
    public void DoLuaFile(string fileName)
    {
        string str = string.Format("require('{0}')", fileName);
        if(luaEnv == null)
        {
            Debug.Log("Lua环境未初始化");
            return;
        }
        luaEnv.DoString(str);
    }
    public byte[] RefreshLoadPath(ref string FileName)
    {
        string path = Application.dataPath + "/Lua/" + FileName + ".lua";
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.Log("Lua文件不存在:" + path);
        }
        return null;
    }
    private byte[] RefreshABLoadPath(ref string FileName)
    {
        TextAsset Luatext = ABManager.Instance.LoadRes<TextAsset>("lua",FileName + ".lua");
        if (Luatext != null)
        {
            return Luatext.bytes;
        }
        else
        {
            Debug.Log("AB文件不存在:" + FileName);
            return null;
        }
    }
    public void Tick()
    {
        if (luaEnv == null)
        {
            Debug.Log("Lua环境未初始化");
            return;
        }
        luaEnv.Tick();
    }
    public void Dispose()
    {
        if (luaEnv == null)
        {
            Debug.Log("Lua环境未初始化");
            return;
        }
        luaEnv.Dispose();
        luaEnv = null;
    }
}
