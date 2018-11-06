using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;

public enum DebugNetMessageColorType
{
    black, yellow, red, green, white
}
public enum BtnState
{
    NoGetReward, GetReward, AlreadyReward
}

public enum LanguageAtlasType
{
    EnglishAtlas,//英文图集
    BattleTextEnglish,//战斗文字
}
public static class ZhaoChangUtil
{
    #region uiGrid扩展
    /// <summary>
    /// 隐藏GRID下所有子项
    /// </summary>
    /// <param name="grid">UIGrid</param>
    public static void HideItem(this UIGrid grid)
    {
        if (grid == null) return;
        int len = grid.transform.childCount;
        for (int i = 0; i < len; i++)
        {
            if (grid.GetChild(i) == null) continue;
            grid.GetChild(i).gameObject.SetActive(false);
        }

    }


    /// <summary>
    /// 获取GRID下显示的ITEM数量
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    public static int GetActiveChildCount(this UIGrid grid)
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (grid.transform.GetChild(i).gameObject.activeSelf)
            {
                return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 获取GRID下显示的ITEM数量
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    public static int GetActiveChildNumber(this UIGrid grid)
    {
        int num = 0;
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (grid.transform.GetChild(i).gameObject.activeSelf)
            {
                ++num;
            }
        }
        return num;
    }


    /// <summary>
    /// 获取Grid下面所有显示的GameObject
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    public static List<GameObject> GetActiveGameObject(this UIGrid grid)
    {
        List<GameObject> tempObjList = new List<GameObject>();
        if (grid != null)
        {
            for (int i = 0; i < grid.transform.childCount; i++)
            {
                if (grid.transform.GetChild(i).gameObject.activeSelf)
                {
                    tempObjList.Add(grid.transform.GetChild(i).gameObject);
                }
            }
            return tempObjList;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 对GRID下的物体通过按钮状态排序
    /// </summary>
    /// <param name="gird"></param>
    /// <param name="">按钮状态和物体组成的字典</param>
    public static void SortGridChildPosByBtnState(this UIGrid grid, Dictionary<GameObject, BtnState> dic)
    {
        if (dic == null)
        {
            return;
        }
        int index = 0;
        foreach (var item in dic)
        {
            if (BtnState.GetReward == item.Value)
            {
                item.Key.transform.SetSiblingIndex(0);
                index++;
            }
        }
        foreach (var item in dic)
        {
            if (BtnState.NoGetReward == item.Value)
            {
                item.Key.transform.SetSiblingIndex(index);
                index++;
            }
        }
        foreach (var item in dic)
        {
            if (BtnState.AlreadyReward == item.Value)
            {
                item.Key.transform.SetAsLastSibling();
            }
        }

        grid.enabled = true;
    }


    #endregion

    #region gameObject扩展

    /// <summary>
    /// 为游戏物体设置父级
    /// </summary>
    /// <param name="obj">子级物体</param>
    /// <param name="parentTrans">父级Transform</param>
    public static void SetParent(this GameObject obj, GameObject parentTrans)
    {
        obj.transform.SetParent(parentTrans.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = new Quaternion(0, 0, 0, 0);
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
    }
    /// <summary>
    /// 安全的获取组件,如果有取,如果没加
    /// </summary>
    /// <typeparam name="T">组件类型,必须继承自组件</typeparam>
    /// <param name="obj">组件类型</param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (t == null)
        {
            t = obj.AddComponent<T>();
        }
        return t;
    }

    public static void BtnClickTweenScale(this GameObject go)
    {
        if (go == null) return;
        TweenScale ts = go.GetComponent<TweenScale>();
        if (ts == null) ts = go.AddComponent<TweenScale>();
        ts.from = Vector3.one;
        ts.to = Vector3.one * 1.1f;
        ts.style = UITweener.Style.Once;
        ts.duration = 0.1f;
        ts.Play(true);
        EventDelegate.Add(ts.onFinished, () => { ts.Play(false); });
    }
    #endregion

    #region long扩展
    public static uint LongToUint(this long lon)
    {
        uint ui = 0;
        uint.TryParse(lon.ToString(), out ui);
        return ui;
    }

    public static uint IntToUint(this int lon)
    {
        uint ui = 0;
        uint.TryParse(lon.ToString(), out ui);
        return ui;
    }

    static string unitFormat(int i)
    {
        string retStr = null;
        if (i >= 0 && i < 10)
            retStr = "0" + i;
        else
            retStr = "" + i;
        return retStr;
    }
    /// <summary>
    /// Long转Int
    /// </summary>
    /// <param name="lon"></param>
    /// <returns></returns>
    public static int LongToInt(this long lon)
    {
        return lon > int.MaxValue ? int.MaxValue : (int)lon;
    }
    public static uint LongToUInt(this long lon)
    {
        return lon > uint.MaxValue ? uint.MaxValue : (uint)lon < 0 ? 0 : (uint)lon;
    }
    public static long StringToLong(this string str)
    {
        long lon = 0;
        long.TryParse(str, out lon);
        return lon;
    }
    public static int StringToInt(this string str)
    {
        int i = 0;
        int.TryParse(str, out i);
        return i;
    }
    public static uint StringToUInt(this string str)
    {
        uint i = 0;
        uint.TryParse(str, out i);
        return i;
    }
    public static float StringToFloat(this string str)
    {
        float i = 0;
        try
        {
            i = float.Parse(str);
        }
        catch (Exception e)
        {
            GameDebug.LogError(str + "to float error!");
        }
        return i;
    }
    public static double StringToDouble(this string str)
    {
        double i = 0;
        try
        {
            i = double.Parse(str);
        }
        catch (Exception e)
        {
            GameDebug.LogError(str + "to float error!");
        }
        return i;
    }
    #endregion

    #region float扩展

    public static long ToLong(this float f)
    {
        return (long)Mathf.FloorToInt(f);
    }

    #endregion

    #region string扩展
    /// <summary>
    /// 判断文字中是否包含中文字符
    /// </summary>
    /// <param name="str">需判断的文字</param>
    /// <returns>是否包含</returns>
    public static bool IsChinesLanguage(this string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] >= 0x4e00 && str[i] <= 0x9fbb) return true;
        }
        return false;
    }
    #endregion

}