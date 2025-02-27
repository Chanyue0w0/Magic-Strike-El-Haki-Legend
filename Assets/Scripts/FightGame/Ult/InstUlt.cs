using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstUlt : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1; // �ϥΪ��a
    [SerializeField] private int targetNumber = 2; // �ؼЪ��a
    [SerializeField] private string ultName; // �j�ۦW��
    [SerializeField] private int ultDamage = 200; // �j�۶ˮ`
    [SerializeField] private float instTime = 0.7f; // �R���ʵe�S�Įɶ�
    [SerializeField] private Vector2 instPosition = new Vector2(0,2); // �ͦ���m
    [SerializeField] private Quaternion quaternion; // ���ਤ��
    [SerializeField] private GameObject ultGameObject; // �j�ۧޯફ - �z�L�즲���o

    void Start()
    {
        StartCoroutine(InstUltSkill(instTime)); // �̾ګD���v�T�ɶ��R��
    }

    private IEnumerator InstUltSkill(float time)
    {
        yield return new WaitForSecondsRealtime(time); // �ϥ� Unscaled �ɶ��ӭp��
        GameObject obj = Instantiate(ultGameObject, instPosition, quaternion);

        if (ultName == "HammerUlt")
        {
            obj.GetComponent<HammerUlt>().SetPlayerNumber(playerNumber);
            obj.GetComponent<HammerUlt>().SetPlayerNumber(targetNumber);
            obj.GetComponent<HammerUlt>().SetDamage(ultDamage);
        }
    }
}
