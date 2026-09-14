using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    public static PointsManager instance;

    [Header("Settings")]
    [SerializeField] private int startingPoints = 500; // Klasyczny start w CoD Zombies
    [SerializeField] private TextMeshProUGUI pointsText;

    public int CurrentPoints { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CurrentPoints = startingPoints;
        UpdateUI();
    }

    public void AddPoints(int amount)
    {
        CurrentPoints += amount;
        UpdateUI();
    }

    public bool SpendPoints(int amount)
    {
        if (CurrentPoints >= amount)
        {
            CurrentPoints -= amount;
            UpdateUI();
            return true;
        }

        return false; // Gracz nie ma wystarczaj¹co du¿o punktów
    }

    private void UpdateUI()
    {
        if (pointsText != null)
        {
            pointsText.text = $"{CurrentPoints}";
        }
    }
}