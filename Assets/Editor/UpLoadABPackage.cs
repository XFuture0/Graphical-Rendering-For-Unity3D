using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using System.Threading.Tasks;

public class UpLoadABPackage : EditorWindow
{
    private static string path = Application.streamingAssetsPath;
    [MenuItem("Tools/UpLoadABPackageFile")]
    public static void UpLoadABPackageFile()
    {
        DirectoryInfo dir = Directory.CreateDirectory(path);
        FileInfo[] filesInfos = dir.GetFiles();
        foreach(FileInfo file in filesInfos)
        {
            if(file.Extension == "" || file.Extension == ".txt")
            {
                UpLoadFile(file.FullName, file.Name);
            }
        }
    }
    private async static void UpLoadFile(string filePath,string fileName)
    {
        await Task.Run(() =>
        {
            try
            {
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://127.0.0.1/PC/" + fileName)) as FtpWebRequest;
                NetworkCredential credentials = new NetworkCredential("username", "password");
                req.Credentials = credentials;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Proxy = null;
                req.KeepAlive = false;
                req.UseBinary = true;
                Stream upLoadStream = req.GetRequestStream();
                using(FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    byte[] buffer = new byte[2048];
                    int contentLength = fileStream.Read(buffer, 0, buffer.Length);
                    while(contentLength > 0)
                    {
                        upLoadStream.Write(buffer, 0, contentLength);
                        contentLength = fileStream.Read(buffer, 0, buffer.Length);
                    }
                    upLoadStream.Close();
                    fileStream.Close();
                }
            }
            catch(WebException e)
            {
                Debug.LogError("上传文件：" + fileName + "失败，错误信息：" + e.Message);
            }
        });
    }
}
