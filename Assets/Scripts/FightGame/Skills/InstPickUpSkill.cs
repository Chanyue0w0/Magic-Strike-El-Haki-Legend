using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstPickUpSkill : MonoBehaviour
{
    [SerializeField] private int playerNumber = 1;

    [SerializeField] private int cardSkillSetNumber = 0;

    [SerializeField] private float destroyOriginTime = 1.5f;
    [SerializeField] private float destroyTime = 1.5f;

    [SerializeField] private string cardCode;

    [SerializeField] private GameObject pickUpSkill;//Card 1 撿取物頭像

    //[SerializeField] private SpriteRenderer pickUpSkillSprite;//Card 1 撿取物頭像

    // Start is called before the first frame update
    void Start()
    {
        destroyTime = destroyOriginTime;
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (playerNumber == 1)
            pickUpSkill = Resources.Load<GameObject>("Prefabs/PickUpSkill/PickUpSkill(G)");
        else
            pickUpSkill = Resources.Load<GameObject>("Prefabs/PickUpSkill/PickUpSkill(R)");

    }

    // Update is called once per frame
    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0)
        {
            if (playerNumber == 1)
            {
                GameObject obj = Instantiate(pickUpSkill, gameObject.transform.position, Quaternion.identity);
                obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arts/FightScene/PickUpHeadStickers/" + cardCode);
                obj.GetComponent<PickUpSkill>().SetPlayerNumber(playerNumber);
                obj.GetComponent<PickUpSkill>().SetCardSkillSetNumber(cardSkillSetNumber);
            }
            else
            {
                GameObject obj = Instantiate(pickUpSkill, gameObject.transform.position, Quaternion.Euler(0, 0, 180));
                obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Arts/FightScene/PickUpHeadStickers/" + cardCode);
                obj.GetComponent<PickUpSkill>().SetPlayerNumber(playerNumber);
                obj.GetComponent<PickUpSkill>().SetCardSkillSetNumber(cardSkillSetNumber);
            }
            Destroy(gameObject);
        }

    }

    public void SetCardCode(string cCode)
    {
        cardCode = cCode;
        //pickUpSkillSprite.sprite = Resources.Load<Sprite>("Arts/PickUpHeadStickers/" + cardCode);
    }

    public void SetPlayerNumber(int pNumber)
    {
        playerNumber = pNumber;
    }

    public void SetCardSkillSetNumber(int cssNumber)//對應CardSkillSet哪張卡
    {
        cardSkillSetNumber = cssNumber;
    }
}
