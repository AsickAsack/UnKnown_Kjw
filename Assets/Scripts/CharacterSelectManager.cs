using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{

    private int limitCount = 3;

    public SelectBtn[] sBtns;

    [SerializeField]
    private GameObject rejectPopup;

    //확인할거고
    public List<List<Entity>> all = new List<List<Entity>>();
    public List<Entity> first = new List<Entity>();
    public List<Entity> second = new List<Entity>();
    public List<Entity> third = new List<Entity>();

    private void Awake()
    {
        all.Add(first);
        all.Add(second);
        all.Add(third);

        for (int i=0;i<Player.instance.CurStageMonsters.Count;i++)
        {
            Entity newEntity = Instantiate<Entity>(Player.instance.CurStageMonsters[i].monster);
            newEntity.sprRenderer.flipX = true;
            newEntity.transform.position = GameUtill.GetPos(Player.instance.CurStageMonsters[i].coordinate.x, Player.instance.CurStageMonsters[i].coordinate.y,true);

        }

        for(int i=0;i < Player.instance.MyEntities.Count;i++)
        {
            sBtns[i].Init(Player.instance.MyEntities[i], this);
        }
        
    }

    public void SelectRoutine(int startIndex,Entity entity)
    {
        int startY = 2 - all[startIndex].Count;

        if(startY == -1)
        {
            if(startIndex == 2)
            {
                startIndex -= 1;
            }
            else
            {
                startIndex += 1;
            }

            SelectRoutine(startIndex, entity);
        }
        else
        {
            all[startIndex].Add(Instantiate<Entity>(entity));

            for (int i = 0; i < all[startIndex].Count; i++)
            {
                Entity newEntity = all[startIndex][i];

                newEntity.startCoordinate = new Vector2Int(startIndex, startY);
                newEntity.transform.position = GameUtill.GetPos(startIndex, startY, false);
                startY += 2;
            }

            limitCount--;
        }

    }

    //씬이동
    public void MoveScene()
    {

        if(limitCount.Equals(0))
        {
            for(int i =0;i<Player.instance.MyEntities.Count;i++)
            {
                for(int j = 0; j < all.Count;j++)
                {
                    Entity tempEntity = all[j].Find(x => x.name.Split("(")[0] == Player.instance.MyEntities[i].name);

                    if(tempEntity != null)
                    {
                        Player.instance.MyEntities[i].startCoordinate = tempEntity.startCoordinate;
                    }
                    
                }
            }

            GameUtill.MoveScene("InGameScene");
        }
        else
        {
            rejectPopup.SetActive(true);
           
        }
        
    }




}
