using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToPositionSkill : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private float arriveTime = 1;
    [SerializeField] private float delayTime = 0f;

    [SerializeField] private GameObject explosion;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool onTargetDestroyed = false;
    [SerializeField] private bool movingUnscaledTime = false;//不受時間暫停影響

    private Vector2 startPosition;
    private float elapsedTime = 0;
    private bool isMoving = false;

    void Start()
    {
        startPosition = this.transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            //elapsedTime += Time.deltaTime;
            // 根據是否使用 Unscaled Time，選擇對應的 deltaTime
            float delta = movingUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += delta;

            float t = Mathf.Clamp01(elapsedTime / arriveTime);
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            if (t >= 1)
            {
                isMoving = false;
                if(explosion != null)
                    Instantiate(explosion, transform.position, Quaternion.identity);

                if (onTargetDestroyed)
                    Destroy(gameObject);
            }
        }
    }

    public void SetStartPosition(Vector2 sPosition)
    {
        startPosition = sPosition;
    }

    public void SetTargetPosition(Vector2 tPosition)
    {
        targetPosition = tPosition;
        //startPosition = transform.position;
        elapsedTime = 0;
        isMoving = false;
        StartCoroutine(StartMovingAfterDelay());
    }

    public void SetArriveTime(float aTime)
    {
        arriveTime = aTime;
    }

    public void SetDelayTime(float dTime)
    {
        delayTime = dTime;
    }

    private IEnumerator StartMovingAfterDelay()
    {
        //yield return new WaitForSeconds(delayTime);
        yield return new WaitForSecondsRealtime(delayTime);
        isMoving = true;
    }
}