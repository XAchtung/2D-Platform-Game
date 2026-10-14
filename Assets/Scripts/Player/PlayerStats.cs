using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;

    [SerializeField] private LevelUpManager levelUpPanel;
    [SerializeField] private float attackDamagePerLevel = 5f;

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

        // Formuùa na zwiùkszenie trudnoùci levelowania:
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f + 5);

        Debug.Log("LEVEL UP! Obecny poziom: " + currentLevel);

        ApplyLevelUpAttackBonus();

        if (levelUpPanel != null)
            levelUpPanel.OpenPanel();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ApplyLevelUpAttackBonus()
    {
        if (attackDamagePerLevel <= 0f)
            return;

        PlayerAimWeapon aim = GetComponent<PlayerAimWeapon>();
        if (aim == null)
            aim = GetComponentInChildren<PlayerAimWeapon>();
        if (aim != null)
            aim.AddDamage(attackDamagePerLevel);

        PlayerAttack melee = GetComponent<PlayerAttack>();
        if (melee != null)
            melee.AddDamage(attackDamagePerLevel);
    }
}
