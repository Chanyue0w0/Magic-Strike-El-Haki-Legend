using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShakingManager : MonoBehaviour
{
    public static UIShakingManager Instance { get; private set; }

    [Header("Player 1 UI")]
    [SerializeField] private RectTransform player1_HeadIcon;
    [SerializeField] private RectTransform player1_HealthBar;

    [Header("Player 2 UI")]
    [SerializeField] private RectTransform player2_HeadIcon;
    [SerializeField] private RectTransform player2_HealthBar;

    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 5f;

    // 每個 UI 對應初始位置
    private Dictionary<RectTransform, Vector2> originalPositions = new Dictionary<RectTransform, Vector2>();
    private Dictionary<RectTransform, Coroutine> shakingCoroutines = new Dictionary<RectTransform, Coroutine>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 儲存原始位置
        CacheOriginalPosition(player1_HeadIcon);
        CacheOriginalPosition(player1_HealthBar);
        CacheOriginalPosition(player2_HeadIcon);
        CacheOriginalPosition(player2_HealthBar);
    }

    private void CacheOriginalPosition(RectTransform uiElement)
    {
        if (uiElement != null)
        {
            originalPositions[uiElement] = uiElement.anchoredPosition;
        }
    }

    public void ShakePlayerUI(int playerNumber)
    {
        if (playerNumber == 1)
        {
            StartShake(player1_HeadIcon);
            StartShake(player1_HealthBar);
        }
        else if (playerNumber == 2)
        {
            StartShake(player2_HeadIcon);
            StartShake(player2_HealthBar);
        }
        else
        {
            Debug.LogWarning("無效的玩家編號：" + playerNumber);
        }
    }

    private void StartShake(RectTransform uiElement)
    {
        if (uiElement == null || !originalPositions.ContainsKey(uiElement))
            return;

        if (shakingCoroutines.TryGetValue(uiElement, out Coroutine existing))
        {
            StopCoroutine(existing);
        }

        Coroutine newCoroutine = StartCoroutine(ShakeUI(uiElement, originalPositions[uiElement]));
        shakingCoroutines[uiElement] = newCoroutine;
    }

    private IEnumerator ShakeUI(RectTransform uiElement, Vector2 originalPos)
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            uiElement.anchoredPosition = originalPos + new Vector2(x, y);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        uiElement.anchoredPosition = originalPos;
        shakingCoroutines.Remove(uiElement);
    }
}
