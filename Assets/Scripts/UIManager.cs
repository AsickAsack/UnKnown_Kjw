using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("[��� UI]")]
    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Slider mySlider;
    [SerializeField]
    private Slider enemySlider;

    [Header("[������ �׷���]")]
    [SerializeField]
    private RectTransform damageGraphRect;
    [SerializeField]
    private RectTransform arrowIcon;
    [SerializeField]
    private List<DamageGraph> damageGraphes;
    public List<DamageGraph> DamageGraphes => damageGraphes;

    [Header("[��ų ��ư]")]
    [SerializeField]
    private SkillBtn[] skillBtns;
    public SkillBtn[] SkillBtns => skillBtns;

    [Header("[������ �ؽ�Ʈ]")]
    [SerializeField]
    private Transform damageTextCanavs;
    [SerializeField]
    private DamageText damageText;

    [Header("[��ųUI]")]
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

    [Header("[���âUI]")]
    [SerializeField]
    private ResultUI resultUI;

    [Header("[���� �Ŵ���]")]
    [SerializeField]
    private GameManager gameManager;

    private void Awake()
    {
        SetGameTime();
    }

    //�����Ҷ� UI�ʱ�ȭ
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

    //��ų���� UI
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

    //������ �ؽ�Ʈ ����
    public void SetDamageText(float value, Vector2 pos, TextType type)
    {
        Instantiate<DamageText>(damageText, damageTextCanavs).Set(value, pos, type);
    }

    //���� �ð� UI����
    public void SetGameTime()
    {
        timeText.text = ((int)(gameManager.gameTime / 60)).ToString("D2") + " : " + ((int)(gameManager.gameTime % 60)).ToString("D2");
    }

    //��� hp�� ����
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


    #region ���׷��� ���

    private bool isHide = false;
    private Coroutine coHideGraph = null;

    //���׷��� �����
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


    //�� �׷��� ���� �� �̵�
    public void CheckGraphRank()
    {
        damageGraphes.Sort((one, two) => two.TotalDamage.CompareTo(one.TotalDamage));

        for (int i = 0; i < damageGraphes.Count; i++)
        {
            damageGraphes[i].MoveGraph(i + 1);
        }
    }

    #endregion


    #region �޴� ���

    //Ÿ�� ������ ����
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    //�ٽ� ����
    public void ReTry()
    {
        SetTimeScale(1.0f);
        GameUtill.MoveScene("InGameScene");
        
    }

    //���� �ϱ�
    public void GiveUp()
    {
        SetTimeScale(1.0f);
        GameUtill.MoveScene("MainScene");
    }

    #endregion



}
