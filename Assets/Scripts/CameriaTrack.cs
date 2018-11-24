using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机跟随
/// </summary>
public class CameriaTrack : MonoBehaviour
{
    //默认位置
    private Vector3 distincePos = Vector3.zero;

    void Update()
    {
        if (GameData.m_IsDragMinMap)
            return;
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null)
            return;
        if (distincePos == Vector3.zero)
            distincePos = playerGo.transform.position - this.transform.position;
        this.transform.position = playerGo.transform.position - distincePos;
    }
}
