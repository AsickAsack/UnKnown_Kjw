using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    [SerializeField]
    private List<Entity> myEntities = new List<Entity>();
    public List<Entity> MyEntities => myEntities;

    public List<StageMonster> curStageMonsters = new List<StageMonster>();
    public List<StageMonster> CurStageMonsters => curStageMonsters;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //�������� ���� ����
    public void SetStageMonster(List<StageMonster> curStageMonsters)
    {
        this.curStageMonsters.Clear();
       
        foreach (var curMonster in curStageMonsters)
        {
            this.curStageMonsters.Add(new StageMonster(curMonster));
        }

    }



}
