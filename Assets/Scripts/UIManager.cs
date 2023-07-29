using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("[상단 UI]")]
    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Slider mySlider;
    [SerializeField]
    private Slider enemySlider;

    [Header("[데미지 그래프]")]
    [SerializeField]
    private RectTransform damageGraphRect;
    [SerializeField]
    private RectTransform arrowIcon;
    [SerializeField]
    private List<DamageGraph> damageGraphes;
    public List<DamageGraph> DamageGraphes => damageGraphes;

    [Header("[스킬 버튼]")]
    [SerializeField]
    private SkillBtn[] skillBtns;
    public SkillBtn[] SkillBtns => skillBtns;

    [Header("[데미지 텍스트]")]
    [SerializeField]
    private Transform damageTextCanavs;
    [SerializeField]
    private DamageText damageText;

    [Header("[스킬UI]")]
    [SerializeField]
    private Image skillUI;

    [SerializeField]
    private Image teamPortrait;
    [SerializeField]
    private GameObject TeamSkFrame;

    [SerializeField]
    private Image enemyPortrait;
    [SerializeField]
    private GameObject enemySkFrame;

    [Header("[결과창UI]")]
    [SerializeField]
    private ResultUI resultUI;

    [Header("[게임 매니저]")]
    [SerializeField]
    private GameManager gameManager;

    private void Awake()
    {
        SetGameTime();
    }

    //시작할때 UI초기화
    public void Init()
    {
        float teamAllHp = gameManager.GetAllHp(true);
        float enemyAllHp = gameManager.GetAllHp(false);

        mySlider.maxValue = teamAllHp;
        mySlider.value = teamAllHp;

        enemySlider.maxValue = enemyAllHp;
        enemySlider.value = enemyAllHp;

        for (int i = 0; i < gameManager.MyChars.Count; i++)
        {
            damageGraphes[i].Init(gameManager.MyChars[i], this);
            skillBtns[i].Init(gameManager.MyChars[i]);
        }
    }

    //스킬쓸때 UI
    public void SetSkillUI(bool isActive, Entity entity)
    {
        if (entity != null)
        {
            if (gameManager.MyChars.Contains(entity))
            {
                teamPortrait.sprite = entity.portrait;
                TeamSkFrame.SetActive(isActive);
            }
            else
            {
                enemyPortrait.sprite = entity.portrait;
                enemySkFrame.SetActive(isActive);
            }
        }

        skillUI.gameObject.SetActive(isActive);
    }

    //데미지 텍스트 생성
    public void SetDamageText(float value, Vector2 pos, TextType type)
    {
        Instantiate<DamageText>(damageText, damageTextCanavs).Set(value, pos, type);
    }

    //게임 시간 UI세팅
    public void SetGameTime()
    {
        timeText.text = ((int)(gameManager.gameTime / 60)).ToString("D2") + " : " + ((int)(gameManager.gameTime % 60)).ToString("D2");
    }

    //상단 hp바 세팅
    public void SetTopSliders()
    {
        mySlider.value = gameManager.GetAllHp(true);
        enemySlider.value = gameManager.GetAllHp(false);
    }

    public void SetResultUI(bool isClear)
    {
        StartCoroutine(GameUtill.DelayCoroutine(0.5f,
            callBack: () =>
            {
                damageTextCanavs.GetComponent<Canvas>().enabled = false;
                resultUI.SetResult(isClear, damageGraphes[0].ConnectEntity);
            }));
    }


    #region 딜그래프 기능

    private bool isHide = false;
    private Coroutine coHideGraph = null;

    //딜그래프 숨기기
    public void HideGraph()
    {
        isHide = !isHide;

        if (coHideGraph != null)
        {
            StopCoroutine(coHideGraph);
        }

        Vector2 targetPos = new Vector2(isHide ? -damageGraphRect.sizeDelta.x : 0, damageGraphRect.anchoredPosition.y);
        coHideGraph = StartCoroutine(GameUtill.MoveCoroutine(damageGraphRect, targetPos,0.1f,()=> arrowIcon.rotation = Quaternion.Euler(0, 0, isHide ? 90 : -90)));
    }


    //딜 그래프 정렬 후 이동
    public void CheckGraphRank()
    {
        damageGraphes.Sort((one, two) => two.TotalDamage.CompareTo(one.TotalDamage));

        for (int i = 0; i < damageGraphes.Count; i++)
        {
            damageGraphes[i].MoveGraph(i + 1);
        }
    }

    #endregion


    #region 메뉴 기능

    //타임 스케일 세팅
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    //다시 시작
    public void ReTry()
    {
        SetTimeScale(1.0f);
        GameUtill.MoveScene("InGameScene");
        
    }

    //포기 하기
    public void GiveUp()
    {
        SetTimeScale(1.0f);
        GameUtill.MoveScene("MainScene");
    }

    #endregion



}
