using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageMonster
{

    public StageMonster(StageMonster monster)
    {
        this.monster = monster.monster;
        this.coordinate = monster.coordinate;
    }

    public Vector2Int coordinate;
    public Entity monster;
}

public class Stage : MonoBehaviour
{

    public string stageName;

    [SerializeField]
    private List<StageMonster> stageMonsters = new List<StageMonster>();
    public List<StageMonster> StageMonsters =>stageMonsters;

    [SerializeField]
    private StageUI stageUI;

    private void OnMouseDown()
    {
        stageUI.SetStageUI(this);
    }
}
