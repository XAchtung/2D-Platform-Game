using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "LevelUp System/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;

    public enum UpgradeType { Health, Attack, Speed, DoubleJump, Dash }
    public UpgradeType type;

    public float modifierValue;

    [Header("Pool Settings")]
    public bool isOneTimeUpgrade; // Zaznacz true dla kart takich jak Double Jump
}