using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("Pannel settigns")]
    [SerializeField]private GameObject levelUpPanel;

    [Header("Player Prefs")]
    [SerializeField] private Health health;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerMovement movement;



    public void SelectHealth()
    {
        Debug.Log("Zwiêkszono zdrowie!");
        health.MoreHealth(2);
        ClosePanel();
    }

    public void SelectDamage()
    {
        Debug.Log("Zwiêkszono obra¿enia!");
        attack.AddDamage(2);
        ClosePanel();
    }

    public void SelectSpeed()
    {
        Debug.Log("Zwiêkszono prêdkoœæ!");
        
        ClosePanel();
    }

    private void ClosePanel()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}