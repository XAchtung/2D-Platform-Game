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
    [SerializeField] private PlayerAimWeapon playerAimWeapon;
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        if (playerAimWeapon == null)
            playerAimWeapon = player.GetComponentInChildren<PlayerAimWeapon>();
        if (playerAttack == null)
            playerAttack = player.GetComponent<PlayerAttack>();
        if (playerHealth == null)
            playerHealth = player.GetComponent<Health>();
        if (playerMovement == null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void OpenPanel()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(true);

        Time.timeScale = 0f;
        DrawUpgrades();
    }

    private void DrawUpgrades()
    {
        if (upgradeButtons == null)
            return;

        List<UpgradeData> availableOptions = allUpgrades != null
            ? new List<UpgradeData>(allUpgrades)
            : new List<UpgradeData>();

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (upgradeButtons[i] == null)
                continue;

            if (availableOptions.Count > 0)
            {
                upgradeButtons[i].gameObject.SetActive(true);

                int randomIndex = Random.Range(0, availableOptions.Count);
                UpgradeData selectedUpgrade = availableOptions[randomIndex];

                upgradeButtons[i].SetupButton(selectedUpgrade, this);
                availableOptions.RemoveAt(randomIndex);
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }
    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null)
            return;

        switch (upgrade.type)
        {
            case UpgradeData.UpgradeType.Health:
                if (playerHealth != null)
                    playerHealth.AddHealth(upgrade.modifierValue);
                break;
            case UpgradeData.UpgradeType.Attack:
                if (playerAttack != null)
                    playerAttack.AddDamage(upgrade.modifierValue);
                if (playerAimWeapon != null)
                    playerAimWeapon.AddDamage(upgrade.modifierValue);
                break;
            case UpgradeData.UpgradeType.Speed:
                if (playerMovement != null)
                    playerMovement.AddSpeed(upgrade.modifierValue);
                break;
            case UpgradeData.UpgradeType.DoubleJump:
                if (playerMovement != null)
                    playerMovement.EnableDoubleJump();
                break;
            case UpgradeData.UpgradeType.Dash:
                if (playerMovement != null)
                    playerMovement.EnableDash();
                break;
        }

        // Jeùli ulepszenie jest jednorazowe, usuwamy je z puli dostùpnych kart
        if (upgrade.isOneTimeUpgrade && allUpgrades != null)
            allUpgrades.Remove(upgrade);

        ClosePanel();
    }

    private void ClosePanel()
    {
        Time.timeScale = 1f;
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }
}