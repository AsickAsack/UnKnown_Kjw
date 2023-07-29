using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSceneManager : MonoBehaviour
{

    public float moveSpeed;
    public float zoomSpeed;

    public Vector2 moveLimitX;
    public Vector2 moveLimitY;

    public float minZoomDistance;
    public float maxZoomDistance;
    
    private int fingerCount;
    private Vector2 touchStart;
    private float zoomDistance;

    private float startOthorSize;

    [SerializeField]
    private StageUI stageUI;

    private void Awake()
    {
        startOthorSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (stageUI.gameObject.activeInHierarchy) return;

        fingerCount = Input.touchCount;

        if (fingerCount == 2) //2개일때 줌
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaDist = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaDist = (touchZero.position - touchOne.position).magnitude;

            float deltaDist = prevTouchDeltaDist - touchDeltaDist;

            zoomDistance += deltaDist * zoomSpeed * Time.deltaTime;
            zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);

            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomDistance, Time.deltaTime * 5f);

            Vector3 newMovePos = Camera.main.transform.position;

            float cul = Mathf.Abs(startOthorSize - Camera.main.orthographicSize) * Camera.main.aspect;
            newMovePos.x = Mathf.Clamp(newMovePos.x, moveLimitX.x - cul, moveLimitX.y + cul);
            newMovePos.y = Mathf.Clamp(newMovePos.y, moveLimitY.x + (startOthorSize - Camera.main.orthographicSize), moveLimitY.y - (startOthorSize - Camera.main.orthographicSize));

            Camera.main.transform.position = newMovePos;
        }
        else if (fingerCount == 1) //터치 한개일때 이동
        {

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                
                Vector3 delta = touch.position - touchStart;
                Vector3 newMovePos = Camera.main.transform.position;

                newMovePos -= delta * moveSpeed * Time.deltaTime;

                float cul = Mathf.Abs(startOthorSize - Camera.main.orthographicSize) * Camera.main.aspect;
                newMovePos.x = Mathf.Clamp(newMovePos.x, moveLimitX.x - cul, moveLimitX.y + cul);
                newMovePos.y = Mathf.Clamp(newMovePos.y, moveLimitY.x + (startOthorSize - Camera.main.orthographicSize), moveLimitY.y - (startOthorSize - Camera.main.orthographicSize));

                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newMovePos, Time.deltaTime * 5f);
                //Camera.main.transform.position = newMovePos;

                touchStart = touch.position;
            }
        }
        
    }
}
