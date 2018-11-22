using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动控制
/// </summary>
public class PlayerMoveController : MonoBehaviour
{
    private void LateUpdate()
    {
        Vector3 newEulerAngles = Vector3.zero;
        float x = ETCInput.GetAxis("Horizontal");
        float y = ETCInput.GetAxis("Vertical");
        if (x == 0 && y == 0)
        {
            if (GameData.m_CurrentPlayer.m_IsMove)
            {
                GameData.m_GameManager.InputCmd(Cmd.MoveEnd);
            }
        }
        else
        {
            if (GameData.m_CurrentPlayer.m_IsSkill || GameData.m_CurrentPlayer.m_IsAttack || GameData.m_CurrentPlayer.m_IsHit)
                return;
            string parameter = string.Format("{0}#{1}#{2}", x, 0, y);
            GameData.m_GameManager.InputCmd(Cmd.Move, parameter);
        }
    }
}
