using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "LevelUp System/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    [TextArea] public string description;

    public enum UpgradeType { Health, Attack, Speed }
    public UpgradeType type;

    public float modifierValue;
}