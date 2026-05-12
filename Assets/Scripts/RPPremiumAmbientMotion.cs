using UnityEngine;

public class RPPremiumAmbientMotion : MonoBehaviour
{
    public float pulseAmount = 0.015f;
    public float pulseSpeed = 1.15f;
    public bool rotateSlowly = false;
    public float rotationSpeed = 8f;
    private Vector3 startScale;
    private void Start(){ startScale = transform.localScale; }
    private void Update(){ float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount; transform.localScale = startScale * pulse; if(rotateSlowly){ transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World); } }
}