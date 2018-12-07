using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动控制
/// </summary>
public class PlayerMoveManager
{
    private Vector3 m_PrePos = Vector3.zero;
    private float m_PreAngle = 0f;
    public void UpdateMove()
    {
        if (GameData.m_CurrentPlayer == null)
            return;
        Vector3 newEulerAngles = Vector3.zero;
        float x = ETCInput.GetAxis("Horizontal");
        float y = ETCInput.GetAxis("Vertical");
        if (x == 0 && y == 0)
        {
            if (GameData.m_CurrentPlayer.m_IsMove)
            {
                GameData.m_GameManager.InputCmd(Cmd.MoveEnd);
                m_PrePos = Vector3.zero;
                m_PreAngle = 0f;
            }
        }
        else
        {
            if (GameData.m_CurrentPlayer.m_IsSkill || GameData.m_CurrentPlayer.m_IsAttack || GameData.m_CurrentPlayer.m_IsHit || GameData.m_CurrentPlayer == null || GameData.m_CurrentPlayer.m_PlayerData == null)
                return;
            //if (Math.Abs(x) < 0.1 && Math.Abs(y) < 0.1)
            //    return;
            //Vector3 pos = new Vector3(x, 0, y);
            //float angle = Mathf.Acos(Vector3.Dot(pos.normalized, (pos - m_PrePos).normalized)) * Mathf.Rad2Deg;
            //if ((m_PrePos == Vector3.zero || Mathf.Abs(angle - m_PreAngle) > 0.1f))
            //{
            string parameter = string.Format("{0}#{1}#{2}#{3}#{4}#{5}", x, 0, y, GameData.m_CurrentPlayer.m_Pos.x, GameData.m_CurrentPlayer.m_Pos.y, GameData.m_CurrentPlayer.m_Pos.z);
            GameData.m_GameManager.InputCmd(Cmd.Move, parameter);
            //m_PrePos = pos;
            //m_PreAngle = angle;
            //}
        }
    }
}
