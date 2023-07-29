using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObj : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private GameObject effect;

    [SerializeField]
    private SpriteRenderer sprRenderer;


    //오브젝트 세팅
    public void SetAttackObj(Entity callEntity)
    {
        sprRenderer.flipX = callEntity.sprRenderer.flipX;

        StartCoroutine(GameUtill.MoveCoroutineSpeed(this.transform, callEntity.enemy.transform.position, speed,
            callBack: () =>
            {
                Instantiate(effect, this.transform.position, Quaternion.identity);

                if (callEntity.enemy != null)
                {
                    callEntity.enemy.GetDamage(callEntity);
                }

                Destroy(this.gameObject);
            }));
    }


}

