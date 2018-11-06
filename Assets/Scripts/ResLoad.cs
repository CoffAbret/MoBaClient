using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
public class ResLoad : MonoBehaviour
{
    public static ResLoad instance;
    //本地资源下载地址
    public static string LOCAL_RES_URL =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
"file:///" + GetSavePath();
#elif UNITY_ANDROID
 "file:///"+ GetSavePath();
#elif UNITY_IPHONE
 "file:///"+ GetSavePath();
#endif
    //本地资源加载地址
    public static string LOCAL_RES_PATH =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
 GetSavePath();
#elif UNITY_ANDROID
    GetSavePath();
#elif UNITY_IPHONE
   GetSavePath() +  "/Raw/";
#endif

    public static int LOADRESYPE = 1;//加载资源方式，1本地资源；2读取资源包的资源
    //读取json资源
    static AssetBundle JsonBundle;
    static string[] BundleLength = null;
    static List<string> tmpname = new List<string>();
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public static string GetSavePath()
    {
        string strTablePath = string.Empty;
        // persistentData AB & File 路径
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        strTablePath = Application.persistentDataPath + "/";
#elif UNITY_IOS
        strTablePath = Application.persistentDataPath + "/";
#elif UNITY_ANDROID
        strTablePath = Application.persistentDataPath + "/";
#endif

        return strTablePath;
    }
    public static string LoadJsonRes(string name)
    {
        if (LOADRESYPE == 1)
        {
            TextAsset ob = (TextAsset)Resources.Load("JsonData/" + name);
            return ob.ToString();
        }

        else
        {
            if (LOCAL_RES_PATH == null)
            {

            }
            string tmpName = name.ToLower();

            if (JsonBundle == null)
            {
                JsonBundle = AssetBundle.LoadFromFile(ResLoad.LOCAL_RES_PATH + "/JsonData/" + tmpName + ".unity3d");
            }
            TextAsset ob = (TextAsset)JsonBundle.LoadAsset(tmpName);
            UnloadJsonBundle();
            return ob.text;
        }
    }

    public static void UnloadJsonBundle()
    {
        if (JsonBundle != null)
            JsonBundle.Unload(false);
    }
    static AssetBundle UIBundle;
    static Transform mainUITra;
    public static void SetUnLoadUIAssetBundelRes()
    {
        if (UIBundle)
            UIBundle.Unload(false);
    }
}

