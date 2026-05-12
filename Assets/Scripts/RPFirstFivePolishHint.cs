using UnityEngine;

public class RPFirstFivePolishHint : MonoBehaviour
{
    public RPGameManager gameManager;
    private void Update()
    {
        if(gameManager == null || gameManager.hintText == null) return;
        if(gameManager.state != RPGameState.Aiming) return;
        if(gameManager.currentLevelIndex == 0) gameManager.hintText.text = "Pull back. Release. Rails bounce.";
        else if(gameManager.currentLevelIndex == 1) gameManager.hintText.text = "Small pull = soft shot.";
        else if(gameManager.currentLevelIndex == 2) gameManager.hintText.text = "Follow the blue ricochet preview.";
        else if(gameManager.currentLevelIndex == 3) gameManager.hintText.text = "Break both glasses for 3 stars.";
        else if(gameManager.currentLevelIndex == 4) gameManager.hintText.text = "Plan the cleanest route.";
    }
}