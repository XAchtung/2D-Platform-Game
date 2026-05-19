using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;

    [SerializeField]private LevelUpManager levelUpPanel;

    public void AddExperience(int amount)
    {
        currentXP += amount;
        Debug.Log("XP: " + currentXP + " / " + xpToNextLevel);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;

        // Formu³a na zwiêkszenie trudnoœci levelowania:
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f + 5);

        Debug.Log("LEVEL UP! Obecny poziom: " + currentLevel);

        levelUpPanel.OpenPanel();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
