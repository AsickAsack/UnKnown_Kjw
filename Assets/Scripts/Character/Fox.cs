using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fox : Entity
{

    private Entity parent;

    public void SetFox(Entity parent,GameManager manager)
    {
        this.parent = parent;
        this.enemy = parent.enemy;
        this.manager = manager;
        this.sprRenderer.sortingOrder = parent.sprRenderer.sortingOrder;
        MoveAssassination();
        StartCoroutine(FoxCoroutine());
    }


    IEnumerator FoxCoroutine()
    {

        while (this.myState != State.Death)
        {
            if(enemy == null || enemy.CompareState(State.Death))
            {
                Destroy(this.gameObject);
            }

            this.stat.curHP -= Time.deltaTime;
            SetHpBar();

            if (this.stat.curHP < 0 || this.parent.CompareState(State.Death))
            {
                ChangeState(State.Death);
                yield return new WaitForSeconds(0.1f);
                Destroy(this.gameObject);
            }

            yield return null;
        }

    }

    protected override void ChangeMoveState()
    {
        curAttackTime = 0;
        curSkillCoolTime = 0;
        animator.SetBool("IsWalk", true);
    }

    protected override void ChangeFightState()
    {
        animator.SetBool("IsWalk", false);
    }

    public override void Skill()
    {
        if(enemy!=null)
        {
            enemy.GetDamage(this.parent);
        }
    }
}
