using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RogueLikePanelManager : MonoBehaviour
{
    public static RogueLikePanelManager Instance { get; private set; }

    [Header("面板與生成點設定")]
    [SerializeField] private GameObject rogueLikePanel;
    [SerializeField] private Transform[] skillButtonPositions = new Transform[3];
    [SerializeField] private Transform[] ownedSkillIconPositions = new Transform[4];

    private Dictionary<string, SkillData> allSkillData = new Dictionary<string, SkillData>();
    private Dictionary<string, GameObject> skillTemplatePrefabs = new Dictionary<string, GameObject>();

    private readonly Vector3[] buttonLocalPositions = new Vector3[]
    {
        new Vector3(-231.41f, -104f, 0f),
        new Vector3(0f, -104f, 0f),
        new Vector3(231.41f, -104f, 0f)
    };

    //[SerializeField] private int nowRound = 1;
    private HashSet<string> ownedActiveSkills = new HashSet<string>();
    private HashSet<string> ownedPassiveSkills = new HashSet<string>();

    private string[] drawnSkillIDs = new string[3];

    [SerializeField] private GameObject ReDrawClickEffect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        LoadTemplatePrefabs();
        InitSkillData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetPanelActive(true);
            DrawSkills(FightPlayer1Config.CurrentStage);
        }
    }

    private void LoadTemplatePrefabs()
    {
        skillTemplatePrefabs["SK"] = Resources.Load<GameObject>("Prefabs/RogueLikePanel/template/ActiveSkillButton");
        skillTemplatePrefabs["PS"] = Resources.Load<GameObject>("Prefabs/RogueLikePanel/template/PassiveSkillButton");
        skillTemplatePrefabs["IS"] = Resources.Load<GameObject>("Prefabs/RogueLikePanel/template/InstantEffectButton");
    }

    private void InitSkillData()
    {
        allSkillData["SK01"] = new SkillData("火球術", "投擲火球\n造成傷害與燃燒", "SK");
        allSkillData["SK02"] = new SkillData(" 能量護盾", "減少50%承受傷害\n持續8秒", "SK");
        allSkillData["SK03"] = new SkillData(" 分身球", "生成3顆球\n持續5秒", "SK");
        allSkillData["SK04"] = new SkillData(" 閃電風暴", "大範圍閃電攻擊", "SK");
        allSkillData["SK05"] = new SkillData(" 治療術", "恢復少量生命", "SK");

        allSkillData["PS01"] = new SkillData(" 寒冰球", "「進球」\n造成寒冰效果\n緩速30%", "PS");
        allSkillData["PS02"] = new SkillData(" 燃燒球", "「進球」\n造成燃燒效果\n每秒1%傷害", "PS");
        allSkillData["PS03"] = new SkillData(" 寒冰增幅", "「進球」\n對寒冰狀態敵人\n召換冰面爆炸", "PS");
        allSkillData["PS04"] = new SkillData(" 燃燒增幅", "燃燒傷害提升至2倍", "PS");

        allSkillData["IS01"] = new SkillData(" 立即治癒", "恢復30%最大生命", "IS");
        allSkillData["IS02"] = new SkillData(" 魔力補充", "獲得全滿魔力值", "IS");
        allSkillData["IS03"] = new SkillData(" 金幣加倍", "勝利後\n金幣獎勵x1.5倍", "IS");
    }

    public void SetPanelActive(bool isActive)
    {
        rogueLikePanel.SetActive(isActive);
        if (isActive)
        {
            ShowOwnedSkillIcons();
        }
    }

    public void OnSkillButtonClicked(string skillID)
    {
        Debug.Log("技能被點擊: " + skillID);

        if (!allSkillData.ContainsKey(skillID)) return;

        string type = allSkillData[skillID].Type;

        if (type == "SK")
        {
            if (FightPlayer1Config.Group[1] == "SK00")
            {
                FightPlayer1Config.Group[1] = skillID;
            }
            else if (FightPlayer1Config.Group[2] == "SK00")
            {
                FightPlayer1Config.Group[2] = skillID;
            }
        }
        else if (type == "PS")
        {
            if (FightPlayer1Config.PassiveEffectGroup[0] == "PS00")
            {
                FightPlayer1Config.PassiveEffectGroup[0] = skillID;
            }
            else if (FightPlayer1Config.PassiveEffectGroup[1] == "PS00")
            {
                FightPlayer1Config.PassiveEffectGroup[1] = skillID;
            }
        }
        else if (type == "IS")
        {
            if(skillID == "IS01")//恢復最大生命30%
            {
                FightPlayer1Config.NowHP += Mathf.RoundToInt(FightPlayer1Config.StartHP * 0.3f);
            } 
            else if (skillID == "IS02")//補滿魔力值
            {
                FightPlayer1Config.NowMagicPoint = 5;
            }
        }

        //關閉Panel
        SetPanelActive(false);

        //RoundController.Instance.SetTimeScale(1);
        RoundController.Instance.OnRogueLikePanelFinished();
    }

    public void DrawSkills(int round)
    {
        ClearSkillButtons();
        UpdateOwnedSkillSets();

        List<string> candidates = new List<string>();
        bool hasSKSlot = HasEmptySKSlot();
        bool hasPSSlot = HasEmptyPSSlot();

        foreach (var pair in allSkillData)
        {
            if (pair.Value.Type == "SK" && !ownedActiveSkills.Contains(pair.Key))
                candidates.Add(pair.Key);
            else if (pair.Value.Type == "PS" && !ownedPassiveSkills.Contains(pair.Key))
                candidates.Add(pair.Key);
            else if (pair.Value.Type == "IS")
                candidates.Add(pair.Key);
        }

        List<string> filtered = new List<string>();

        switch (round)
        {
            case 1:
                filtered = candidates.FindAll(id => allSkillData[id].Type == "SK");
                break;
            case 2:
                filtered = candidates;
                break;
            case 3:
                filtered = candidates.FindAll(id =>
                    (hasSKSlot || allSkillData[id].Type != "SK")
                );
                break;
            case 4:
                filtered = candidates.FindAll(id =>
                    (!hasSKSlot && !hasPSSlot && allSkillData[id].Type == "IS") ||
                    (!hasSKSlot && allSkillData[id].Type != "SK") ||
                    (!hasPSSlot && allSkillData[id].Type != "PS") ||
                    (hasSKSlot && hasPSSlot)
                );
                break;
        }

        List<string> chosen = GetRandomSubset(filtered, 3);

        for (int i = 0; i < chosen.Count; i++)
        {
            string skillID = chosen[i];
            SkillData data = allSkillData[skillID];
            GameObject template = skillTemplatePrefabs[data.Type];
            GameObject instance = Instantiate(template, skillButtonPositions[i]);
            instance.transform.localPosition = buttonLocalPositions[i];
            instance.transform.localRotation = Quaternion.identity;

            Button button = instance.GetComponent<Button>();
            if (button != null)
            {
                string capturedID = skillID;
                button.onClick.AddListener(() => OnSkillButtonClicked(capturedID));
            }

            Transform title = instance.transform.Find("SkillTitle");
            if (title != null && title.TryGetComponent(out Text titleText))
                titleText.text = data.Title;

            Transform info = instance.transform.Find("SkillInfo");
            if (info != null && info.TryGetComponent(out Text infoText))
                infoText.text = data.Info;

            Transform icon = instance.transform.Find("SkillIcon");

            // 綁定 ReDrawButton 功能
            Transform redrawBtn = instance.transform.Find("ReDrawButton");

            if (redrawBtn != null && redrawBtn.TryGetComponent(out Button redrawButton))
            {
                int capturedIndex = i;
                redrawButton.onClick.AddListener(() => RedrawSingleSkill(capturedIndex));
            }


            if (icon != null && icon.TryGetComponent(out Image iconImage))
            {
                Sprite sprite = Resources.Load<Sprite>($"Arts/FightScene/RogueLikePanelIcons/icon/{skillID}");
                if (sprite != null) iconImage.sprite = sprite;
            }

            drawnSkillIDs[i] = skillID;

        }
    }

    public void RedrawSingleSkill(int index)
    {
        if (index < 0 || index >= 3) return;

        // 找出目前已抽中的技能，排除自己這格
        List<string> currentDrawnSkills = new List<string>();
        for (int i = 0; i < drawnSkillIDs.Length; i++)
        {
            if (i != index && !string.IsNullOrEmpty(drawnSkillIDs[i]))
            {
                currentDrawnSkills.Add(drawnSkillIDs[i]);
            }
        }

        // 額外排除當前那格本身的技能
        if (!string.IsNullOrEmpty(drawnSkillIDs[index]))
        {
            currentDrawnSkills.Add(drawnSkillIDs[index]);
        }


        // 更新可用技能清單
        UpdateOwnedSkillSets();
        List<string> candidates = new List<string>();
        bool hasSKSlot = HasEmptySKSlot();
        bool hasPSSlot = HasEmptyPSSlot();

        foreach (var pair in allSkillData)
        {
            string id = pair.Key;
            string type = pair.Value.Type;

            bool isOwned = (type == "SK" && ownedActiveSkills.Contains(id)) ||
                           (type == "PS" && ownedPassiveSkills.Contains(id));

            bool typeAllowed = (FightPlayer1Config.CurrentStage == 1 && type == "SK") ||
                               (FightPlayer1Config.CurrentStage == 2) ||
                               (FightPlayer1Config.CurrentStage == 3 && (hasSKSlot || type != "SK")) ||
                               (FightPlayer1Config.CurrentStage == 4 && (
                                    (!hasSKSlot && !hasPSSlot && type == "IS") ||
                                    (!hasSKSlot && type != "SK") ||
                                    (!hasPSSlot && type != "PS") ||
                                    (hasSKSlot && hasPSSlot)));

            if (typeAllowed && !isOwned && !currentDrawnSkills.Contains(id))
                candidates.Add(id);
        }

        if (candidates.Count == 0) return;

        string newSkillID = GetRandomSubset(candidates, 1)[0];

        // 清空原先的
        foreach (Transform child in skillButtonPositions[index])
        {
            Destroy(child.gameObject);
        }

        // 生成新按鈕
        SkillData data = allSkillData[newSkillID];
        GameObject template = skillTemplatePrefabs[data.Type];
        GameObject instance = Instantiate(template, skillButtonPositions[index]);
        instance.transform.localPosition = buttonLocalPositions[index];
        instance.transform.localRotation = Quaternion.identity;
        instance.name = newSkillID; // 方便找出已抽技能

        Button button = instance.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnSkillButtonClicked(newSkillID));
        }

        Transform title = instance.transform.Find("SkillTitle");
        if (title != null && title.TryGetComponent(out Text titleText))
            titleText.text = data.Title;

        Transform info = instance.transform.Find("SkillInfo");
        if (info != null && info.TryGetComponent(out Text infoText))
            infoText.text = data.Info;

        Transform icon = instance.transform.Find("SkillIcon");
        if (icon != null && icon.TryGetComponent(out Image iconImage))
        {
            Sprite sprite = Resources.Load<Sprite>($"Arts/FightScene/RogueLikePanelIcons/icon/{newSkillID}");
            if (sprite != null) iconImage.sprite = sprite;
        }

        // 加上 ReDrawButton 的綁定
        Transform redrawBtn = instance.transform.Find("ReDrawButton");
        if (redrawBtn != null && redrawBtn.TryGetComponent(out Button redrawButton))
        {
            int capturedIndex = index;
            redrawButton.onClick.AddListener(() => RedrawSingleSkill(capturedIndex));
        }

        drawnSkillIDs[index] = newSkillID;

    }


    private void UpdateOwnedSkillSets()
    {
        ownedActiveSkills.Clear();
        ownedPassiveSkills.Clear();

        string[] currentSK = {
            FightPlayer1Config.Group[1],
            FightPlayer1Config.Group[2]
        };
        string[] currentPS = {
            FightPlayer1Config.PassiveEffectGroup[0],
            FightPlayer1Config.PassiveEffectGroup[1]
        };

        foreach (var skill in currentSK)
        {
            if (skill != "SK00")
                ownedActiveSkills.Add(skill);
        }
        foreach (var skill in currentPS)
        {
            if (skill != "PS00")
                ownedPassiveSkills.Add(skill);
        }
    }

    private void ShowOwnedSkillIcons()
    {
        ClearOwnedSkillIcons();

        string[] currentSK = {
        FightPlayer1Config.Group[1],
        FightPlayer1Config.Group[2]
    };
        string[] currentPS = {
        FightPlayer1Config.PassiveEffectGroup[0],
        FightPlayer1Config.PassiveEffectGroup[1]
    };

        int skIndex = 0;
        int psIndex = 0;

        foreach (var skill in currentSK)
        {
            if (skill != "SK00" && skIndex < 2)
            {
                CreateIconAt(ownedSkillIconPositions[skIndex], skill);
                skIndex++;
            }
        }

        foreach (var skill in currentPS)
        {
            if (skill != "PS00" && psIndex < 2)
            {
                CreateIconAt(ownedSkillIconPositions[psIndex + 2], skill);
                psIndex++;
            }
        }
    }


    private void CreateIconAt(Transform target, string skillID)
    {
        Sprite sprite = Resources.Load<Sprite>($"Arts/FightScene/RogueLikePanelIcons/icon_with_backGround/{skillID}");
        if (sprite == null) return;

        GameObject iconObj = new GameObject(skillID);
        iconObj.transform.SetParent(target, false);
        Image img = iconObj.AddComponent<Image>();
        img.sprite = sprite;
        img.SetNativeSize();
    }

    private void ClearSkillButtons()
    {
        foreach (var t in skillButtonPositions)
        {
            foreach (Transform child in t)
                Destroy(child.gameObject);
        }
    }

    private void ClearOwnedSkillIcons()
    {
        foreach (var t in ownedSkillIconPositions)
        {
            foreach (Transform child in t)
                Destroy(child.gameObject);
        }
    }

    private bool HasEmptySKSlot()
    {
        return FightPlayer1Config.Group[1] == "SK00" || FightPlayer1Config.Group[2] == "SK00";
    }

    private bool HasEmptyPSSlot()
    {
        return FightPlayer1Config.PassiveEffectGroup[0] == "PS00" || FightPlayer1Config.PassiveEffectGroup[1] == "PS00";
    }

    private List<string> GetRandomSubset(List<string> source, int count)
    {
        List<string> copy = new List<string>(source);
        List<string> result = new List<string>();
        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }
        return result;
    }

    private class SkillData
    {
        public string Title;
        public string Info;
        public string Type;

        public SkillData(string title, string info, string type)
        {
            Title = title;
            Info = info;
            Type = type;
        }
    }
}
