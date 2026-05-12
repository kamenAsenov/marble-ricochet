using UnityEngine;

public class RPMobileRuntimeSettings : MonoBehaviour
{
    [Header("Mobile Runtime")]
    public int targetFrameRate = 60;
    public bool neverSleep = true;

    private void Awake()
    {
        Apply();
    }

    private void Start()
    {
        Apply();
    }

    public void Apply()
    {
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;

        if (neverSleep)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}