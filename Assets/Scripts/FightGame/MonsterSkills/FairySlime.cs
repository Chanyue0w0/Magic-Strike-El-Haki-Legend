using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairySlime : MonoBehaviour
{
    [SerializeField] private int playerNumber = 2;
    [SerializeField] private GameObject mistAmmoObject;
    [SerializeField] private int mistAmmoAmount = 3;

    [SerializeField] private GameObject flashExplosion;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [SerializeField] private AIController player2_controller;

    [SerializeField] private Animator player2_animator;

    [SerializeField] private Vector2 player2_TopLeftBoundary; //(-1.4f,2.8f)
    [SerializeField] private Vector2 player2_ButtomRightBoundary; //(1.4f,0.4f)


    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        player2_controller = GameObject.Find("Player 2 Manager").GetComponent<AIController>();
        player2_animator = GameObject.Find("Player2Sprite").GetComponent<Animator>();
        //instSmoke = Resources.Load<GameObject>("Prefabs/MonsterSkills/MagicPowerGain_Yellow");
        mistAmmoObject = Resources.Load<GameObject>("Prefabs/MonsterSkills/MistAmmo");
        flashExplosion = Resources.Load<GameObject>("Prefabs/Skills/Explosions/GreenExplosion");

        player2_TopLeftBoundary = new Vector2(-1.4f, 2.8f);
        player2_ButtomRightBoundary = new Vector2(1.4f, 0.4f);
    }

    public void Active()
    {
        player2_animator.SetTrigger("Attack");
        //player2_controller.SetStopMoving(true);

        // 第一次爆炸（原本位置）
        Instantiate(flashExplosion, player2.transform.position, Quaternion.identity);

        // 瞬間移動到邊界內隨機位置
        Vector2 randomPos = new Vector2(
            Random.Range(player2_TopLeftBoundary.x, player2_ButtomRightBoundary.x),
            Random.Range(player2_ButtomRightBoundary.y, player2_TopLeftBoundary.y)
        );
        player2.transform.position = randomPos;

        // 第二次爆炸（新位置）
        Instantiate(flashExplosion, player2.transform.position, Quaternion.identity);

        player2_controller.SetStopMoving(true);
        // 發射 MistAmmo
        StartCoroutine(SpawnMultipleMistAmmo(0f));

        // 如需之後恢復移動，可以加這行（延遲幾秒後）
        //StartCoroutine(DelayedStartMoving(3f));
    }


    private IEnumerator SpawnMultipleMistAmmo(float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < mistAmmoAmount; i++)
        {
            // 設定 X 軸的隨機偏移量（範圍可依需求調整）
            float randomXOffset = Random.Range(-0.5f, 0.5f);
            Vector3 spawnPosition = player2.transform.position + new Vector3(randomXOffset, 0, 0);

            GameObject obj = Instantiate(mistAmmoObject, spawnPosition, Quaternion.Euler(90, 0, 0));
            obj.GetComponent<AimPlayerShootSkill>().SetPlayerNumber(2);
            obj.GetComponent<AimPlayerShootSkill>().SetTargetNumber(1);

            yield return new WaitForSeconds(0.3f);
        }

        player2_controller.SetStopMoving(false);
    }


    private IEnumerator DelayedStartMoving(float delay)
    {
        yield return new WaitForSeconds(delay);
        player2_controller.SetStopMoving(false);
    }
}
