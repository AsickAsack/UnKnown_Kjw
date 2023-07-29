using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField]
    private Text stageTitle;
    [SerializeField]
    private Image[] portraits;
    [SerializeField]
    private Button readyButton;

    [SerializeField]
    private RectTransform contentRect;

    //스테이지 ui세팅
    public void SetStageUI(Stage stage)
    {
        contentRect.anchoredPosition = Vector2.zero;

        foreach(var p in portraits)
        {
            if (p != null)
            {
                p.gameObject.SetActive(false);
            }
        }

        stageTitle.text = $"스테이지 {stage.stageName}";

        for (int i = 0; i < stage.StageMonsters.Count; i++)
        {
            portraits[i].sprite = stage.StageMonsters[i].monster.portrait;
            portraits[i].gameObject.SetActive(true);
        }

        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(()=> ActionReadyBtn(stage));

        this.gameObject.SetActive(true);

    }

    //전투준비 버튼 눌렀을때
    public void ActionReadyBtn(Stage stage)
    {
        Player.instance.SetStageMonster(stage.StageMonsters);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("CharacterSelectScene");
    }


}
