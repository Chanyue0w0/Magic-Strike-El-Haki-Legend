using UnityEngine;

public class CoinSpawnerOnCollision : MonoBehaviour
{
    public GameObject coinPrefab; // 你的硬幣Prefab

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle hit detected!"); // 有進來嗎？
        // 取得所有碰撞點
        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        if (ps == null) return;

        ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];
        int numEvents = ps.GetCollisionEvents(gameObject, collisionEvents);

        for (int i = 0; i < numEvents; i++)
        {
            Vector3 spawnPos = collisionEvents[i].intersection;
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}
