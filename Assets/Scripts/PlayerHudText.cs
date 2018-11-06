using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHudText : MonoBehaviour
{
    public GameObject m_Prefab;
    public Transform m_Target;
    private HUDText m_Text = null;
    private UIFollowTarget m_UIFollowTarget = null;
    void Start()
    {
        if (HUDRoot.go == null)
        {
            GameObject.Destroy(this);
            return;
        }

        GameObject child = NGUITools.AddChild(HUDRoot.go, m_Prefab);
        m_Text = child.GetComponentInChildren<HUDText>();
        m_UIFollowTarget = child.AddComponent<UIFollowTarget>();
        m_UIFollowTarget.target = m_Target;
        m_UIFollowTarget.gameCamera = Camera.main;
        m_UIFollowTarget.uiCamera = GameObject.Find("Camera").GetComponent<Camera>();
        child.SetActive(false);
    }

    public HUDText PlayerHUDText
    {
        get { return m_Text; }
    }

}
