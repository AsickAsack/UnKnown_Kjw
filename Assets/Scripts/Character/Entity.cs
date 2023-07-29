using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//상태
public enum State
{
    None, Move, Fight, Death
}

//캐릭터 타입
public enum CharType
{
    Warrior, Assassin, Wizard
}


public class Entity : FSM
{

    [Header("[스텟]")]
    [SerializeField]
    protected Stat stat;
    public Stat Stat => stat;
    public CharType charType;

    [Header("[상태]")]
    public bool isEnemy = false;

    [Header("[컴포넌트]")]
    public Image hpBar;
    public Animator animator;
    public SpriteRenderer sprRenderer;

    [Header("[초상화]")]
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


    #region 초기 세팅

    //초기화 
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

    //초기화 콜백 코루틴
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

    //암살 움직임
    public void MoveAssassination()
    {
        StartCoroutine(GameUtill.MoveCoroutine(this.transform, enemy.transform.position + (isEnemy ? Vector3.left : Vector3.right), 0.2f,
            callBack: () =>
            {
                Reverse();
                ChangeState(State.Move);
            }));
    }

    #region 상태 변경


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

    #region 상태별 루틴
    protected override void MoveStateRoutine()
    {
        if (enemy != null)
        {
            if (Vector2.Distance(this.transform.position, enemy.transform.position) < stat.atkRange)
            {
                //공격
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

    #region 일반 공격

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

    //애니메이션 끝날때 콜백
    public void AttackCallBack()
    {
        isAttack = false;
    }

    #endregion

    #region 스킬 관련

    public bool isSkill = false;
    protected float curSkillCoolTime = 0;

    //쿨타임 체크
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
            Debug.Log("오류");
        }

    }

    //스킬 실행
    public virtual void Skill()
    {
        animator.SetTrigger("Skill");
    }

    //스킬 큐 추가
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

    //애니메이션 끝날때 콜백
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


    #region 데미지 관련

    //데미지 맞았을 때
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

    #region UI 연동

    //딜그래프, 스킬 버튼 UI 연동
    public void ConnectUI(DamageGraph damageGraph, SkillBtn skillBtn)
    {
        this.damageGraph = damageGraph;
        this.skillBtn = skillBtn;
    }

    //때릴 때 데미지 그래프 세팅
    public void SetDamageGraph(float damage)
    {
        if (damageGraph != null)
        {
            this.damageGraph.UpdateDamage(damage);
        }
    }

    //Hp바 세팅
    public void SetHpBar()
    {
        hpBar.fillAmount = stat.curHP / stat.maxHP;

        if (skillBtn != null)
        {
            skillBtn.SetHp(this);
        }
    }

    #endregion


    #region 유틸

    // 반대 방향으로 뒤집기
    public void Reverse()
    {
        this.sprRenderer.flipX = !this.sprRenderer.flipX;
    }

    //포지션 Y값 정규화
    public float NomarlizedPosY()
    {
        return (this.transform.position.y - positionYLimit.x) / (positionYLimit.y - positionYLimit.x);
    }


    //정규화 된 Y값으로 스케일을 역정규화
    public void SetSizeByPosY()
    {
        float deNormalizedScale = NomarlizedPosY() * (sizeLimit.y - sizeLimit.x) + sizeLimit.x;
        this.transform.localScale = new Vector3(deNormalizedScale, deNormalizedScale, deNormalizedScale);
    }

    #endregion
}
