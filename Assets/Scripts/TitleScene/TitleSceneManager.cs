using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
 

    //챕터 버튼
    public void ActionChapterBtn()
    {
        SceneManager.LoadSceneAsync("StageScene");
    }

    //게임 종료 버튼
    public void ActionExitBtn()
    {
        Application.Quit();
    }

    //사이트 접속
    public void ConnectToURL(string URL)
    {
        Application.OpenURL(URL);
    }

}
