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

        nameText.text = data.upgradeName;
        descriptionText.text = data.description;
    }

    public void OnButtonClick()
    {
        manager.ApplyUpgrade(assignedUpgrade);
    }
}