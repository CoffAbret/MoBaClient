using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 延时执行
/// </summary>
public class Delay
{
    public string m_DelayId = string.Empty;
    //时长
    Fix64 m_FixPlanTime = Fix64.Zero;
    //累计时间
    Fix64 m_FixElapseTime = Fix64.Zero;
    //匹配倒计时
    public delegate void MatchTimeCallback(int time);
    MatchTimeCallback m_MatchTimeCallback;
    //英雄复活延时
    public delegate void ResurgenceCallback(PlayerData data);
    //英雄复活延时
    ResurgenceCallback m_ResurgenceCallback = null;
    //复活英雄数据
    PlayerData m_ResurgencePlayerData = null;
    //销毁物体
    GameObject m_DestoryGo = null;
    //CD图片
    UISprite m_SkillCDUISprite = null;
    //加血点
    GameObject m_AddHPGo = null;
    //是否开启
    public bool m_Enable;
    public Delay()
    {
        m_DelayId = Guid.NewGuid().ToString();
    }
    public void UpdateLogic()
    {
        m_FixElapseTime = m_FixElapseTime + GameData.m_FixFrameLen;
        if (m_ResurgencePlayerData != null)
        {
            if (m_ResurgencePlayerData.m_Id == GameData.m_CurrentRoleId)
                GameData.m_GameManager.m_UIManager.m_ResurrectionLabel.text = string.Format("{0}", (int)(m_FixPlanTime - m_FixElapseTime) + 1);
            else
                GameData.m_GameManager.m_UIManager.m_EnemyResurrectionLabel.text = string.Format("{0}", (int)(m_FixPlanTime - m_FixElapseTime) + 1);
        }

        if (m_MatchTimeCallback != null)
        {
            m_MatchTimeCallback((int)m_FixElapseTime);
        }

        if (m_SkillCDUISprite != null)
        {
            m_SkillCDUISprite.fillAmount = 1 - ((float)m_FixElapseTime / (float)m_FixPlanTime);
        }

        if (m_FixElapseTime >= m_FixPlanTime)
        {
            if (m_DestoryGo != null)
                GameObject.DestroyImmediate(m_DestoryGo);

            if (m_AddHPGo != null)
                m_AddHPGo.SetActive(true);

            if (m_ResurgenceCallback != null && m_ResurgencePlayerData != null)
                m_ResurgenceCallback(m_ResurgencePlayerData);

            m_Enable = false;
        }
    }

    /// <summary>
    /// 初始化匹配倒计时
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void InitMatchTime(Fix64 time, MatchTimeCallback callback)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_MatchTimeCallback = callback;
    }

    /// <summary>
    /// 初始化角色复活
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void InitResurgence(PlayerData playerData, Fix64 time, ResurgenceCallback callback)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_ResurgencePlayerData = playerData;
        m_ResurgenceCallback = callback;
    }

    /// <summary>
    /// 初始化加血点
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void InitAddHP(GameObject addHPGo, Fix64 time)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_AddHPGo = addHPGo;
    }

    /// <summary>
    /// 初始化延时
    /// </summary>
    /// <param name="time"></param>
    /// <param name="acb"></param>
    public void InitDestory(GameObject go, Fix64 time)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_DestoryGo = go;
    }

    /// <summary>
    /// 初始化CD
    /// </summary>
    /// <param name="uiSprite"></param>
    /// <param name="time"></param>
    public void InitSkillCD(UISprite uiSprite, Fix64 time)
    {
        m_Enable = true;
        m_FixElapseTime = Fix64.Zero;
        m_FixPlanTime = time;
        m_SkillCDUISprite = uiSprite;
        m_SkillCDUISprite.fillAmount = 1;
    }

    public void Destory()
    {
        if (m_DestoryGo != null)
            GameObject.DestroyImmediate(m_DestoryGo);
        m_DestoryGo = null;
        m_FixPlanTime = Fix64.Zero;
        m_FixElapseTime = Fix64.Zero;
        m_ResurgenceCallback = null;
        m_ResurgencePlayerData = null;
        m_MatchTimeCallback = null;
        m_SkillCDUISprite = null;
        m_Enable = false;
    }
}