using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏对象生成管理类
/// </summary>
public class SpawnManager
{
    private Fix64 m_IntervalTime = Fix64.Zero;
    private Fix64 m_SpawnTime = Fix64.FromRaw(20000);

    /// <summary>
    /// 每帧执行逻辑
    /// </summary>
    public void UpdateLogic()
    {
        m_IntervalTime += GameData.m_FixFrameLen;
        if (m_IntervalTime >= m_SpawnTime)
        {
            for (int i = 0; i < 2; i++)
            {
                int roleId = 0;
                string roleName = "";
                int heroId = i % 2 == 0 ? 202100200 : 202100500;
                int campId = i % 2 == 0 ? 1 : 2;
                PlayerData charData = new PlayerData(roleId, heroId, roleName, campId, 2);
                GameData.m_GameManager.CreatePlayer(charData);
            }

            for (int i = 0; i < 2; i++)
            {
                int roleId = 0;
                string roleName = "";
                int heroId = i % 2 == 0 ? 202100100 : 202100400;
                int campId = i % 2 == 0 ? 1 : 2;
                PlayerData charData = new PlayerData(roleId, heroId, roleName, campId, 3);
                GameData.m_GameManager.CreatePlayer(charData);
            }
            m_IntervalTime = Fix64.Zero;
        }
    }
}
