using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveToPositionSkill : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private float arriveTime = 1;

    [SerializeField] private GameObject explosion;
    [SerializeField] private Vector2 targetPosition;

    private Vector2 startPosition;
    private float elapsedTime = 0;
    private bool isMoving = false;

    void Start()
    {
        //startPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / arriveTime);
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            if (t >= 1)
            {
                isMoving = false;
                if(explosion != null)
                    Instantiate(explosion, transform.position, Quaternion.identity);
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
        isMoving = true;
    }

    public void SetArriveTime(float aTime)
    {
        arriveTime = aTime;
    }
}