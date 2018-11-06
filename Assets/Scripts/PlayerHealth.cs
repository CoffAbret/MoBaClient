using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int m_Health=100;
    public void SetDamage(int damage)
    {
        m_Health -= damage;
    }
}
