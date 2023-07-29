using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : MonoBehaviour
{
    [SerializeField]
    private Image charIcon;
    [SerializeField]
    private Image coolTimeImg;
    [SerializeField]
    private Text coolTimeText;
    [SerializeField]
    private Image deathImage;

    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    private Button btn;

    private Coroutine coCoolTime;


    //��ų ��ư �ʱ�ȭ
    public void Init(Entity entity)
    {
        charIcon.sprite = entity.portrait;
        coolTimeText.text = entity.Stat.skillCoolTime.ToString();
        btn.onClick.AddListener(() => entity.AddSkillQueue());
    }

    //HP UI ����
    public void SetHp(Entity entity)
    {
        hpSlider.value = entity.Stat.curHP / entity.Stat.maxHP;
    }

    //��Ÿ�� üũ
    public void StartCheckCoolTime(float coolTime)
    {
        if(coCoolTime != null)
        {
            StopCoroutine(coCoolTime);
        }

        coCoolTime = StartCoroutine(CoolTimeCoroutine(coolTime));
    }

    //��Ÿ�� üũ �ڷ�ƾ
    IEnumerator CoolTimeCoroutine(float coolTime)
    {
        btn.interactable = false;
        coolTimeImg.gameObject.SetActive(true);
        float curTime = coolTime;

        while(curTime > 0.0f)
        {
            curTime -= Time.deltaTime;

            coolTimeText.text = curTime.ToString("N0");
            coolTimeImg.fillAmount = curTime / coolTime;

            yield return null;
        }

        coolTimeImg.gameObject.SetActive(false);
        btn.interactable = true;

    }

    //����� ĳ���Ͱ� �׾����� 
    public void DeathRoutine()
    {
        if (coCoolTime != null)
        {
            StopCoroutine(coCoolTime);
        }

        btn.interactable = false;

        
        coolTimeText.gameObject.SetActive(false);
        deathImage.gameObject.SetActive(true);

        coolTimeImg.fillAmount = 1;
        coolTimeImg.color = Color.red;
        coolTimeImg.gameObject.SetActive(true);

        charIcon.rectTransform.anchoredPosition = new Vector2(charIcon.rectTransform.anchoredPosition.x, charIcon.rectTransform.anchoredPosition.y - 20);

    }

}
