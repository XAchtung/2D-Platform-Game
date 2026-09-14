using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float currentDamage = 1f;
    [SerializeField] private float attackCooldown = 0.25f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioClip tearsSound;
    [SerializeField] private GameObject bulletPrefab; // Twój prefab pocisku

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
            Attack();

        cooldownTimer += Time.deltaTime;
    }

    public void AddDamage(float amount)
    {
        currentDamage += amount;
        Debug.Log("Zwiêkszono obra¿enia do: " + currentDamage);
    }

    private void Attack()
    {
        if (bulletPrefab == null || firePoint == null) return;

        cooldownTimer = 0;

        if (anim != null)
            anim.SetTrigger("Attack");

        if (SoundManager.instance != null && tearsSound != null)
            SoundManager.instance.PlaySound(tearsSound);

        // 1. Obliczamy pozycjê kursora w œwiecie 2D
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorldPos.z = 0f;

        // 2. Liczymy wektor kierunku od lufy do myszki
        Vector2 shootDirection = ((Vector2)mouseWorldPos - (Vector2)firePoint.position).normalized;

        // 3. Tworzymy nowy pocisk za pomoc¹ Instantiate
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 4. Przekazujemy obra¿enia oraz kierunek lotu
        Projectile projectileScript = newBullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.damage = currentDamage;
            projectileScript.SetDirection(shootDirection);
        }
    }
}