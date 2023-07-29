using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextType
{
    Damage,Critical,Heal
}

public class DamageText : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private float lifeTime;

    //데미지 텍스트 세팅
    public void Set(float value,Vector2 pos,TextType type)
    {
        this.transform.position = Camera.main.WorldToScreenPoint(pos);
        text.text = value.ToString("N0");

        switch (type)
        {
            case TextType.Damage:
                text.color = Color.white;
                break;
            case TextType.Critical:
                text.color = Color.red;
                break;
            case TextType.Heal:
                text.color = Color.green;
                break;
        }

        StartCoroutine(GameUtill.MoveCoroutineSpeed(this.transform,this.transform.position + (Vector3.up*100f),150f,
            callBack: () =>
            {
                Destroy(this.gameObject);
            }));
    }

    IEnumerator MoveTextCoroutine()
    {
        //float curTime = 0;

        while(lifeTime > 0.0f)
        {
            lifeTime -= Time.deltaTime;

            this.transform.position += Vector3.up * Time.unscaledDeltaTime * 250f;

            yield return null;
        }

        Destroy(gameObject);

    }


}
