using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;

    [SerializeField] private float speed;
    private bool hit;
    private Vector2 moveDirection; // 1. Zamiast float direction mamy wektor 2D
    private float lifetime;

    private BoxCollider2D boxcollider;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;

        // 2. Ruch w kierunku wyznaczonym przez wektor (dzia³a w 360 stopniach)
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);

        lifetime += Time.deltaTime;
        if (lifetime > 5) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorujemy gracza i inne pociski
        if (collision.CompareTag("Player") || collision.CompareTag("Projectile")) return;

        // Jeœli trafiliœmy wroga – zadaj obra¿enia
        if (collision.CompareTag("Enemy"))
        {
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        // Natychmiast niszczymy kulê po trafieniu (we wroga LUB w œcianê/przeszkodê)
        Destroy(gameObject);
    }

    // 3. Nowa metoda przyjmuj¹ca kierunek wprost do kursora myszy
    public void SetDirection(Vector2 _direction)
    {
        lifetime = 0;
        moveDirection = _direction.normalized; // Znormalizowany wektor d³ugoœci 1
        gameObject.SetActive(true);
        hit = false;
        boxcollider.enabled = true;

        // Obracamy pocisk w locie, ¿eby lecia³ "czubkiem" do celu
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Deactivate()
    {
        Destroy(gameObject);
    }
}