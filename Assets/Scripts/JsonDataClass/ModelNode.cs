using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelNode : FSDataNodeBase
{
    public int id;
   // public string modelName;
    public string respath;
    public string modelRoot;
    public string modelPath;
    public string modelLowPath;
    public float showSize;  // 王冲添加 脚底红圈半径
    public float colliderRadius;
    public float navRadius;
   // public float modelSize = 1.2f;
    public float colliderCenterY = 0.3f;
   // public float modelRotation = 180;
    public float colliderHeight = 0.8f;
    public float modelPosY = 0;
    public int[] modelDataType;
    public override void ParseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = int.Parse(item["id"].ToString());
       // modelName = item["remarks"].ToString();
        respath = item["respath"].ToString();
        modelRoot = respath.Substring(0, respath.LastIndexOf("/") + 1);
        modelPath = respath.Substring(respath.LastIndexOf("/") + 1);
        modelLowPath = modelPath + "_low";
        navRadius = float.Parse(item["navSize"].ToString());
        colliderRadius = float.Parse(item["colliderSize"].ToString());
       // if (item.ContainsKey("size"))
       // {
       //     modelSize = float.Parse(item["size"].ToString());
       // }
        modelDataType = item.TryGetIntArr("ModelDataType");
       showSize = item.TryGetFloat("showSize", 0);
        object[] temObj;
        if (item.ContainsKey("colliderCenter"))
        {
            temObj = (object[])item["colliderCenter"];
            if (temObj!=null&&temObj.Length>=2)
            {
                colliderCenterY = float.Parse(temObj[1].ToString());
            }
        }
       // if (item.ContainsKey("modelrotation"))
       // {
       //     modelRotation = float.Parse(item["modelrotation"].ToString());
       // }
        if (item.ContainsKey("colliderHeight"))
        {
            colliderHeight = float.Parse(item["colliderHeight"].ToString());
        }
        if (item.ContainsKey("modelPosition_dialogue"))
        {
            modelPosY = float.Parse(item["modelPosition_dialogue"].ToString());
        }
    }
}
