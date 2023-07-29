using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwashBuckler : Entity
{
    [SerializeField]
    private Bomb bomb;
    
    public override void Skill()
    {
        Instantiate<Bomb>(bomb,this.transform.position,Quaternion.identity).SetBomb(this);
    }
}
