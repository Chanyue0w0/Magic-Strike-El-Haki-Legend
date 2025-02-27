using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstUlt : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1; // 使用玩家
    [SerializeField] private int targetNumber = 2; // 目標玩家
    [SerializeField] private string ultName; // 大招名稱
    [SerializeField] private int ultDamage = 200; // 大招傷害
    [SerializeField] private float instTime = 0.7f; // 刪除動畫特效時間
    [SerializeField] private Vector2 instPosition = new Vector2(0,2); // 生成位置
    [SerializeField] private Quaternion quaternion; // 旋轉角度
    [SerializeField] private GameObject ultGameObject; // 大招技能物 - 透過拖曳取得

    void Start()
    {
        StartCoroutine(InstUltSkill(instTime)); // 依據非受影響時間刪除
    }

    private IEnumerator InstUltSkill(float time)
    {
        yield return new WaitForSecondsRealtime(time); // 使用 Unscaled 時間來計算
        GameObject obj = Instantiate(ultGameObject, instPosition, quaternion);

        if (ultName == "HammerUlt")
        {
            obj.GetComponent<HammerUlt>().SetPlayerNumber(playerNumber);
            obj.GetComponent<HammerUlt>().SetPlayerNumber(targetNumber);
            obj.GetComponent<HammerUlt>().SetDamage(ultDamage);
        }
    }
}
