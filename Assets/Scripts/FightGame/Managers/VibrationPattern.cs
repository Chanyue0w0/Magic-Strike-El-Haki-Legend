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

    // �I�s�_�ʼҦ���k
    public void StartVibrationPattern()
    {
        VibratePattern();
    }

    // �]�w�_�ʼҦ�
    public void VibratePattern()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("�_�ʼҦ��}�l");

        using (AndroidJavaClass vibrationClass = new AndroidJavaClass("android.os.VibrationEffect"))
        {
            using (AndroidJavaClass contextClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject context = contextClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (context == null)
                {
                    Debug.LogError("�L�k����W�U��");
                    return;
                }

                // �w�q�_�ʼҦ� [�_�ʮɶ�, ����ɶ�, �_�ʮɶ�, ����ɶ�]
                long[] pattern = new long[] { 0, 50}; // [�_��, ����, �_��, ����, �_��]
                Debug.Log("�_�ʼҦ��w�q����");

                // �ϥ� VibrateEffect �ӳЫؾ_�ʼҦ�
                AndroidJavaObject vibrationEffect = vibrationClass.CallStatic<AndroidJavaObject>("createWaveform", pattern, -1); // -1 ��ܥu�_�ʤ@���Ҧ�
                Debug.Log("�_�ʮĪG�Ыا���");

                // ����_�ʪA�ȨñҰʾ_��
                using (AndroidJavaObject vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                {
                    if (vibrator != null)
                    {
                        vibrator.Call("vibrate", vibrationEffect);
                        Debug.Log("�_�ʶ}�l");
                    }
                    else
                    {
                        Debug.LogError("�_�ʪA�ȥ��Ұ�");
                    }
                }
            }
        }
#else
        Debug.LogWarning("�o�q�N�X�u��b Android �]�ƤW�B��I");
#endif
    }

   

    public void TestVibration()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("�_�ʴ��դ�...");
            Handheld.Vibrate();
    #endif
    }
}
