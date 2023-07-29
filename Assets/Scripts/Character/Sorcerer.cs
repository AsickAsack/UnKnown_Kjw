using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorcerer : Entity
{
    [SerializeField]
    private Transform[] atkPos; //0-¿ÞÂÊ, 1-¿À¸¥ÂÊ

    [SerializeField]
    private AttackObj attackObj;

    [SerializeField]
    private GameObject healEffect;

    public override void Skill()
    {
        foreach(Entity team in (isEnemy ? manager.Enmeies : manager.MyChars))
        {
            if(team.CompareState(State.Death) || team == null) continue;

            Instantiate(healEffect, team.transform.position, Quaternion.identity).GetComponent<ParticleSystemRenderer>().sortingOrder = team.sprRenderer.sortingOrder+1;
            float healValue = team.Stat.curHP * 0.1f;
            team.Stat.curHP += healValue;
            manager.SetDamageText(healValue, team.transform.position, TextType.Heal);
            team.SetHpBar();
        }
        
    }

    public void AttackObj()
    {
        Instantiate<AttackObj>(attackObj, atkPos[this.sprRenderer.flipX ? 0 : 1].position, Quaternion.identity).SetAttackObj(this);
    }
}
