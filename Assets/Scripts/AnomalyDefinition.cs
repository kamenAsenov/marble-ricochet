using UnityEngine;

public enum AnomalyDifficulty
{
    Easy,
    Medium,
    Hard
}

public class AnomalyDefinition : MonoBehaviour
{
    public AnomalyDifficulty difficulty = AnomalyDifficulty.Easy;
    public string displayName = "Anomaly";
}