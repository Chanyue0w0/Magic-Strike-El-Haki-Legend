using UnityEngine;

public class VibrationPattern : MonoBehaviour
{
    
    public static VibrationPattern Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 呼叫震動模式方法
    public void StartVibrationPattern()
    {
        VibratePattern();
    }

    // 設定震動模式
    public void VibratePattern()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("震動模式開始");

        using (AndroidJavaClass vibrationClass = new AndroidJavaClass("android.os.VibrationEffect"))
        {
            using (AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (context == null)
                {
                    Debug.LogError("無法獲取上下文");
                    return;
                }

                // 定義震動模式 [震動時間, 停止時間, 震動時間, 停止時間]
                long[] pattern = new long[] { 0, 50}; // [震動, 停止, 震動, 停止, 震動]
                Debug.Log("震動模式定義完成");

                // 使用 VibrateEffect 來創建震動模式
                AndroidJavaObject vibrationEffect = vibrationClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, -1); // -1 表示只震動一次模式
                Debug.Log("震動效果創建完成");

                // 獲取震動服務並啟動震動
                using (AndroidJavaObject vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                {
                    if (vibrator != null)
                    {
                        vibrator.Call("vibrate", vibrationEffect);
                        Debug.Log("震動開始");
                    }
                    else
                    {
                        Debug.LogError("震動服務未啟動");
                    }
                }
            }
        }
#else
        Debug.LogWarning("這段代碼只能在 Android 設備上運行！");
#endif
    }

   

    public void TestVibration()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("震動測試中...");
            Handheld.Vibrate();
    #endif
    }
}
