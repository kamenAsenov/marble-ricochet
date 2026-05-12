using UnityEngine;

public class RPRailEducationHint : MonoBehaviour
{
    public RPGameManager gameManager;

    private void Update()
    {
        if (gameManager == null || gameManager.hintText == null)
        {
            return;
        }

        if (gameManager.state != RPGameState.Aiming)
        {
            return;
        }

        if (gameManager.currentLevelIndex == 0)
        {
            gameManager.hintText.text = "Pull back. Rails bounce the marble.";
        }
        else if (gameManager.currentLevelIndex == 1)
        {
            gameManager.hintText.text = "Use the side rails for angled shots.";
        }
        else if (gameManager.currentLevelIndex == 2)
        {
            gameManager.hintText.text = "The blue preview shows rail bounces.";
        }
    }
}