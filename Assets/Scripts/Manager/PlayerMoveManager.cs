using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动控制
/// </summary>
public class PlayerMoveManager
{
    private Fix64 m_PreAngle = Fix64.Zero;
    private Vector3 preV3;
    public void UpdateMove()
    {
        if (GameData.m_CurrentPlayer == null)
            return;
        Fix64 fixX = (Fix64)ETCInput.GetAxis("Horizontal");
        Fix64 fixY = (Fix64)ETCInput.GetAxis("Vertical");
        if (fixX == Fix64.Zero && fixY == Fix64.Zero)
        {
            if (GameData.m_CurrentPlayer.m_IsMove)
            {
                GameData.m_GameManager.InputCmd(Cmd.MoveEnd);
                m_PreAngle = Fix64.Zero;
            }
        }
        else
        {
            if (GameData.m_CurrentPlayer.m_IsSkill || GameData.m_CurrentPlayer.m_IsAttack || GameData.m_CurrentPlayer.m_IsHit || GameData.m_CurrentPlayer == null || GameData.m_CurrentPlayer.m_PlayerData == null)
            {
                m_PreAngle = Fix64.Zero;
                return;
            }
            Fix64 angle = CalcAngle(fixX, fixY);
            if (angle < Fix64.One)
                return;
            Fix64 subAngle = angle - m_PreAngle;
            subAngle = subAngle < Fix64.Zero ? -subAngle : subAngle;
            if (subAngle < Fix64.One)
                return;
            string parameter = string.Format("{0}#{1}", fixX, fixY);
            GameData.m_GameManager.InputCmd(Cmd.Move, parameter);
            m_PreAngle = angle;
        }
    }

    private Fix64 CalcAngle(Fix64 x, Fix64 y)
    {
        x = x < Fix64.Zero ? -x : x;
        y = y < Fix64.Zero ? -y : y;
        Fix64 angleX = x * Fix64.FromRaw(180000);
        Fix64 angleY = y * Fix64.FromRaw(180000);
        return angleX + angleY;
    }
}
