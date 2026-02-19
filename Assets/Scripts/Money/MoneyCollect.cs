using UnityEngine;

public class MoneyCollect : MonoBehaviour
{
    [SerializeField] private int moneyValue;

    private void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EconomyManager.instance.AddMoney(moneyValue);
            Destroy(gameObject);
        }
    }

}
