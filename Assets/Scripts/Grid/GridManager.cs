using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EpPathFinding.cs;
using SimpleJSON;

public class GridManager
{
    private BaseGrid grids;
    private int width, height;
    private Vector3 center;
    private float nodeSize;
    private List<float> mapHeight;
    private const int iterNum = 1;
    private const float TEPSILON = 0.01f;
    private const float EPSILON = 0.01f;
    public void InitGrid()
    {
        var mapSource = Resources.Load<TextAsset>("JsonData/MapSourceConfig");
        JSONNode data = JSON.Parse(mapSource.text);
        width = System.Convert.ToInt32(data[0]["width"].AsFloat);
        height = Convert.ToInt32(data[0]["height"].AsFloat);
        width = System.Convert.ToInt32(data[0]["width"].AsFloat);
        height = Convert.ToInt32(data[0]["height"].AsFloat);
        var cenArray = data[0]["center"].AsObject;
        var cx = cenArray["x"].AsFloat;
        var cy = cenArray["y"].AsFloat;
        var cz = cenArray["z"].AsFloat;
        center = new Vector3(cx, cy, cz);
        nodeSize = data[0]["nodeSize"].AsFloat;

        var mapData = data[0]["mapdata"].AsArray;
        grids = new StaticGrid(width, height);
        var i = 0;
        foreach (SimpleJSON.JSONNode d in mapData)
        {
            var v = d.AsInt;
            if (v == 0)
            {
                var r = i / width;
                var c = i % width;
                grids.SetWalkableAt(c, r, true);
            }
            i++;
        }

        var mh = data[0]["mapHeight"].AsArray;
        mapHeight = new List<float>(width * height);
        foreach (JSONNode d in mh)
        {
            mapHeight.Add(d.AsFloat);
        }
    }

    /// <summary>
    /// Unity坐标转化为Grid 网格坐标
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2 MapPosToGrid(Vector3 pos)
    {
        var off = pos - center;
        var left = off.x + nodeSize * width / 2;
        var bottom = off.z + nodeSize * height / 2;
        var gx = (int)(left / nodeSize);
        var gy = (int)(bottom / nodeSize);
        gx = Mathf.Clamp(gx, 0, width - 1);
        gy = Mathf.Clamp(gy, 0, height - 1);
        return new Vector2(gx, gy);
    }

    /// <summary>
    /// 网格坐标带小数部分
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Vector2 MapPosToGridFloat(Vector3 pos)
    {
        var off = pos - center;
        var left = off.x + nodeSize * width / 2;
        var bottom = off.z + nodeSize * width / 2;
        var gx = (left / nodeSize);
        var gy = (bottom / nodeSize);
        gx = Mathf.Clamp(gx, 0, width - 1);
        gy = Mathf.Clamp(gy, 0, height - 1);
        return new Vector2(gx, gy);
    }

    /// <summary>
    /// 查找附近网格
    /// 返回网格的中心点位置
    /// </summary>
    /// <param name="gPos"></param>
    /// <returns></returns>
    private List<Vector2> BroadColGrids(Vector2 gPos)
    {
        var igx = (int)gPos.x;
        var igy = (int)gPos.y;
        var walk = grids.IsWalkableAt(igx, igy);
        var neibors = new List<Vector2>();
        if (!walk)
        {
            neibors.Add(new Vector2(igx + 0.5f, igy + 0.5f));
        }

        for (var i = igx - 1; i <= (igx + 1); i++)
        {
            for (var j = igy - 1; j <= (igy + 1); j++)
            {
                walk = grids.IsWalkableAt(i, j);
                if (!walk)
                {
                    neibors.Add(new Vector2(i + 0.5f, j + 0.5f));
                }
            }
        }
        return neibors;
    }

    private bool FindFirstColGrid(Vector2 gPos, List<Vector2> allGrids, out Vector2 firstPos)
    {
        //网格空间坐标
        var radius = 1 / 2.0f; //玩家半径
        var gridRadius = 1 / 2.0f; //网格球半径
        var dist = (radius + gridRadius);
        dist *= dist;

        //radius *= radius;

        foreach (var n in allGrids)
        {
            var dx = gPos.x - n.x;
            var dy = gPos.y - n.y;
            var newV2 = new Vector2(dx, dy);
            if (newV2.sqrMagnitude < dist)
            {
                firstPos = n;
                return true;
            }
        }
        firstPos = Vector2.zero;
        return false;
    }

    private Vector2 FixPos(Vector2 gPos, Vector2 firstGrid)
    {
        var radius = 1 / 2.0f;
        var gridRadius = 1 / 2.0f;

        var dx = gPos.x - firstGrid.x;
        var dy = gPos.y - firstGrid.y;
        var dirV = new Vector2(dx, dy);
        var offPos = dirV.normalized * (radius + gridRadius + EPSILON);
        return firstGrid + offPos;
    }

    /// <summary>
    /// 浮点网格坐标转化为实际Unity坐标
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    private Vector3 GridToMapPosFloat(Vector2 grid)
    {
        var gx = Mathf.Clamp(grid.x, 0, width - 1);
        var gy = Mathf.Clamp(grid.y, 0, height - 1);

        //TODO:高度可能需要插值
        var igx = (int)gx;
        var igy = (int)gy;
        var gid = (int)(igx + igy * width);
        var h = mapHeight[gid];


        var px = gx * nodeSize - nodeSize * width / 2;
        var py = gy * nodeSize - nodeSize * height / 2;
        var pos = new Vector3(px, 0, py) + center;
        pos.y = h;
        return pos;

    }

    /// <summary>
    /// 得到点Pos 最近的可以走的位置 
    /// 所在网格可以行走
    /// 不能行走则临近最近网格接触点
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 FindNearestWalkableGridPos(Vector3 pos)
    {
        //假设玩家是一个nodeSize 半径的球
        //和周围的网格进行碰撞计算
        //得到实际的位置
        //1:位置得到Circle
        //2:第一遍碰撞 修正位置
        //3:迭代第二次碰撞 修正位置 迭代多次
        var gPos = MapPosToGridFloat(pos);
        var allGrids = BroadColGrids(gPos);

        var count = 0;
        while (count < iterNum)
        {
            Vector2 firstGrid;
            //只迭代1次
            var col = FindFirstColGrid(gPos, allGrids, out firstGrid);
            if (col)
            {
                var newGPos = FixPos(gPos, firstGrid);
                gPos = newGPos;
            }
            else
            {
                break;
            }
        }

        var mp2 = GridToMapPosFloat(gPos);
        return mp2;
    }

    /// <summary>
    /// 是否可以行走
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool GetWalkable(Vector2 p)
    {
        return grids.IsWalkableAt((int)p.x, (int)p.y);
    }

    public void SetWalkable(Tower tower)
    {
        Vector2 v2 = MapPosToGrid(tower.m_Pos.ToVector3());
        int distince = 6;
        if (tower.m_Type == 1)
            distince = 3;
        int xMin = (int)v2.x - distince;
        int xMax = (int)v2.x + distince;
        int yMin = (int)v2.y - distince;
        int yMax = (int)v2.y + distince;
        for (int i = xMin; i <= xMax; i++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                grids.SetWalkableAt(i, y, true);
            }
        }
    }

    public void InitTowerGrid()
    {
        for (int i = 0; i < GameData.m_TowerList.Count; i++)
        {
            Vector2 v2 = MapPosToGrid(GameData.m_TowerList[i].m_Pos.ToVector3());
            int distince = 6;
            if (GameData.m_TowerList[i].m_Type == 1)
                distince = 3;
            int xMin = (int)v2.x - distince;
            int xMax = (int)v2.x + distince;
            int yMin = (int)v2.y - distince;
            int yMax = (int)v2.y + distince;
            for (int k = xMin; k <= xMax; k++)
            {
                for (int u = yMin; u < yMax; u++)
                {
                    grids.SetWalkableAt(k, u, false);
                }
            }
        }
    }
}
