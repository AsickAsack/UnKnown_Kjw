using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostGiant : Entity
{
    [SerializeField]
    private GameObject effect;

    public override void Skill()
    {
        Instantiate(effect, enemy.transform.position+Vector3.up*0.5f, Quaternion.identity).GetComponent<ParticleSystemRenderer>().sortingOrder = enemy.sprRenderer.sortingOrder + 1;
        if(enemy != null)
        {
            enemy.GetDamage(this);
        }
        
    }
}
