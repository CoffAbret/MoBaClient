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
    /// 是否可以行走
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public bool GetWalkable(Vector2 p)
    {
        return grids.IsWalkableAt((int)p.x, (int)p.y);
    }
}
