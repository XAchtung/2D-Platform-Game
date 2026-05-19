using UnityEngine;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private GameObject levelUpPanel;

    [Header("Upgrade Pool")]
    [SerializeField] private List<UpgradeData> allUpgrades;

    [Header("UI Buttons")]
    [SerializeField] private UpgradeButton[] upgradeButtons;

    [Header("Player Scripts References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerMovement playerMovement;

    public void OpenPanel()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        DrawUpgrades();
    }

    private void DrawUpgrades()
    {
        List<UpgradeData> availableOptions = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (availableOptions.Count == 0) break;

            int randomIndex = Random.Range(0, availableOptions.Count);
            UpgradeData selectedUpgrade = availableOptions[randomIndex];

            upgradeButtons[i].SetupButton(selectedUpgrade, this);

            availableOptions.RemoveAt(randomIndex);
        }
    }
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeData.UpgradeType.Health:
                playerHealth.AddHealth(upgrade.modifierValue);
                break;
            case UpgradeData.UpgradeType.Attack:
                playerAttack.AddDamage(upgrade.modifierValue);
                break;
            case UpgradeData.UpgradeType.Speed:
                playerMovement.AddSpeed(upgrade.modifierValue);
                break;
        }

        ClosePanel();
    }

    private void ClosePanel()
    {
        Time.timeScale = 1f;
        levelUpPanel.SetActive(false);
    }
}