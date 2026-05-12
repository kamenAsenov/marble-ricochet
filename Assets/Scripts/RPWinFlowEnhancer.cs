using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPWinFlowEnhancer : MonoBehaviour
{
    public RPGameManager gameManager;
    public GameObject winPanel;
    public Text nextPreviewText;

    private List<RPLevelData> levels;
    private bool wasVisible;

    private void Start()
    {
        levels = RPLevelLibrary.CreateLevels();
    }

    private void Update()
    {
        if (gameManager == null || winPanel == null || nextPreviewText == null)
        {
            return;
        }

        bool visible = winPanel.activeSelf;

        if (visible && !wasVisible)
        {
            RefreshText();
        }

        wasVisible = visible;
    }

    public void RefreshText()
    {
        if (levels == null || levels.Count == 0)
        {
            levels = RPLevelLibrary.CreateLevels();
        }

        int nextIndex = Mathf.Clamp(gameManager.currentLevelIndex + 1, 0, levels.Count - 1);

        if (nextIndex == gameManager.currentLevelIndex)
        {
            nextPreviewText.text = "End of pack";
            return;
        }

        RPLevelData nextLevel = levels[nextIndex];
        nextPreviewText.text = "Next: " + (nextIndex + 1) + " — " + nextLevel.title;
    }
}