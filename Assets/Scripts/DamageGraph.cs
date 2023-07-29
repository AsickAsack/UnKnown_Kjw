using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageGraph : MonoBehaviour
{
    [SerializeField]
    private RectTransform myRect;

    [SerializeField]
    private Image charIcon;
    [SerializeField]
    private Text damageTx;
    [SerializeField]
    private Image damageBar;

    private UIManager manager;

    private float totalDamage;
    public float TotalDamage => totalDamage;

    private int rank = 0;
    private Coroutine moveCo;

    private Entity connectEntity;
    public Entity ConnectEntity => connectEntity;
    //�׷��� �ʱ�ȭ
    public void Init(Entity entity,UIManager manager)
    {
        this.manager = manager;
        this.connectEntity = entity;
        charIcon.sprite = entity.portrait;
    }


    //������ �׷��� ������Ʈ => ��Ż �������� ���� ����
    public void UpdateDamage(float damage)
    {
        totalDamage += damage;
        damageTx.text = totalDamage.ToString("N0");

        // �����ϸ��� �� ���
        float scaledValue = (float)(totalDamage / (Mathf.Pow(10,Mathf.Log10(manager.DamageGraphes[0].totalDamage)+1)));

        damageBar.rectTransform.sizeDelta = new Vector2(150 * scaledValue, damageBar.rectTransform.sizeDelta.y);
        manager.CheckGraphRank();
    }

    //���� ��ũ�� �׷��� ��ġ �̵�
    public void MoveGraph(int rank)
    {
        if(this.rank == rank) return;

        this.rank = rank;

        if(moveCo != null)
        {
            StopCoroutine(moveCo);
        }

        moveCo = StartCoroutine(GameUtill.MoveCoroutine(myRect, new Vector2(myRect.anchoredPosition.x, (-100 * rank) + 50), 0.1f));
    }



}
