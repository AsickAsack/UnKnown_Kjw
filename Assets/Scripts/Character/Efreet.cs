using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Efreet : Entity
{

    [Header("[����,��ų ������Ʈ]")]

    [SerializeField]
    private Transform[] atkPos; //0-����, 1-������

    [SerializeField]
    private AttackObj attackObj;

    [SerializeField]
    private GameObject skillObj;

    [SerializeField]
    private float skillObjSpeed;

    [SerializeField]
    private GameObject skillEffect;

    private GameObject skillObjectInstance;

    public void AttackObj()
    {
        Instantiate<AttackObj>(attackObj, atkPos[this.sprRenderer.flipX ? 0 : 1].position, Quaternion.identity).SetAttackObj(this);
    }


    public override void Skill()
    {
        skillObjectInstance = Instantiate(skillObj, this.transform.position + Vector3.up, Quaternion.identity);
    }

    //Ÿ�̹� ���� ��ų ������Ʈ ������
    public void ThrowSkillObj()
    {
        StartCoroutine(GameUtill.MoveCoroutineSpeed(skillObjectInstance.transform, enemy.transform.position, skillObjSpeed,
            callBack: () =>
            {
                Instantiate(skillEffect, skillObjectInstance.transform.position, Quaternion.identity);
                Destroy(skillObjectInstance);

                enemy.GetDamage(this);

            }));
    }


}
