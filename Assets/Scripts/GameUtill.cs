using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class GameUtill
{

    public static IEnumerator MoveCoroutine(Transform tr,Vector2 targetPos,float moveTime,UnityAction initAction = null,UnityAction callBack = null)
    {
        initAction?.Invoke();

        float curTime = 0;
        Vector2 startPos = tr.position;

        while(curTime < moveTime)
        {
            curTime += Time.deltaTime;
            tr.transform.position = Vector2.Lerp(startPos, targetPos, curTime/moveTime);

            yield return null;
        }

        tr.position = targetPos;

        callBack?.Invoke();
    }

    public static IEnumerator MoveCoroutine(Transform tr, Vector2 targetPos, float moveTime, UnityAction initAction = null, IEnumerator callBack = null)
    {
        initAction?.Invoke();

        float curTime = 0;
        Vector2 startPos = tr.position;

        while (curTime < moveTime)
        {
            curTime += Time.deltaTime;
            tr.transform.position = Vector2.Lerp(startPos, targetPos, curTime / moveTime);

            yield return null;
        }

        tr.position = targetPos;

        yield return callBack;
    }


    public static IEnumerator MoveCoroutine(RectTransform tr, Vector2 targetPos,float moveTime, UnityAction initAction = null, UnityAction callBack = null)
    {
        initAction?.Invoke();

        float curTime = 0;
        Vector2 startPos = tr.anchoredPosition;

        while (curTime < moveTime)
        {
            curTime += Time.deltaTime;
            tr.anchoredPosition = Vector2.Lerp(startPos, targetPos, curTime / moveTime);

            yield return null;
        }

        tr.anchoredPosition = targetPos;

        callBack?.Invoke();
    }

    public static IEnumerator MoveCoroutineSpeed(Transform tr, Vector3 targetPos,float speed, UnityAction initAction = null, UnityAction callBack = null)
    {
        initAction?.Invoke();

        Vector2 startPos = tr.position;
        float dist = (targetPos - tr.transform.position).magnitude;
        float delta = 0;

        while (delta < dist)
        {
            delta += Time.deltaTime * speed;
            tr.transform.position = Vector2.Lerp(startPos, targetPos, delta / dist);

            yield return null;
        }

        tr.position = targetPos;

        callBack?.Invoke();
    }

    public static IEnumerator DelayCoroutine(float delayTime,UnityAction initAction = null, UnityAction callBack = null)
    {
        initAction?.Invoke();

        yield return new WaitForSeconds(delayTime);

        callBack?.Invoke();
    }



    // 인게임 정해진 위치 리턴
    public static Vector2 GetPos(int x, int y, bool isEnemy)
    {

        Vector2 pos = new Vector2(y * 0.1f + ((x + 1) * 1.5f), y * -0.8f);

        if (!isEnemy)
        {
            pos.x = -pos.x;
        }

        return pos;

    }

    //씬이동
    public static void MoveScene(string SceneName)
    {
        SceneManager.LoadSceneAsync(SceneName);
    }


}
