using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastMaster : Entity
{

    [SerializeField]
    private Fox fox;

    public override void Skill()
    {   
        Instantiate<Fox>(fox,this.transform.position,Quaternion.identity).SetFox(this,manager);
    }
}
