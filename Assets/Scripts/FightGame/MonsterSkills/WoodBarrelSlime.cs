using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBarrelSlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int slimeAmount = 3;
    [SerializeField] private float instSlimeTime = 1;
    [SerializeField] private GameObject instSmoke;//魔法煙霧(移動至生成定點再產生史萊姆)
    [SerializeField] private GameObject woodBarrelSlimeObject;
    [SerializeField] private Vector2 instPositionXRange = new Vector2(1.42f, -1.42f);
    [SerializeField] private Vector2 instPositionYRange = new Vector2(2.86f, 2.86f);

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
        woodBarrelSlimeObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/WoodBarrelSlime");
    }

    public void Active()
    {
        StartCoroutine(SpawnSlimesWithInterval());
    }

    private IEnumerator SpawnSlimesWithInterval()
    {
        List<Vector2> slimePositions = new List<Vector2>();

        for (int i = 0; i < slimeAmount; i++)
        {
            Vector2 targetPosition;
            bool validPosition = false;
            int maxAttempts = 10;
            int attempts = 0;

            do
            {
                float randomX = Random.Range(instPositionXRange.x, instPositionXRange.y);
                float randomY = Random.Range(instPositionYRange.x, instPositionYRange.y);
                targetPosition = new Vector2(randomX, randomY);
                attempts++;

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

            slimePositions.Add(targetPosition);

            // 生成煙霧
            GameObject CFS = Instantiate(instSmoke, targetPosition, Quaternion.identity);
            moveToPositionSkill moveScript = CFS.GetComponent<moveToPositionSkill>();
            moveScript.SetStartPosition(player2.transform.position);
            moveScript.SetTargetPosition(targetPosition);
            moveScript.SetArriveTime(instSlimeTime);

            // 延遲後再生成 slime
            StartCoroutine(DelayedInstSlime(targetPosition, instSlimeTime));

            // 接下來這個 slime 會延遲 0.3~1 秒才進行
            float delay = Random.Range(0.3f, 1.0f);
            yield return new WaitForSeconds(delay);
        }
    }




    private IEnumerator DelayedInstSlime(Vector2 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject obj = Instantiate(woodBarrelSlimeObject, position, Quaternion.identity);
        obj.GetComponent<Animator>().SetTrigger("Rolling");
    }
}
