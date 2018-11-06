using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroSkinNode : FSDataNodeBase
{
    /// <summary>
    /// 皮肤id
    /// </summary>
    public long skinId;
    /// <summary>
    /// 皮肤卡（物品）id
    /// </summary>
    public long skin_cardId;
    /// <summary>
    /// 英雄id
    /// </summary>
    public long heroId;
    /// <summary>
    /// 皮肤名称
    /// </summary>
    public string skinName;
    /// <summary>
    /// 图集名uicard/uicard1
    /// </summary>
    public string iconAtlas;
    /// <summary>
    /// 头像图集名
    /// </summary>
    public string head_icon_atlas;
    /// <summary>
    /// 皮肤原画
    /// </summary>
    public string skinCard;
    /// <summary>
    /// 英雄模型
    /// </summary>
    public int modelId;
    /// <summary>
    /// 音效路径
    /// </summary>
    public string path;
    /// <summary>
    /// 死亡音效
    /// </summary>
    public string deathSound;
    /// <summary>
    /// 受击音效
    /// </summary>
    public string hitSound;
    /// <summary>
    /// 台词数量
    /// </summary>
    public int dlgAmount;
    /// <summary>
    /// 头像名称
    /// </summary>
    public string iconName;
    /// <summary>
    /// 当前版本是否开放 0不开放 1开放
    /// </summary>
    public int released;
    /// <summary>
    /// 类型 1：力量，2：智力，3：敏捷
    /// </summary>
    public int type;
    /// <summary>
    /// 增加的属性值
    /// </summary>
    public float property;
    /// <summary>
    /// 价格[钻石,电卷]
    /// </summary>
    public int[] price;

    /// <summary>
    /// 商品ID
    /// </summary>
    public int commodity_id;
    public ModelNode modelNode;
    public string grade;
    public string effectName;
    public override void ParseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        released = item.TryGetInt("released");
        if (released==1)
        {
            skinId = item.TryGetLong("skin_id");

            grade = item.TryGetString("grade");
            commodity_id= item.TryGetInt("commodity_id");
            skin_cardId = item.TryGetLong("skin_card");

            heroId = item.TryGetLong("hero_id");
            skinName = item.TryGetString("name");
            iconAtlas = item.TryGetString("icon_atlas");
            head_icon_atlas = item.TryGetString("head_icon_atlas");
            skinCard = item.TryGetString("original_painting");
            modelId = item.TryGetInt("model");
            modelNode = FSDataNodeTable<ModelNode>.GetSingleton().FindDataByType(modelId);
            path = item.TryGetString("path");
            deathSound = item.TryGetString("death_sound");

            if (item.ContainsKey("hit_sound"))
                hitSound = item["hit_sound"].ToString();
            if (item.ContainsKey("dlgAmount"))
                dlgAmount = item.TryGetInt("dlgAmount");
            if (item.ContainsKey("icon_name"))
                iconName = item["icon_name"].ToString();
            if (item.ContainsKey("type"))
                type = item.TryGetInt("type");
            if (item.ContainsKey("property"))
                property = item.TryGetFloat("property");
            if (item.ContainsKey("price"))
                price = item["price"] as int[];
            if (item.ContainsKey("effectName"))
                effectName = item["effectName"] == null?null: item["effectName"].ToString();
        }

    }
}
