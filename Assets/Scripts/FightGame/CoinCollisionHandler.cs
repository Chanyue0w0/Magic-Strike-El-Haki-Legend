using UnityEngine;
using System.Collections.Generic;

public class CoinCollisionHandler : MonoBehaviour
{
    public GameObject coinPrefab;  // 你要實際生成的硬幣 Prefab
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps == null) return;

        // 取得碰撞事件並儲存到 List 中
        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);

        GameObject player1 = GameObject.Find("Player1");

        for (int i = 0; i < numCollisionEvents; i++)
        {
            // 在碰撞位置生成硬幣
            GameObject obj = Instantiate(coinPrefab, collisionEvents[i].intersection, Quaternion.identity);
            obj.GetComponent<moveToPositionSkill>().SetTargetPosition(player1.transform.position);
            obj.GetComponent<moveToPositionSkill>().SetDelayTime(1f);
        }
    }
}
