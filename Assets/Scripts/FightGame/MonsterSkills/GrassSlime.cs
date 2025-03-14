using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int slimeAmount = 3;
    [SerializeField] private float instSlimeTime = 1;
    [SerializeField] private GameObject instSmoke;//魔法煙霧(移動至生成定點再產生史萊姆)
    [SerializeField] private GameObject grassSlimeObject;
    [SerializeField] private Vector2 instPositionXRange = new Vector2(1.42f,-1.42f);
    [SerializeField] private Vector2 instPositionYRange = new Vector2(0.35f,2.94f);

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;


    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_Yellow");
        grassSlimeObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/GrassSlime");
    }

    public void Active()
    {
        List<Vector2> slimePositions = new List<Vector2>();

        for (int i = 0; i < slimeAmount; i++)
        {
            Vector2 targetPosition;
            bool validPosition = false;
            int maxAttempts = 10; // 避免無窮迴圈
            int attempts = 0;

            do
            {
                float randomX = Random.Range(instPositionXRange.x, instPositionXRange.y);
                float randomY = Random.Range(instPositionYRange.x, instPositionYRange.y);
                targetPosition = new Vector2(randomX, randomY);
                attempts++;

                // 檢查是否與所有已經生成的位置間距離至少 (0.5, 0.5)
                validPosition = true;
                foreach (var pos in slimePositions)
                {
                    if (Mathf.Abs(pos.x - targetPosition.x) < 0.5f &&
                        Mathf.Abs(pos.y - targetPosition.y) < 0.5f)
                    {
                        validPosition = false;
                        break;
                    }
                }
            } while (!validPosition && attempts < maxAttempts);

            // 儲存這個位置，確保後續生成的史萊姆與之保持距離
            slimePositions.Add(targetPosition);

            GameObject CFS = Instantiate(instSmoke, targetPosition, Quaternion.identity);
            moveToPositionSkill moveScript = CFS.GetComponent<moveToPositionSkill>();
            moveScript.SetStartPosition(player2.transform.position);
            moveScript.SetTargetPosition(targetPosition);
            moveScript.SetArriveTime(instSlimeTime);

            StartCoroutine(DelayedInstSlime(targetPosition, instSlimeTime));
        }
    }



    private IEnumerator DelayedInstSlime(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = Instantiate(grassSlimeObject, position, Quaternion.identity);
    }

}
