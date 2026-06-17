using UnityEngine;
using UnityEngine.UI;

public enum RPBallArchetype
{
    Classic = 0,
    Gold = 1,
    Heavy = 2,
    Rubber = 3,
    Glass = 4
}

public class RPBallStatsManager : MonoBehaviour
{
    public RPGameManager gameManager;
    public RPProgressionManager progression;
    public Text statsHudText;

    [Header("Current Stats")]
    public float powerMultiplier = 1.0f;
    public float bounceMultiplier = 1.0f;
    public float controlMultiplier = 1.0f;
    public int coinBonusPercent = 0;
    public string abilityName = "Balanced";
    public string abilityDescription = "Balanced power, bounce and control.";

    private PhysicsMaterial runtimeBallPhysics;

    private void Start()
    {
        ResolveReferences();
        ApplySelectedBallStats();
    }

    private void Update()
    {
        ResolveReferences();

        if (gameManager != null && gameManager.ball != null)
        {
            ApplySelectedBallStats();
        }
    }

    public void ApplySelectedBallStats()
    {
        ResolveReferences();

        int selected = progression != null ? progression.selectedSkin : 0;
        RPBallArchetype type = MapSkinToArchetype(selected);

        switch (type)
        {
            case RPBallArchetype.Heavy:
                powerMultiplier = 0.86f;
                bounceMultiplier = 0.72f;
                controlMultiplier = 1.18f;
                coinBonusPercent = 0;
                abilityName = "Heavy Control";
                abilityDescription = "Slower and safer. Less bounce, more control.";
                break;

            case RPBallArchetype.Rubber:
                powerMultiplier = 1.03f;
                bounceMultiplier = 1.28f;
                controlMultiplier = 0.88f;
                coinBonusPercent = 0;
                abilityName = "Rubber Bounce";
                abilityDescription = "Huge bounce potential. Risky near pockets.";
                break;

            case RPBallArchetype.Glass:
                powerMultiplier = 1.18f;
                bounceMultiplier = 1.04f;
                controlMultiplier = 0.92f;
                coinBonusPercent = 0;
                abilityName = "Glass Speed";
                abilityDescription = "Fast and sharp. Powerful but risky.";
                break;

            case RPBallArchetype.Gold:
                powerMultiplier = 1.0f;
                bounceMultiplier = 1.0f;
                controlMultiplier = 1.0f;
                coinBonusPercent = 25;
                abilityName = "Gold Bonus";
                abilityDescription = "+25% skill bonus coins. Balanced physics.";
                break;

            default:
                powerMultiplier = 1.0f;
                bounceMultiplier = 1.0f;
                controlMultiplier = 1.0f;
                coinBonusPercent = 0;
                abilityName = "Balanced";
                abilityDescription = "Balanced power, bounce and control.";
                break;
        }

        if (gameManager != null)
        {
            gameManager.minLaunchPower = 3.4f * powerMultiplier;
            gameManager.maxLaunchPower = 10.9f * powerMultiplier;
            gameManager.maxPullDistance = Mathf.Clamp(2.05f / controlMultiplier, 1.72f, 2.45f);
        }

        ApplyBallPhysics();
        UpdateHud();
    }

    public int ApplyCoinBonus(int baseCoins)
    {
        if (coinBonusPercent <= 0)
        {
            return baseCoins;
        }

        return Mathf.CeilToInt(baseCoins * (1f + coinBonusPercent / 100f));
    }

    public string GetShopStatLine(int skinId)
    {
        RPBallArchetype type = MapSkinToArchetype(skinId);

        switch (type)
        {
            case RPBallArchetype.Heavy:
                return "Ability: Heavy Control\nPower 2/5 • Bounce 2/5 • Control 5/5";
            case RPBallArchetype.Rubber:
                return "Ability: Rubber Bounce\nPower 3/5 • Bounce 5/5 • Control 2/5";
            case RPBallArchetype.Glass:
                return "Ability: Glass Speed\nPower 5/5 • Bounce 3/5 • Control 2/5";
            case RPBallArchetype.Gold:
                return "Ability: Gold Bonus\nBalanced stats • +25% skill coins";
            default:
                return "Ability: Balanced\nPower 3/5 • Bounce 3/5 • Control 3/5";
        }
    }

    private RPBallArchetype MapSkinToArchetype(int selectedSkin)
    {
        // Existing shop IDs:
        // 0 Classic, 1 Gold, 2 Black Pearl, 3 Ocean Glass, 4 Ruby Core.
        // We reinterpret them as gameplay archetypes with trade-offs.
        if (selectedSkin == 1) return RPBallArchetype.Gold;
        if (selectedSkin == 2) return RPBallArchetype.Heavy;
        if (selectedSkin == 3) return RPBallArchetype.Rubber;
        if (selectedSkin == 4) return RPBallArchetype.Glass;
        return RPBallArchetype.Classic;
    }

    private void ApplyBallPhysics()
    {
        if (gameManager == null || gameManager.ball == null)
        {
            return;
        }

        Collider collider = gameManager.ball.GetComponent<Collider>();

        if (collider == null)
        {
            return;
        }

        if (runtimeBallPhysics == null)
        {
            runtimeBallPhysics = new PhysicsMaterial("Runtime Marble Ability Physics");
            runtimeBallPhysics.dynamicFriction = 0f;
            runtimeBallPhysics.staticFriction = 0f;
            runtimeBallPhysics.frictionCombine = PhysicsMaterialCombine.Minimum;
            runtimeBallPhysics.bounceCombine = PhysicsMaterialCombine.Maximum;
        }

        runtimeBallPhysics.bounciness = Mathf.Clamp(bounceMultiplier, 0.55f, 1.45f);
        collider.material = runtimeBallPhysics;
    }

    private void UpdateHud()
    {
        if (statsHudText != null)
        {
            statsHudText.text = abilityName;
        }
    }

    private void ResolveReferences()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<RPGameManager>();
        }

        if (progression == null)
        {
            progression = FindFirstObjectByType<RPProgressionManager>();
        }
    }
}