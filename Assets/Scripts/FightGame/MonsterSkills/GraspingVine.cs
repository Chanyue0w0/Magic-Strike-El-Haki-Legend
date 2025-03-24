using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraspingVine : MonoBehaviour
{
    [Header("----------------- Skill info ------------------")]
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private int targetNumber = 2;
    [SerializeField] private int skillDamage = 100;
    [SerializeField] private GameObject explosion;

    [SerializeField] private float moveSpeed = 2f;  // 向下移動的速度
    [SerializeField] private float scaleSpeed = 0.1f;  // 向下移動的縮放速度

    [SerializeField] private float backMoveSpeed = 2f;  // 向上移動的速度
    [SerializeField] private float backScaleSpeed = 0.1f;  // 向上移動的縮放速度

    [SerializeField] private SpriteRenderer vineMiddle;  // 參考到CandyHook_Stick的SpriteRenderer
    [SerializeField] private Transform vineEnd; // 參考到CandyHook_Hook
    [SerializeField] private float initialSizeY;  // 記錄初始的Stick大小

    [SerializeField] private bool isMoving = true;
    [SerializeField] private bool canHookedOnce = true;//只觸發拉動一次
    //private bool isBackMoving = false;
    
    [SerializeField] private StatusEffect EffectToApply = StatusEffect.Grasp; // 要套用的狀態

    void Start()
    {
        // 記錄初始大小
        initialSizeY = vineMiddle.size.y;
        canHookedOnce = true;
    }

    void Update()
    {
        if (isMoving)
        {
            // 整個CandyHook向下移動
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

            // 拉長CandyHook_Stick向上
            Vector2 newSize = vineMiddle.size;
            newSize.y += scaleSpeed * Time.deltaTime;
            vineMiddle.size = newSize;

            // 更新CandyHook_Stick的位置使其向上延伸
            vineMiddle.transform.localPosition = new Vector3(
                vineMiddle.transform.localPosition.x,
                newSize.y / 2,  // 確保拉長時始終往上延伸
                vineMiddle.transform.localPosition.z
            );

        }
        else
        {
            // 整個CandyHook向下移動
            transform.Translate(Vector2.up * backMoveSpeed * Time.deltaTime);

            // 拉長CandyHook_Stick向上
            Vector2 newSize = vineMiddle.size;
            newSize.y -= backScaleSpeed * Time.deltaTime;
            vineMiddle.size = newSize;

            // 更新CandyHook_Stick的位置使其向上延伸
            vineMiddle.transform.localPosition = new Vector3(
                vineMiddle.transform.localPosition.x,
                newSize.y / 2,  // 確保拉長時始終往上延伸
                vineMiddle.transform.localPosition.z
            );

            // 確保不會小於初始大小
            if (newSize.y <= initialSizeY)
            {
                newSize.y = initialSizeY;

                //FightStatus.boss2UsingHook = false;//重製Boss2使用鉤子自身固定
                // 刪除物件
                Destroy(gameObject);
            }

        }

        if (gameObject.transform.position.y <= -4f)//長度過長
        {
            //FightStatus.boss2UsingHook = false;//重製Boss2使用鉤子自身固定
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果CandyHook_Hook碰到標記為"Player"的物件
        if (collision.CompareTag("Player1") && canHookedOnce)
        {
            // 嘗試獲取 IDamageable 介面（目標可受傷）
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(skillDamage);
                //Debug.Log($"{collision.gameObject.name} 受到 {NormalAttackDamage} 傷害！");
            }

            // 嘗試獲取 IStatusEffectReceiver 介面（目標可受 Buff/Debuff）
            IStatusEffectReceiver statusReceiver = collision.GetComponent<IStatusEffectReceiver>();
            if (statusReceiver != null)
            {
                statusReceiver.ApplyStatusEffect(EffectToApply);
                //Debug.Log($"{collision.gameObject.name} 受到狀態影響：{EffectToApply}");
            }

            // 停止移動與拉長
            Invoke("StopHook", 0.1f);
            //FightStatus.playerIsDragged = true;
            canHookedOnce = false;
        }
    }

    public void StopHook()
    {
        isMoving = false;
        //isBackMoving = true;
    }
}
