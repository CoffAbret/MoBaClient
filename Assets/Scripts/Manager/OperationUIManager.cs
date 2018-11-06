using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationUIManager : MonoBehaviour
{
    public GameObject m_AttackGo;
    public GameObject m_Skill1Go;
    public GameObject m_Skill2Go;
    public GameObject m_Skill3Go;
    public GameObject m_Skill4Go;
    private int m_Index = 0;
    private string m_Parameter;
    public string m_Text = string.Empty;
    private void Start()
    {
        UIEventListener.Get(m_AttackGo).onClick = OnAttackClick;
        UIEventListener.Get(m_Skill1Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill2Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill3Go).onClick = OnSkillClick;
        UIEventListener.Get(m_Skill4Go).onClick = OnSkillClick;
    }

    private void OnAttackClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsSkill)
            return;
        if (GameData.m_CurrentPlayer.m_IsDie)
            return;
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        GameData.m_CurrentPlayer.m_IsAttack = true;
        GameData.m_GameManager.InputCmd(Cmd.Attack);
    }

    public void OnSkillClick(GameObject go)
    {
        if (GameData.m_CurrentPlayer.m_IsSkill)
            return;
        if (GameData.m_CurrentPlayer.m_IsAttack)
            return;
        if (GameData.m_CurrentPlayer.m_IsHit)
            return;
        if (!int.TryParse(go.name.Substring(go.name.Length - 1, 1), out m_Index))
            return;
        m_Parameter = string.Format("{0}", m_Index);
        GameData.m_CurrentPlayer.m_IsSkill = true;
        GameData.m_GameManager.InputCmd(Cmd.UseSkill, m_Parameter);
    }
}
