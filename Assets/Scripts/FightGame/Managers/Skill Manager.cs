using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //[SerializeField] private float p1InstTime = 10;
    //[SerializeField] private float p1InstOriginTime = 0;

    //[SerializeField] private float p2InstTime = 10;
    //[SerializeField] private float p2InstOriginTime = 0;

    [SerializeField] private float instTime = 10;
    [SerializeField] private float instOriginTime = 10;

    //[SerializeField] private SpriteRenderer player1_card1_sprite;//Card 1 具繷钩
    //[SerializeField] private SpriteRenderer player1_card2_sprite;//Card 2 具繷钩

    //[SerializeField] private SpriteRenderer player2_card5_sprite;//Card 5 具繷钩
    //[SerializeField] private SpriteRenderer player2_card6_sprite;//Card 6 具繷钩

    [SerializeField] private string player1_card1_cardCode;//Card 1 cardCode
    [SerializeField] private string player1_card2_cardCode;//Card 2 cardCode
    [SerializeField] private string player2_card1_cardCode;//Card 1 cardCode
    [SerializeField] private string player2_card2_cardCode;//Card 2 cardCode

    [SerializeField] private Vector2 xAxisRange;
    [SerializeField] private Vector2 yAxisRange;

    [SerializeField] private GameObject instPickUpSkill;//Card 6 cardCode
    [SerializeField] private GameObject instPickUpSkillG;//Card 6 cardCode
    [SerializeField] private GameObject instPickUpSkillR;//Card 6 cardCode



    void Start()
    {
        instTime = instOriginTime - FightPlayer1Config.SkillBubbleTimeDecrease;
        //string fullPath = "Arts/Field/Object/" + player2IMGName;
        //Sprite sprite = Resources.Load<Sprite>(fullPath);
        //player2Sprite.sprite

        //player1_card2_cardCode = GlobalIndex.cardGroup[0];
        player1_card1_cardCode = FightPlayer1Config.Group[1];
        player1_card2_cardCode = FightPlayer1Config.Group[2];

        //player2_card6_cardCode = GlobalIndex.cardGroup[4];
        player2_card1_cardCode = FightPlayer2Config.Group[1];
        player2_card2_cardCode = FightPlayer2Config.Group[2];

        //player1_card1_sprite.sprite = Resources.Load<Sprite>("Arts/PickUpHeadStickers/" + player1_card1_cardCode);
        //player1_card2_sprite.sprite = Resources.Load<Sprite>("Arts/PickUpHeadStickers/" + player1_card1_cardCode);
        //player2_card5_sprite.sprite = Resources.Load<Sprite>("Arts/PickUpHeadStickers/" + player2_card5_cardCode);
        //player2_card6_sprite.sprite = Resources.Load<Sprite>("Arts/PickUpHeadStickers/" + player2_card6_cardCode);

        instPickUpSkill = Resources.Load<GameObject>("Prefabs/PickUpSkill/InstPickUpSkill");
        instPickUpSkillG = Resources.Load<GameObject>("Prefabs/PickUpSkill/InstPickUpSkill(G)");
        instPickUpSkillR = Resources.Load<GameObject>("Prefabs/PickUpSkill/InstPickUpSkill(R)");

        xAxisRange = new Vector2(-1.6f, 1.6f);
        yAxisRange = new Vector2(-2.8f, 2.8f);
    }

    void Update()
    {
        instTime -= Time.deltaTime;
        if (instTime <= 0)
        {
            InstSkill();
            instTime = instOriginTime;
        }
    }

    public void InstSkill()
    {
        float randomP1 = Random.Range(1, 10);
        float randomP2 = Random.Range(1, 10);

        //Debug.Log("r p1: " + randomP1);
        //Debug.Log("r p2: " + randomP2);


        if (FightPlayer1Config.instSkillP1)
        {
            if (randomP1 <= 5)//P1 left skill & left side (On Top Left Field)
            {
                float randomP1positionX = Random.Range(xAxisRange.x, 0);
                float randomP1positionY = Random.Range(0, yAxisRange.y);
                GameObject obj = Instantiate(instPickUpSkillG, new Vector2(randomP1positionX, randomP1positionY), Quaternion.identity);
                obj.GetComponent<InstPickUpSkill>().SetCardCode(player1_card1_cardCode);
                obj.GetComponent<InstPickUpSkill>().SetPlayerNumber(1);
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(1);
                //Set cardCode & cardSkillSetNumber
            }
            else//P1 right skill & right size  (On Top Right Field)
            {
                float randomP1positionX = Random.Range(0, xAxisRange.y);
                float randomP1positionY = Random.Range(0, yAxisRange.y);
                GameObject obj = Instantiate(instPickUpSkillG, new Vector2(randomP1positionX, randomP1positionY), Quaternion.identity);
                obj.GetComponent<InstPickUpSkill>().SetCardCode(player1_card2_cardCode);
                obj.GetComponent<InstPickUpSkill>().SetPlayerNumber(1);
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(2);
                //obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(0);

                //Set cardCode & cardSkillSetNumber
            }
        }

        if (FightPlayer2Config.instSkillP2)
        {
            if (randomP2 <= 5)//P2 left skill & left side (On Button Left Field)
            {
                float randomP2positionX = Random.Range(xAxisRange.x, 0);
                float randomP2positionY = Random.Range(yAxisRange.x, 0);
                GameObject obj = Instantiate(instPickUpSkillR, new Vector2(randomP2positionX, randomP2positionY), Quaternion.identity);
                obj.GetComponent<InstPickUpSkill>().SetCardCode(player2_card1_cardCode);
                obj.GetComponent<InstPickUpSkill>().SetPlayerNumber(2);
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(5);

                //Set cardCode & cardSkillSetNumber
            }
            else//P2 right skill & right size  (On Button Right Field)
            {
                float randomP2positionX = Random.Range(0, xAxisRange.y);
                float randomP2positionY = Random.Range(yAxisRange.x, 0);
                GameObject obj = Instantiate(instPickUpSkillR, new Vector2(randomP2positionX, randomP2positionY), Quaternion.identity);
                obj.GetComponent<InstPickUpSkill>().SetCardCode(player2_card2_cardCode);
                obj.GetComponent<InstPickUpSkill>().SetPlayerNumber(2);
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(6);
                //obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(4);

                //Set cardCode & cardSkillSetNumber
            }
        }
    }
}
