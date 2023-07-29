using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float bombTime;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject explosion;

    //ÆøÅº ¼¼ÆÃ
    public void SetBomb(Entity callEntity)
    {
        StartCoroutine(GameUtill.MoveCoroutine(this.transform,callEntity.enemy.transform.position,1f,callBack: WaitExplosion(callEntity)));
    }

    //ÆøÅº ÅÍÁö´Â ½Ã°£±îÁö ´ë±â
    IEnumerator WaitExplosion(Entity callEntity)
    {
        while (bombTime > 0.0f)
        {
            bombTime -= Time.deltaTime;

            yield return null;
        }

        Instantiate(explosion, this.transform.position, Quaternion.identity);

        if(callEntity.enemy != null)
        {
            callEntity.enemy.GetDamage(callEntity);
        }
        

        Destroy(this.gameObject);

    }

  
}
