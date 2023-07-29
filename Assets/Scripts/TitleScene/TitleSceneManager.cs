using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
 

    //é�� ��ư
    public void ActionChapterBtn()
    {
        SceneManager.LoadSceneAsync("StageScene");
    }

    //���� ���� ��ư
    public void ActionExitBtn()
    {
        Application.Quit();
    }

    //����Ʈ ����
    public void ConnectToURL(string URL)
    {
        Application.OpenURL(URL);
    }

}
