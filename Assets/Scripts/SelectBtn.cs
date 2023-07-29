using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBtn : MonoBehaviour
{

    public int index;

    [SerializeField]
    private Image portrait;
    [SerializeField]
    private GameObject selected;
    private Entity entity;
    private CharacterSelectManager manager;

    public void Init(Entity entity,CharacterSelectManager charSelectManager)
    {
        this.entity = entity;
        portrait.sprite = entity.portrait;
        this.manager = charSelectManager;
        this.GetComponent<Button>().onClick.AddListener(ClickBtn);
    }


    public void ClickBtn()
    {
        if(!selected.gameObject.activeInHierarchy)
        {
            switch (entity.charType)
            {
                case CharType.Warrior:

                    manager.SelectRoutine(0, this.entity);
                    break;
                case CharType.Assassin:
                    manager.SelectRoutine(1, this.entity);
                    break;
                case CharType.Wizard:
                    manager.SelectRoutine(2, this.entity);
                    break;
            }

            selected.SetActive(true);
        }
        else
        {
            return;
        }
           
    }



}
