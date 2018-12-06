using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIThebattlePanel : MonoBehaviour
{
    public GameObject m_ExitGame;
    public GameObject m_ShengLiGo;
    public GameObject m_ShiBaiGo;
    private void OnEnable()
    {
        UIEventListener.Get(m_ExitGame).onClick = OnExitGameClick;
        if (GameData.m_GameResult)
        {
            m_ShengLiGo.SetActive(true);
            m_ShiBaiGo.SetActive(false);
        }
        else
        {
            m_ShengLiGo.SetActive(false);
            m_ShiBaiGo.SetActive(true);
        }
    }

    private void OnExitGameClick(GameObject go)
    {
        Application.Quit();
    }
}
