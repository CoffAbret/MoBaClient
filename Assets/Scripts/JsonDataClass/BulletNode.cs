using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class BulletNode : FSDataNodeBase
{
    public int id;//子弹id
    public float fly_speed;//飞行速度
    public float fly_max;//飞行最大距离
    public string effect;//子弹特效
    public Vector3 effect_xyz;//子弹特效偏移
    public int aerobatics;//特殊轨迹处理
    public int target_type;//目的地类型 "1.目标坐标 2.追踪目标
    public float time_max;//最大持续时间
    public float effect_timeend;//自然销毁特效
    public int follow_type;//角色跟随处理 "0.无1.子弹发射者跟随该子弹2.子弹命中时子弹发射者瞬移到命中坐标3.子弹命中时技能使用者瞬移到命中坐标"
    public int[] col_type;//碰撞检测类型
    public int[] pen_times_max;//穿透总次数限制
    public int col_times_single;//单目标碰撞最大次数
    public int pen_times_singe;//单目标穿透最大次数
    public int[] col_times_space;//碰撞检测间隔
    public int col_times_max;//碰撞限制最大数量
    public int col_times_oneupdate;//单次心跳碰撞限制最大数量
    public int col_size_type;//碰撞检测类型
    public double[] col_size_value;//碰撞检测参数
    public double[] col_size_change;//碰撞检测高级参数
    public float col_size_changespeed;//碰撞检测高级参数变化速率
    public string effect_hit;//碰撞命中特效名
    public int effect_hit_position;//命中特效挂点
    public Vector3 effect_hit_positionxyz;//命中特效挂点偏移
    public float damage;//命中伤害
    public int dmg_origin;//伤害来源
    public int[] buff;//命中赋予BUFF
    public int[] buff_value;//BUFF参数
    public float buff_odds;//BUFF赋予几率
    public int[] new_unit;//命中生成单位ID
    public int[] unit_num;//生成数量
    public double[] unit_range;//生成随机范围半径
    public List<int[]> newbul = new List<int[]>();//触发子弹 "1.销毁触发2.碰撞触发 格式"
    public List<double[]> newbul_dalay = new List<double[]>();//触发子弹发射延时
    public List<int[]> newbul_origin = new List<int[]>();//触发子弹发射者 0.无1.当前子弹碰撞目标2.技能释放者
    public List<int[]> newbul_target_type = new List<int[]>();//子弹目标类型 "1.当前目标2.范围内目标"
    public List<int[]> newbul_target_value = new List<int[]>();//子弹目标类型参数
    public List<double[]> newbul_target_size = new List<double[]>();//子弹目标范围参数
    public List<int[]> newbul_target_extra = new List<int[]>();//目标额外判定  "0.无额外判定1.要求没有受到过当前技能伤害2.要求没有受到过当前子弹系列伤害（包含上级子弹）类型为当前目标时不读取
    public List<int[]> newbul_max = new List<int[]>();//子弹最大数量
    public List<int[]> newbul_num_single = new List<int[]>();//同目标最小，最大数量
    public List<int[]> newbul_firing = new List<int[]>();//子弹发射挂点
    public List<Vector3> newbul_firing_xyz = new List<Vector3>();//子弹发射挂点偏移
    public List<int[]> newbul_end = new List<int[]>();//子弹目标挂点
    public List<Vector3> newbul_end_xyz = new List<Vector3>();//子弹目标挂点偏移
    public List<double[]> newbul_angle = new List<double[]>();//子弹弹道角度偏移

    public override void ParseJson(object jd)
    {
        Dictionary<string, object> item = (Dictionary<string, object>)jd;
        id = item.TryGetInt("ID");
        fly_speed = item.TryGetFloat("fly_speed");
        fly_max = item.TryGetFloat("fly_max");
        effect = item.TryGetString("effect");
        //if (item.ContainsKey("effect_xyz") && item["effect_xyz"] != null)
        //{
        //    object[] effect_xyz_temp = item["effect_xyz"] as object[];
        //    GetVector3List(effect_xyz, effect_xyz_temp);
        //}
        effect_xyz = item.TryGetToVector3("effect_xyz");
        aerobatics = item.TryGetInt("aerobatics");
        target_type = item.TryGetInt("target_type");
        time_max = item.TryGetFloat("time_max");
        effect_timeend = item.TryGetFloat("effect_timeend");
        follow_type = item.TryGetInt("follow_type");
        col_type = item.TryGetIntArr("col_type");
        pen_times_max = item.TryGetIntArr("pen_times_max");
        col_times_single = item.TryGetInt("col_times_single");
        pen_times_singe = item.TryGetInt("pen_times_singe");
        col_times_space = item.TryGetIntArr("col_times_space");
        col_times_max = item.TryGetInt("col_times_max");
        col_times_oneupdate = item.TryGetInt("col_times_oneupdate");
        col_size_type = item.TryGetInt("col_size_type");
        col_size_value = item.TryGetDoubleArr("col_size_value");
        col_size_change = item.TryGetDoubleArr("col_size_change");
        col_size_changespeed = item.TryGetFloat("col_size_changespeed");
        effect_hit = item.TryGetString("effect_hit");
        effect_hit_position = item.TryGetInt("effect_hit_position");
        effect_hit_positionxyz = item.TryGetToVector3("effect_hit_positionxyz");
        damage = item.TryGetFloat("damage");
        dmg_origin = item.TryGetInt("dmg_origin");
        buff = item.TryGetIntArr("buff");
        buff_value = item.TryGetIntArr("buff_value");
        buff_odds = item.TryGetFloat("buff_odds");
        new_unit = item.TryGetIntArr("new_unit");
        unit_num = item.TryGetIntArr("unit_num");
        unit_range = item.TryGetDoubleArr("unit_range");
        if (item.ContainsKey("newbul") && item["newbul"] != null)
        {
            object[] newbul_temp = item["newbul"] as object[];
            GetIntList(newbul, newbul_temp);
        }
        if (item.ContainsKey("newbul_dalay") && item["newbul_dalay"] != null)
        {
            object[] newbul_dalay_temp = item["newbul_dalay"] as object[];
            GetDoubleList(newbul_dalay, newbul_dalay_temp);
        }
        if (item.ContainsKey("newbul_origin") && item["newbul_origin"] != null)
        {
            object[] newbul_origin_temp = item["newbul_origin"] as object[];
            GetIntList(newbul_origin, newbul_origin_temp);
        }
        if (item.ContainsKey("newbul_target_type") && item["newbul_target_type"] != null)
        {
            object[] newbul_target_type_temp = item["newbul_target_type"] as object[];
            GetIntList(newbul_target_type, newbul_target_type_temp);
        }
        if (item.ContainsKey("newbul_target_value") && item["newbul_target_value"] != null)
        {
            object[] newbul_target_value_temp = item["newbul_target_value"] as object[];
            GetIntList(newbul_target_value, newbul_target_value_temp);
        }
        if (item.ContainsKey("newbul_target_size") && item["newbul_target_size"] != null)
        {
            object[] newbul_target_size_temp = item["newbul_target_size"] as object[];
            GetDoubleList(newbul_target_size, newbul_target_size_temp);
        }
        if (item.ContainsKey("newbul_target_extra") && item["newbul_target_extra"] != null)
        {
            object[] newbul_target_extra_temp = item["newbul_target_extra"] as object[];
            GetIntList(newbul_target_extra, newbul_target_extra_temp);
        }
        if (item.ContainsKey("newbul_max") && item["newbul_max"] != null)
        {
            object[] newbul_max_temp = item["newbul_max"] as object[];
            GetIntList(newbul_max, newbul_max_temp);
        }
        if (item.ContainsKey("newbul_num_single") && item["newbul_num_single"] != null)
        {
            newbul_num_single = new List<int[]>();
            object[] newbul_num_single_temp = item["newbul_num_single"] as object[];
            for (int i = 0; i < newbul_num_single_temp.Length; i++)
            {
                int[] objs = newbul_num_single_temp[i] as int[];
                newbul_num_single.Add(objs);
            }
        }
        if (item.ContainsKey("newbul_firing") && item["newbul_firing"] != null)
        {
            object[] newbul_firing_temp = item["newbul_firing"] as object[];
            GetIntList(newbul_firing, newbul_firing_temp);
        }
        if (item.ContainsKey("newbul_firing_xyz") && item["newbul_firing_xyz"] != null)
        {
            object[] newbul_firing_xyz_temp = item["newbul_firing_xyz"] as object[];
            GetVector3List(newbul_firing_xyz, newbul_firing_xyz_temp);
        }
        if (item.ContainsKey("newbul_end") && item["newbul_end"] != null)
        {
            object[] newbul_end_temp = item["newbul_end"] as object[];
            GetIntList(newbul_end, newbul_end_temp);
        }
        if (item.ContainsKey("newbul_end_xyz") && item["newbul_end_xyz"] != null)
        {
            object[] newbul_end_xyz_temp = item["newbul_end_xyz"] as object[];
            GetVector3List(newbul_end_xyz, newbul_end_xyz_temp);
        }
        if (item.ContainsKey("newbul_angle") && item["newbul_angle"] != null)
        {
            object[] newbul_angle_temp = item["newbul_angle"] as object[];
            GetDoubleList(newbul_angle, newbul_angle_temp);
        }
    }
    public void GetIntList(List<int[]> list_temp, object[] object_temp)
    {
        if (object_temp == null || object_temp.Length == 0)
        {
            //Debug.LogError("     BulletNode    ");
            return;
        }
        for (int i = 0; i < object_temp.Length; i++)
        {
            int[] objs = object_temp[i] as int[];
            list_temp.Add(objs);
        }
    }
    public void GetDoubleList(List<double[]> list_temp, object[] object_temp)
    {
        if (object_temp == null || object_temp.Length == 0)
        {
            //Debug.LogError("     BulletNode    ");
            return;
        }
        for (int i = 0; i < object_temp.Length; i++)
        {
            double[] objs = object_temp[i] as double[];
            list_temp.Add(objs);
        }
    }

    public void GetVector3List(List<Vector3> list_temp, object[] object_temp)
    {
        if (object_temp == null || object_temp.Length == 0)
        {
            //Debug.LogError("     BulletNode    ");
            return;
        }
        for (int i = 0; i < object_temp.Length; i++)
        {
            double[] objs = object_temp[i] as double[];
            if (objs != null && objs.Length == 3)
            {
                Vector3 temp = new Vector3(objs[0].ToString().StringToFloat(), objs[1].ToString().StringToFloat(), objs[2].ToString().StringToFloat());
                list_temp.Add(temp);
            }
        }
    }
}
