using UnityEngine;

public class RPPanelJuice : MonoBehaviour
{
    public float pulseAmount = 0.012f;
    public float pulseSpeed = 1.7f;
    private Vector3 startScale;

    private void OnEnable(){ startScale = transform.localScale; }
    private void Update(){ float scale = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount; transform.localScale = startScale * scale; }
}
