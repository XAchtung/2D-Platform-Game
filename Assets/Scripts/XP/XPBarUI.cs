using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [SerializeField]private Image fillImage;
    [SerializeField]private TextMeshProUGUI levelText;
    [SerializeField]private PlayerStats playerStats;


    void Update()
    {
        UpdateXPBar();
    }

    void UpdateXPBar()
    {
        if (playerStats == null || fillImage == null || playerStats.xpToNextLevel <= 0)
            return;

        float fillAmount = (float)playerStats.currentXP / playerStats.xpToNextLevel;
        fillImage.fillAmount = fillAmount;

        if (levelText != null)
        {
            levelText.text = "Level: " + playerStats.currentLevel;
        }
    }
}