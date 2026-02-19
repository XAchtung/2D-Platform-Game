using UnityEngine;
using UnityEngine.UI;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;

    private int money = 0;
    public Text moneyText;

    private void Awake()
    {
        instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        moneyText.text = "Money: $" + money.ToString();
    }
}
