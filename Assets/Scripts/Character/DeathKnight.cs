using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathKnight : Entity
{
    public GameObject skillEffect;

    public override void Skill()
    {
        Instantiate(skillEffect, enemy.transform.position, Quaternion.identity).GetComponent<ParticleSystemRenderer>().sortingOrder = enemy.sprRenderer.sortingOrder + 1;
        enemy.GetDamage(this);
    }
}
