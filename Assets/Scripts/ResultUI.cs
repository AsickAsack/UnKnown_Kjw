using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject mvpPanel;

    [SerializeField]
    private Text titleText;

    [SerializeField]
    private Text killText;


    //결과창 세팅
    public void SetResult(bool isClear,Entity mvpEntity = null)
    {
        canvas.enabled = true;

        if (mvpEntity != null)
        {
            mvpEntity.transform.localScale *= 2;
            mvpEntity.sprRenderer.sortingOrder = 100;
            mvpEntity.transform.position = Vector2.zero;

            killText.text = $"{mvpEntity.killCount} kill";
        }

        if (isClear)
        {
            titleText.text = "Victory!";
            titleText.color = Color.yellow;

            mvpPanel.SetActive(true);
        }
        else
        {
            titleText.text = "Fail...";
            titleText.color = Color.red;
        }
    }


}
