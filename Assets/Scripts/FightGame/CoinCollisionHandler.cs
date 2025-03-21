using UnityEngine;

public class CoinCollisionHandler : MonoBehaviour
{
    public GameObject coinPrefab;  // 你要實際生成的硬幣 Prefab

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps == null) return;

        ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];
        int numCollisionEvents = ps.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            // 在碰撞位置生成硬幣
            Instantiate(coinPrefab, collisionEvents[i].intersection, Quaternion.identity);
        }
    }
}
