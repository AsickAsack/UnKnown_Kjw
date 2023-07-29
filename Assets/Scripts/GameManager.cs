using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    [Header("[���� ����]")]
    public float gameTime;
    public bool isAutoSkill = false;
    public bool isSpeedUp = false;
    public bool isStart = false;

    [Header("[UI�Ŵ���]")]
    [SerializeField]
    private UIManager uiManager;

    //�����
    private List<Entity> allCharacters = new List<Entity>();

    //�� ĳ����
    private List<Entity> myChars = new List<Entity>();
    public List<Entity> MyChars => myChars;

    //�� ��
    private List<Entity> enemies = new List<Entity>();
    public List<Entity> Enmeies => enemies;

    //��ų ����
    private Queue<(Entity entity,UnityAction skill)> skillQueue = new Queue<(Entity entity, UnityAction skill)>();
    private Coroutine skillCoroutine = null;


    private void Awake()
    {
        //�ʱ� ����

        for (int i = 0; i < Player.instance.MyEntities.Count; i++)
        {
            myChars.Add(Instantiate(Player.instance.MyEntities[i]));
            //myChars[i].startCoordinate = Instantiate(Player.instance.MyEntities[i]
            myChars[i].ConnectUI(uiManager.DamageGraphes[i], uiManager.SkillBtns[i]);
        }

        for (int i = 0; i < Player.instance.CurStageMonsters.Count; i++)
        {
            enemies.Add(Instantiate<Entity>(Player.instance.CurStageMonsters[i].monster));
            enemies[^1].startCoordinate = Player.instance.CurStageMonsters[i].coordinate;
            enemies[^1].isEnemy = true;
        }

        allCharacters.AddRange(myChars);
        allCharacters.AddRange(enemies);

        foreach (Entity entity in allCharacters)
        {
            entity.Init(this, entity.isEnemy);
        }

        uiManager.Init();
    }

    private void Update()
    {
        if (isStart)
        {
            gameTime -= Time.deltaTime;
            uiManager.SetGameTime();

            if (gameTime < 0)
            {
                GameResult(false);
            }
        }
    }

    #region ��ų ����

    public void SetSkillQueue((Entity entity,UnityAction skill) action)
    {
        skillQueue.Enqueue(action);

        if (skillCoroutine == null)
        {
            skillCoroutine = StartCoroutine(SkillCoroutine());
        }

    }


    IEnumerator SkillCoroutine()
    {
        while (!skillQueue.Count.Equals(0))
        {
            (Entity entity, UnityAction skill) curQueue = skillQueue.Dequeue();

            curQueue.skill();
            uiManager.SetSkillUI(true, curQueue.entity);
            Time.timeScale = 0;

            float waitTime = isSpeedUp ? (float)(0.4 / 1.5f) : 0.4f;
            yield return new WaitForSecondsRealtime(waitTime);

            Time.timeScale = isSpeedUp ? 1.5f : 1f;
            uiManager.SetSkillUI(false, curQueue.entity);


            yield return null;
        }

        skillCoroutine = null;
    }

    #endregion

    //���� ��� �����ֱ�
    public void GameResult(bool isClear)
    {
        foreach (var entity in allCharacters)
        {
            entity.ChangeState(State.None);
        }

        uiManager.SetResultUI(isClear);
    }


    //���� ����� �� Ž��
    public Entity FindClose(Entity entity, List<Entity> list)
    {
        Entity close = null;
        float distance = 9999999;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                if (list[i].CompareState(State.Death)) continue;
                if (Vector2.SqrMagnitude(list[i].transform.position - entity.transform.position) < distance)
                {
                    close = list[i];
                    distance = Vector2.SqrMagnitude(close.transform.position - entity.transform.position);
                }
            }
        }

        try
        {
            if (close != null)
            {
                entity.sprRenderer.flipX = entity.transform.position.x > close.transform.position.x;
            }
        }
        catch
        {
            Debug.Log("����");
        }


        return close;

    }


    //Y�� �� => SortingOrder ����
    public void SetSortingOrder()
    {
        allCharacters.Sort((one, two) => two.transform.position.y.CompareTo(one.transform.position.y));

        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (!allCharacters[i].CompareState(State.Death))
            {
                allCharacters[i].sprRenderer.sortingOrder = i + 1;
            }
        }
    }

    #region UI���

    //��� �����̴� ����
    public void SetTopSlider()
    {
        uiManager.SetTopSliders();
    }


    //��� hp Get�Լ�
    public float GetAllHp(bool isMyChars)
    {
        List<Entity> getEntity = isMyChars ? myChars : enemies;

        float allHp = 0;

        foreach (Entity character in getEntity)
        {
            allHp += character.Stat.curHP;
        }

        return allHp;
    }

    //���� ���ǵ� �� ��ư ���
    public void GameSpeedUp()
    {
        isSpeedUp = !isSpeedUp;

        if (isSpeedUp)
        {
            Time.timeScale = 1.5f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

    }

    //���� ��ų ��ư ���
    public void AutoSkillBtn()
    {
        this.isAutoSkill = !this.isAutoSkill;
    }

    //������ �ؽ�Ʈ ����
    public void SetDamageText(float value, Vector2 pos, TextType type)
    {
        uiManager.SetDamageText(value, pos, type);
    }

    #endregion
}
