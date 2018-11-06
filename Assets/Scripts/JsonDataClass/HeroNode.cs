using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class HeroNode : FSDataNodeBase
{
    public long hero_id;        //英雄ID

    public int types;           //英雄类型 1：英雄，2:小怪，3：普通怪物，4：精英，5：BOSS，6：建筑

    public string name;         //英雄名字

    public int[] mount_types;   //可骑乘类型

    public int[] describe;     //英雄定位

    public int[] skinID;        //皮肤ID

    public int[] fate_hero;     //命运英雄

    public int attribute_type;  //命运属性类型
    public int[] recommend_equip;//推荐装备类型
    public double[] attribute_value; //命运加成数值
    public int rarity; //稀有度
    public string info;         //英雄信息

    public int[] skill_order;   //出手顺序

    public int attribute;       //角色属性 1:力量，2：智力，3：敏捷

    public int released;        //当前版本是否开放 0 : No，1：Yes

    public int sex;             // 性别 1：男，2：女，

    public int init_star;       //初始星级

    public int[] characteristic; //英雄特点
    // 1-5星成长系数：[力量, 智力, 敏捷]
    public float[] rate1;
    public float[] rate2;
    public float[] rate3;
    public float[] rate4;
    public float[] rate5;
    public float[] rate6;
    public int soul_gem;         //灵魂石id
    public long[] skill_id;      //技能id
    public object[] skill1_id;   //多段技能id
    public float[] summontime;  //英雄召唤时间
    public int[] skill_forbidden;    //AI禁用技能

    public int[] passiveAttr;//解锁技能
    public int[] passiveAttr_unlockStar;//星级解锁

    public Int32[] equipment;//能穿戴的装备位 以[0,0,0,0,0]的形式依次配置
    public int[] equipment_add;//以[0,0,0,0,0]的形式依次配置


    public override void ParseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        released = int.Parse(item["released"].ToString());
        if (released == 1)
        {
            hero_id = long.Parse(item["hero_id"].ToString());
            types = int.Parse(item["types"].ToString());
            name = item["name"].ToString();
            rarity = item.TryGetInt("rarity");
            recommend_equip = item.TryGetIntArr("recommend_equip");
            mount_types = item["mount_types"] as int[];
            describe = item["describe"] as int[];
            skinID = item["skin_id"] as int[];
            fate_hero = item.TryGetIntArr("fate_hero");
            attribute_type = int.Parse(item["attribute_type"].ToString());
            if (hero_id != 0 && name != null && name != "")
            {
                GameDebug.Log("---------------当前英雄表的英雄ID为 hero_id  111 == " + hero_id + "      name == " + name, DebugLevel.Exception);
            }
            if (item.ContainsKey("attribute_value") && item["attribute_value"] != null)
            {
                int[] da = item.TryGetIntArr("attribute_value");
                if (da != null && da.Length > 0)
                {
                    attribute_value = new double[da.Length];
                    for (int i = 0; i < da.Length; i++)
                    {
                        attribute_value[i] = da[i];
                    }
                }
                if (attribute_value == null)
                {
                    double[] dt = item.TryGetDoubleArr("attribute_value");
                    if (dt != null && dt.Length > 0)
                    {
                        attribute_value = new double[dt.Length];
                        for (int i = 0; i < dt.Length; i++)
                        {
                            attribute_value[i] = dt[i];
                        }
                    }
                }
                if (attribute_value == null)
                {
                    Debug.LogError("attribute_value查看表");
                }

            }
            else
            {

            }
            info = item.TryGetString("info");
            skill_order = item["skill_order"] as int[];
            attribute = int.Parse(item["attribute"].ToString());
            sex = int.Parse(item["sex"].ToString());
            init_star = int.Parse(item["init_star"].ToString());
            if (item.ContainsKey("characteristic"))
                characteristic = item["characteristic"] as int[];
            rate1 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate1"]);
            rate2 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate2"]);
            rate3 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate3"]);
            rate4 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate4"]);
            rate5 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate5"]);
            rate6 = FSDataNodeTable<HeroNode>.GetSingleton().ParseToFloatArray(item["rate6"]);
            soul_gem = int.Parse(item["soul_gem"].ToString());
            int[] nodeIntarr = (int[])item["skill_id"];
            skill_id = new long[nodeIntarr.Length];
            for (int m = 0; m < nodeIntarr.Length; m++)
            {
                skill_id[m] = nodeIntarr[m];
            }
            skill1_id = item["skill1_id"] as object[];

            if (item["summontime"] is int[])
            {
                int[] _summontime = item["summontime"] as int[];
                summontime = new float[_summontime.Length];
                for (int i = 0; i < _summontime.Length; i++)
                {
                    summontime[i] = float.Parse(_summontime[i].ToString());
                }
            }
            else if (item["summontime"] is double[])
            {
                double[] _summontime = item["summontime"] as double[];
                summontime = new float[_summontime.Length];
                for (int i = 0; i < _summontime.Length; i++)
                {
                    summontime[i] = float.Parse(_summontime[i].ToString());
                }
            }
            else
            {
                object[] _summontime = item["summontime"] as object[];
                summontime = new float[_summontime.Length];
                for (int i = 0; i < _summontime.Length; i++)
                {
                    summontime[i] = float.Parse(_summontime[i].ToString());
                }
            }

            if (item["skill_forbidden"] is int[])
                skill_forbidden = item["skill_forbidden"] as int[];
            if (item.ContainsKey("equipment"))
            {
                equipment = item["equipment"] as Int32[];
            }
            if (item.ContainsKey("equipment_add"))
            {

                equipment_add = item["equipment_add"] as int[];
            }
            passiveAttr = item.TryGetIntArr("passiveAttr");
            passiveAttr_unlockStar = item.TryGetIntArr("passiveAttr_unlockStar");
            //if (item.ContainsKey("equipment_add") && item["equipment_add"] != null)
            //{
            //    int[][] skilldata = item["equipment_add"] as int[][];
            //    passiveAttr = new int[skilldata.Length];
            //    passiveAttr_unlockStar = new int[skilldata.Length];
            //    for (int i = 0; i < skilldata.Length; i++)
            //    {
            //        passiveAttr[i] = skilldata[i][0];
            //        passiveAttr_unlockStar[i] = skilldata[i][1];
            //    }
            //}
        }

    }
    /// <summary>
    /// 获取星级成长系数
    /// </summary>
    /// <param name="attrIndex">属性索引【力量，智力，敏捷】</param>
    /// <param name="star">星级</param>
    /// <returns></returns>
    public float GetStarGrowUpRate(int attrIndex, int star)
    {
        switch (star)
        {
            case 1:
                return (float)rate1[attrIndex];
            case 2:
                return (float)rate2[attrIndex];
            case 3:
                return (float)rate3[attrIndex];
            case 4:
                return (float)rate4[attrIndex];
            case 5:
                return (float)rate5[attrIndex];
            case 6:
                return (float)rate6[attrIndex];
            default:
                return 0f;
        }
    }

    /// <summary>
    /// 获取英雄星级成长属性
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public float[] GetStarGrowUpData(int Index)
    {
        float[] Attri = new float[5];
        switch (Index)
        {
            case 1:
                Attri = rate1;
                break;
            case 2:
                Attri = rate2;
                break;
            case 3:
                Attri = rate3;
                break;
            case 4:
                Attri = rate4;
                break;
            case 5:
                Attri = rate5;
                break;
            case 6:
                Attri = rate6;
                break;
            default:
                Attri = null;
                break;
        }
        return Attri;
    }
}

