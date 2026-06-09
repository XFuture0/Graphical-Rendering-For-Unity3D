using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CreateABCompare : EditorWindow
{
    private static string path = Application.streamingAssetsPath;
    [MenuItem("Tools/CreateABCompareFile")]
    public static void CreateABCompareFile()
    {
        DirectoryInfo dir = Directory.CreateDirectory(path);
        FileInfo[] filesInfos = dir.GetFiles();
        StringBuilder abCompareInfo = new StringBuilder();
        foreach(FileInfo file in filesInfos)
        {
            if(file.Extension == "")
            {
                abCompareInfo.AppendLine(file.Name + " " + file.Length + " " + MathTools.GetMD5Code(file.FullName));
            }
        }
        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo.ToString());
        AssetDatabase.Refresh();
        Debug.Log("AB包对比文件创建成功，文件路径：" + Application.streamingAssetsPath + "/ABCompareInfo.txt");
    }
}
