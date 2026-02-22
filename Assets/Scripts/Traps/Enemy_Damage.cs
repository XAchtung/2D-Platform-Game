using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{

    [SerializeField] protected float damage;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<Health>().TakeDamage(damage);
    }
}
