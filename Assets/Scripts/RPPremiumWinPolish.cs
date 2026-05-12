using UnityEngine;
using UnityEngine.UI;

public class RPPremiumWinPolish : MonoBehaviour
{
    public RPGameManager gameManager;
    public GameObject winPanel;
    public Text rewardText;
    public Text titleText;
    public float pulseSpeed = 2.2f;
    public float pulseAmount = 0.035f;
    private Vector3 rewardStartScale = Vector3.one;
    private Vector3 titleStartScale = Vector3.one;
    private void Start(){ if(rewardText!=null) rewardStartScale = rewardText.transform.localScale; if(titleText!=null) titleStartScale = titleText.transform.localScale; }
    private void Update(){ if(winPanel==null || !winPanel.activeSelf) return; float pulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount; if(rewardText!=null) rewardText.transform.localScale = rewardStartScale * pulse; if(titleText!=null) titleText.transform.localScale = titleStartScale * (1f + (pulse - 1f) * 0.45f); }
}