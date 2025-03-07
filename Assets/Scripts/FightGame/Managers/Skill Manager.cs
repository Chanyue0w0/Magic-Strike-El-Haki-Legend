using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }

    //[SerializeField] private float p1InstTime = 10;
    //[SerializeField] private float p1InstOriginTime = 0;

    //[SerializeField] private float p2InstTime = 10;
    //[SerializeField] private float p2InstOriginTime = 0;

    [SerializeField] private float instTime = 10;
    [SerializeField] private float instOriginTime = 10;

    //[SerializeField] private SpriteRenderer player1_card1_sprite;//Card 1 �ߨ����Y��
    //[SerializeField] private SpriteRenderer player1_card2_sprite;//Card 2 �ߨ����Y��

    //[SerializeField] private SpriteRenderer player2_card5_sprite;//Card 5 �ߨ����Y��
    //[SerializeField] private SpriteRenderer player2_card6_sprite;//Card 6 �ߨ����Y��

    [SerializeField] private string player1_card1_cardCode;//Card 1 cardCode
    [SerializeField] private string player1_card2_cardCode;//Card 2 cardCode
    [SerializeField] private string player2_card1_cardCode;//Card 1 cardCode
    [SerializeField] private string player2_card2_cardCode;//Card 2 cardCode

    private Dictionary<(int player, int skillIndex), MonoBehaviour> skillComponents = new();

    [SerializeField] private Vector2 xAxisRange;
    [SerializeField] private Vector2 yAxisRange;

    [SerializeField] private GameObject instPickUpSkill;//Card 6 cardCode
    [SerializeField] private GameObject instPickUpSkillG;//Card 6 cardCode
    [SerializeField] private GameObject instPickUpSkillR;//Card 6 cardCode

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Found more than one SkillData object in the sence");
        }
        Instance = this;
    }

    void Start()
    {
        instTime = instOriginTime - FightPlayer1Config.SkillBubbleTimeDecrease;
        //string fullPath = "Arts/Field/Object/" + player2IMGName;
        //Sprite sprite = Resources.Load<Sprite>(fullPath);
        //player2Sprite.sprite

        player1_card1_cardCode = FightPlayer1Config.Group[1];
        player1_card2_cardCode = FightPlayer1Config.Group[2];

        player2_card1_cardCode = FightPlayer2Config.Group[1];
        player2_card2_cardCode = FightPlayer2Config.Group[2];

        instPickUpSkill = Resources.Load<GameObject>("Prefabs/PickUpSkill/InstPickUpSkill");
        instPickUpSkillG = Resources.Load<GameObject>("Prefabs/PickUpSkill/InstPickUpSkill(G)");
        instPickUpSkillR = Resources.Load<GameObject>("Prefabs/PickUpSkill/InstPickUpSkill(R)");

        xAxisRange = new Vector2(-1.6f, 1.6f);
        yAxisRange = new Vector2(-2.8f, 2.8f);



        //// SkillName P1 Skill 1
        //string skillName1 = SkillData.Instance.GetScriptName(player1_card1_cardCode);
        //System.Type skillScript = System.Type.GetType(skillName1 + ",Assembly-CSharp");
        //gameObject.AddComponent(skillScript);

        //// SkillName P1 Skill 2
        //string skillName2 = SkillData.Instance.GetScriptName(player1_card2_cardCode);
        //System.Type skillScript2 = System.Type.GetType(skillName2 + ",Assembly-CSharp");
        //gameObject.AddComponent(skillScript);
        // �� Player 1 ���U�ޯ�
        AddSkillComponent(1, 1, player1_card1_cardCode);
        AddSkillComponent(1, 2, player1_card2_cardCode);

        // �� Player 2 ���U�ޯ�
        AddSkillComponent(2, 1, player2_card1_cardCode);
        AddSkillComponent(2, 2, player2_card2_cardCode);
    }

    private void AddSkillComponent(int playerNumber, int skillIndex, string cardCode)
    {
        string skillName = SkillData.Instance.GetScriptName(cardCode);
        System.Type skillScriptType = System.Type.GetType(skillName + ",Assembly-CSharp");

        if (skillScriptType != null)
        {
            MonoBehaviour skillComponent = (MonoBehaviour)gameObject.AddComponent(skillScriptType);
            skillComponents[(playerNumber, skillIndex)] = skillComponent;
        }
        else
        {
            Debug.LogError($"�ޯ�}�� {skillName} �䤣��A�нT�{�W�٬O�_���T");
        }
    }

    public void ActiveSkill(int playerNumber, int skillIndex)
    {
        if (skillComponents.TryGetValue((playerNumber, skillIndex), out MonoBehaviour skillComponent))
        {
            var setPlayerNumber = skillComponent.GetType().GetMethod("SetPlayerNumber");
            var method = skillComponent.GetType().GetMethod("Active");
            if (method != null && setPlayerNumber != null)
            {
                setPlayerNumber.Invoke(skillComponent, new object[] { playerNumber });
                method.Invoke(skillComponent, null);
            }
            else if(method == null)
            {
                Debug.LogError($"�ޯ� {skillComponent.GetType().Name} �S�� Active ��k");
            }
            else if(setPlayerNumber == null)
            {
                Debug.LogError($"�ޯ� {skillComponent.GetType().Name} �S�� SetPlayerNumber ��k");
            }
        }
        else
        {
            Debug.LogError($"�䤣�� Player {playerNumber} ���ޯ� {skillIndex}");
        }
    }

    void Update()
    {
        instTime -= Time.deltaTime;
        if (instTime <= 0)
        {
            InstSkill();
            instTime = instOriginTime;
        }

        // �˴������J��Ĳ�o�ޯ�
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActiveSkill(1, 1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ActiveSkill(1, 2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActiveSkill(2, 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ActiveSkill(2, 2);
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
            float randomP1positionX = Random.Range(xAxisRange.x, xAxisRange.y);
            float randomP1positionY = Random.Range(0, yAxisRange.y);
            if (randomP1 <= 5)//P1 left skill & left side (On Top Left Field)
            {
                GameObject obj = Instantiate(instPickUpSkillG, new Vector2(randomP1positionX, randomP1positionY), Quaternion.identity);
                obj.GetComponent<InstPickUpSkill>().SetCardCode(player1_card1_cardCode);
                obj.GetComponent<InstPickUpSkill>().SetPlayerNumber(1);
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(1);
                //Set cardCode & cardSkillSetNumber
            }
            else//P1 right skill & right size  (On Top Right Field)
            {
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
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(1);

                //Set cardCode & cardSkillSetNumber
            }
            else//P2 right skill & right size  (On Button Right Field)
            {
                float randomP2positionX = Random.Range(0, xAxisRange.y);
                float randomP2positionY = Random.Range(yAxisRange.x, 0);
                GameObject obj = Instantiate(instPickUpSkillR, new Vector2(randomP2positionX, randomP2positionY), Quaternion.identity);
                obj.GetComponent<InstPickUpSkill>().SetCardCode(player2_card2_cardCode);
                obj.GetComponent<InstPickUpSkill>().SetPlayerNumber(2);
                obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(2);
                //obj.GetComponent<InstPickUpSkill>().SetCardSkillSetNumber(4);

                //Set cardCode & cardSkillSetNumber
            }
        }
    }
}
