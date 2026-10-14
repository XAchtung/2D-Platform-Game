using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private UpgradeData assignedUpgrade;
    private LevelUpManager manager;

    public void SetupButton(UpgradeData data, LevelUpManager lvlManager)
    {
        assignedUpgrade = data;
        manager = lvlManager;

        if (nameText != null)
            nameText.text = data != null ? data.upgradeName : string.Empty;
        if (descriptionText != null)
            descriptionText.text = data != null ? data.description : string.Empty;
    }

    public void OnButtonClick()
    {
        if (manager != null && assignedUpgrade != null)
            manager.ApplyUpgrade(assignedUpgrade);
    }
}