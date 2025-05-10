using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBall : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;
    //[SerializeField] private int skillDamage = 100;
    [SerializeField] private GameObject cloneBallObj;
    [SerializeField] private GameObject explosion;
    [SerializeField] private int cloneBallAmount = 3;

    //[SerializeField] private GameObject player1;
    //[SerializeField] private GameObject player2;
    [SerializeField] private GameObject ball;

    [SerializeField] private GameObject iceBallEffectPrefab;
    [SerializeField] private GameObject burningBallEffectPrefab;

    public void SetPlayerNumber(int pNumber) // initial
    {
        playerNumber = pNumber;
        InitializedSkillInfo();
    }

    public void InitializedSkillInfo()
    {
        //float skillDamageTMP = 0;
        cloneBallObj = Resources.Load<GameObject>("Prefabs/Balls/CloneBall");

        explosion = Resources.Load<GameObject>("Prefabs/Skills/Explosions/SmokeExplosion");
        iceBallEffectPrefab = Resources.Load<GameObject>("Prefabs/Effect/IceBallEffect");
        burningBallEffectPrefab = Resources.Load<GameObject>("Prefabs/Effect/BurningBallEffect");
        //player1 = GameObject.FindGameObjectWithTag("Player1");
        //player2 = GameObject.FindGameObjectWithTag("Player2");
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public void Active()
    {

        StartCoroutine(DelayedInstCloneBall(0f));
    }

    private IEnumerator DelayedInstCloneBall(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (explosion != null)
            Instantiate(explosion, ball.transform.position, Quaternion.identity);

        string[] passiveSkills = FightPlayer1Config.PassiveEffectGroup;

        


        // ﹚竡よ秖
        Vector2[] directions = new Vector2[]
        {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left
        };

        for (int i = 0; i < cloneBallAmount; i++)
        {
            GameObject obj = Instantiate(cloneBallObj, ball.transform.position, Quaternion.identity);

            //IceBall
            if (System.Array.Exists(passiveSkills, s => s == "PS01"))
            {
                GameObject iceBallEffect = Instantiate(iceBallEffectPrefab, ball.transform.position, Quaternion.identity, ball.transform);
                iceBallEffect.transform.SetParent(obj.transform);
            }

            //BurningBall
            if (System.Array.Exists(passiveSkills, s => s == "PS02"))
            {
                GameObject burningBallEffect = Instantiate(burningBallEffectPrefab, ball.transform.position, Quaternion.identity, ball.transform);
                burningBallEffect.transform.SetParent(obj.transform);
            }


            // т "Sprite" ン
            Transform spriteTransform = obj.transform.Find("Sprite");
            if (spriteTransform != null)
            {
                SpriteRenderer sr = spriteTransform.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // 沮隔畖更 Sprite 戈方安砞璶更 Resources/Sprites/Balls/ChapterXBallSprite
                    string path = "Arts/FightScene/Field/FieldObjects/Chapter" + FightPlayer1Config.CurrentChapter + "BallSprite";
                    Sprite newSprite = Resources.Load<Sprite>(path);
                    if (newSprite != null)
                    {
                        sr.sprite = newSprite;
                    }
                    else
                    {
                        Debug.LogWarning("тぃ﹚ Sprite" + path);
                    }
                }
            }

            // 倒ぉ硉
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = directions[i % directions.Length];
                float force = 5f;
                rb.velocity = dir * force;
            }
        }

    }

}
