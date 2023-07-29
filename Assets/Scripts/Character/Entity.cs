using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//����
public enum State
{
    None, Move, Fight, Death
}

//ĳ���� Ÿ��
public enum CharType
{
    Warrior, Assassin, Wizard
}


public class Entity : FSM
{

    [Header("[����]")]
    [SerializeField]
    protected Stat stat;
    public Stat Stat => stat;
    public CharType charType;

    [Header("[����]")]
    public bool isEnemy = false;

    [Header("[������Ʈ]")]
    public Image hpBar;
    public Animator animator;
    public SpriteRenderer sprRenderer;

    [Header("[�ʻ�ȭ]")]
    public Sprite portrait;

    public int killCount;
    public Entity enemy = null;

    public Vector2Int startCoordinate;

    protected GameManager manager;
    protected DamageGraph damageGraph = null;
    protected SkillBtn skillBtn = null;

    private Vector2 sizeLimit = new Vector2(1, 1.2f);
    private Vector2 positionYLimit = new Vector2(0, -3.2f);


    private WaitForSeconds attackWaitTime = new WaitForSeconds(0.5f);


    #region �ʱ� ����

    //�ʱ�ȭ 
    public void Init(GameManager manager, bool isEnemy = false)
    {
        this.manager = manager;
        this.isEnemy = isEnemy;

        if (isEnemy) Reverse();

        Vector2 startPos = GameUtill.GetPos(this.startCoordinate.x, this.startCoordinate.y, this.isEnemy);
        startPos.x += isEnemy ? 2 : -2;

        this.transform.position = startPos;
        SetSizeByPosY();

        Vector2 targetPos = GameUtill.GetPos(this.startCoordinate.x, this.startCoordinate.y, this.isEnemy);
        StartCoroutine(GameUtill.MoveCoroutine(this.transform, targetPos, 1f, () => this.animator.SetBool("IsWalk", true), InitCallBackCo()));

    }

    //�ʱ�ȭ �ݹ� �ڷ�ƾ
    IEnumerator InitCallBackCo()
    {
        this.animator.SetBool("IsWalk", false);
        enemy = manager.FindClose(this, manager.Enmeies.Contains(this) ? manager.MyChars : manager.Enmeies);

        yield return new WaitForSeconds(1f);

        if (!isEnemy)
        {
            this.skillBtn.StartCheckCoolTime(Stat.skillCoolTime);
        }

        if (charType == CharType.Assassin)
        {
            enemy = manager.MyChars[^1];
            MoveAssassination();
        }
        else
        {
            ChangeState(State.Move);
        }

        manager.isStart = true;
    }

    #endregion

    //�ϻ� ������
    public void MoveAssassination()
    {
        StartCoroutine(GameUtill.MoveCoroutine(this.transform, enemy.transform.position + (isEnemy ? Vector3.left : Vector3.right), 0.2f,
            callBack: () =>
            {
                Reverse();
                ChangeState(State.Move);
            }));
    }

    #region ���� ����


    protected override void ChangeMoveState()
    {
        curAttackTime = stat.atkSpeed;


        enemy = manager.FindClose(this, manager.Enmeies.Contains(this) ? manager.MyChars : manager.Enmeies);

        if (enemy == null)
        {
            manager.GameResult(!isEnemy);
        }

        animator.SetBool("IsWalk", true);
    }

    protected override void ChangeFightState()
    {
        manager.SetSortingOrder();
        animator.SetBool("IsWalk", false);
    }

    protected override void ChangeDeathState()
    {
        if (skillBtn != null)
            skillBtn.DeathRoutine();

        hpBar.transform.parent.gameObject.SetActive(false);
        sprRenderer.sortingOrder = 1;

        animator.SetTrigger("Death");
    }

    #endregion

    #region ���º� ��ƾ
    protected override void MoveStateRoutine()
    {
        if (enemy != null)
        {
            if (Vector2.Distance(this.transform.position, enemy.transform.position) < stat.atkRange)
            {
                //����
                ChangeState(State.Fight);
            }
            else
            {
                this.transform.position += (enemy.transform.position - this.transform.position).normalized * Time.deltaTime * stat.moveSpeed;
                SetSizeByPosY();
            }
        }
    }

    protected override void FightStateRoutine()
    {

        if (Vector2.Distance(this.transform.position, enemy.transform.position) > stat.atkRange || enemy.CompareState(State.Death))
        {
            ChangeState(State.Move);
        }
        else
        {
            CheckAttack();
            CheckSkillCoolTime();
        }

    }

    #endregion

    #region �Ϲ� ����

    public bool isAttack = false;

    protected virtual void Attack()
    {
        if (enemy != null)
        {
            enemy.GetDamage(this);
        }

    }

    protected float curAttackTime = 0;
    public void CheckAttack()
    {
        curAttackTime += Time.deltaTime;

        if (curAttackTime > stat.atkSpeed)
        {
            if (!isAttack && !isSkill)
            {
                isAttack = true;
                curAttackTime = 0;
                animator.SetTrigger("Attack");
            }

        }
    }

    //�ִϸ��̼� ������ �ݹ�
    public void AttackCallBack()
    {
        isAttack = false;
    }

    #endregion

    #region ��ų ����

    public bool isSkill = false;
    protected float curSkillCoolTime = 0;

    //��Ÿ�� üũ
    public void CheckSkillCoolTime()
    {
        try
        {
            curSkillCoolTime += Time.deltaTime;

            if (curSkillCoolTime > stat.skillCoolTime)
            {
                if (isEnemy || manager.isAutoSkill)
                {
                    isSkill = true;
                    curSkillCoolTime = 0;
                    AddSkillQueue();
                }
            }
        }
        catch
        {
            Debug.Log("����");
        }

    }

    //��ų ����
    public virtual void Skill()
    {
        animator.SetTrigger("Skill");
    }

    //��ų ť �߰�
    public void AddSkillQueue()
    {
        manager.SetSkillQueue((this,
            () => {
            if (skillBtn != null)
            {
                skillBtn.StartCheckCoolTime(this.Stat.skillCoolTime);
            }
            AttackCallBack();
            animator.SetTrigger("Skill"); 
            }));
    }

    //�ִϸ��̼� ������ �ݹ�
    public void SkillCallBack()
    {
        isSkill = false;
    }


    IEnumerator AnimationDelayCo(UnityAction beforeAction, UnityAction afterAction)
    {
        beforeAction?.Invoke();

        yield return attackWaitTime;

        afterAction?.Invoke();

    }

    IEnumerator AnimationDelaySkillCo(UnityAction beforeAction, UnityAction afterAction)
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        beforeAction?.Invoke();

        yield return new WaitForSecondsRealtime(0.5f);

        animator.updateMode = AnimatorUpdateMode.Normal;
        afterAction?.Invoke();

    }


    #endregion


    #region ������ ����

    //������ �¾��� ��
    public void GetDamage(Entity hitEntity)
    {
        float damage = 0;
        float defense = (this.Stat.def - hitEntity.Stat.penetration) < 0 ? 0 : this.Stat.def - hitEntity.Stat.penetration;

        if (Random.Range(0, 101) < hitEntity.Stat.criticalHitRate)
        {
            damage = hitEntity.Stat.atk * (hitEntity.Stat.criticalHitDamage / 100) - defense;

            if (damage < 0) damage = 0;
            manager.SetDamageText(damage, this.transform.position, TextType.Critical);
        }
        else
        {
            damage = hitEntity.Stat.atk - defense;

            if (damage < 0) damage = 0;
            manager.SetDamageText(damage, this.transform.position, TextType.Damage);
        }

        StartCoroutine(HitColorCoroutine());

        stat.curHP -= damage;
        SetHpBar();
        hitEntity.SetDamageGraph(damage);
        manager.SetTopSlider();

        if (Stat.curHP < 0)
        {
            stat.curHP = 0;
            hitEntity.killCount++;
            ChangeState(State.Death);
        }
    }


    IEnumerator HitColorCoroutine()
    {
        this.sprRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        this.sprRenderer.color = Color.white;
    }

    #endregion

    #region UI ����

    //���׷���, ��ų ��ư UI ����
    public void ConnectUI(DamageGraph damageGraph, SkillBtn skillBtn)
    {
        this.damageGraph = damageGraph;
        this.skillBtn = skillBtn;
    }

    //���� �� ������ �׷��� ����
    public void SetDamageGraph(float damage)
    {
        if (damageGraph != null)
        {
            this.damageGraph.UpdateDamage(damage);
        }
    }

    //Hp�� ����
    public void SetHpBar()
    {
        hpBar.fillAmount = stat.curHP / stat.maxHP;

        if (skillBtn != null)
        {
            skillBtn.SetHp(this);
        }
    }

    #endregion


    #region ��ƿ

    // �ݴ� �������� ������
    public void Reverse()
    {
        this.sprRenderer.flipX = !this.sprRenderer.flipX;
    }

    //������ Y�� ����ȭ
    public float NomarlizedPosY()
    {
        return (this.transform.position.y - positionYLimit.x) / (positionYLimit.y - positionYLimit.x);
    }


    //����ȭ �� Y������ �������� ������ȭ
    public void SetSizeByPosY()
    {
        float deNormalizedScale = NomarlizedPosY() * (sizeLimit.y - sizeLimit.x) + sizeLimit.x;
        this.transform.localScale = new Vector3(deNormalizedScale, deNormalizedScale, deNormalizedScale);
    }

    #endregion
}
