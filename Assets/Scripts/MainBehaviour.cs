using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏入口
/// </summary>
public class MainBehaviour : MonoBehaviour
{
    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        //修改每帧时间长度
        Time.timeScale = 1f;
        Application.runInBackground = true;
        //固定50帧
        Application.targetFrameRate = 50;
    }
    /// <summary>
    /// 开始准备游戏数据以及网络连接
    /// </summary>
    private void Start()
    {
        GameData.m_GameManager = new GameManager();
        GameData.m_GameManager.InitGame();
        GameData.m_GameManager.InputReady();
    }

    /// <summary>
    /// 每帧更新游戏逻辑
    /// </summary>
    private void FixedUpdate()
    {
        if (GameData.m_GameManager == null)
            return;
        GameData.m_GameManager.UpdateGame();
    }

    /// <summary>
    /// 销毁游戏数据
    /// </summary>
    private void OnDestroy()
    {
        if (GameData.m_GameManager == null)
            return;
        GameData.m_GameManager.DestoryGame();
    }
}
