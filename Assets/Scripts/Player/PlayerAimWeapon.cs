using UnityEngine;

public class PlayerAimWeapon : MonoBehaviour
{
    [Header("Aim & Weapon References")]
    [SerializeField] private Transform gunHolder;   // Pivot broni
    [SerializeField] private Transform firePoint;   // Punkt wylotu pocisku
    [SerializeField] private SpriteRenderer gunSprite; // Sprite samej broni
    [SerializeField] private Transform playerBody;  // Gwny obiekt gracza (do obracania ciaa)

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float damage = 25f;
    [SerializeField] private AudioClip shootSound;

    private float damageBonus;

    [Header("Cursor Settings")]
    [SerializeField] private Texture2D crosshairTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    private float fireTimer;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        // Jeli pole playerBody jest puste, pobierz najwyszego rodzica
        if (playerBody == null)
            playerBody = transform.root;
    }

    private void Start()
    {
        SetCrosshair();
    }

    private void Update()
    {
        // 1. Obracanie gracza i celowanie broni
        AimAndFlip();

        fireTimer += Time.deltaTime;

        // 2. Strza pod lewym przyciskiem myszy (LPM)
        if (Input.GetMouseButton(0) && fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    public void SetCrosshair()
    {
        if (crosshairTexture != null)
        {
            Vector2 actualHotSpot = hotSpot == Vector2.zero
                ? new Vector2(crosshairTexture.width / 2f, crosshairTexture.height / 2f)
                : hotSpot;

            Cursor.SetCursor(crosshairTexture, actualHotSpot, cursorMode);
        }
    }

    private void AimAndFlip()
    {
        if (mainCam == null) return;

        // Pobieramy pozycj kursora w wiecie gry 2D
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreen);
        mouseWorldPos.z = 0f;

        // 1. OBRACANIE CAEGO GRACZA
        if (playerBody != null)
        {
            Vector3 playerScale = playerBody.localScale;
            float baseScaleX = Mathf.Abs(playerScale.x);

            if (mouseWorldPos.x > playerBody.position.x)
            {
                playerBody.localScale = new Vector3(baseScaleX, playerScale.y, playerScale.z);
            }
            else if (mouseWorldPos.x < playerBody.position.x)
            {
                playerBody.localScale = new Vector3(-baseScaleX, playerScale.y, playerScale.z);
            }
        }

        // 2. CELOWANIE BRONI
        if (gunHolder != null)
        {
            Vector2 aimDir = (mouseWorldPos - gunHolder.position).normalized;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

            // Korekta dla odwrconego gracza (skala ujemna)
            if (playerBody != null && playerBody.localScale.x < 0)
            {
                gunHolder.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
                if (gunSprite != null) gunSprite.flipY = true;
            }
            else
            {
                gunHolder.rotation = Quaternion.Euler(0f, 0f, angle);
                if (gunSprite != null) gunSprite.flipY = false;
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || mainCam == null) return;

        // 1. Obliczamy pozycj kursora myszy w wiecie gry
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(mainCam.transform.position.z);
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreen);
        mouseWorldPos.z = 0f;

        // 2. Liczymy bezporedni, czysty kierunek od lufy (firePoint) do kursora myszy
        Vector2 shootDirection = ((Vector2)mouseWorldPos - (Vector2)firePoint.position).normalized;

        // 3. Tworzymy pocisk
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 4. Przekazujemy wyliczony wektor wprost do pocisku
        Projectile projectileScript = bullet.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.damage = damage;
            projectileScript.SetDirection(shootDirection);
        }

        if (SoundManager.instance != null && shootSound != null)
            SoundManager.instance.PlaySound(shootSound);
    }

    public void EquipWeapon(GameObject newBulletPrefab, Sprite newGunSprite = null, float newDamage = -1f, float newFireRate = -1f, AudioClip newShootSound = null)
    {
        if (newBulletPrefab != null)
            bulletPrefab = newBulletPrefab;

        if (newGunSprite != null && gunSprite != null)
            gunSprite.sprite = newGunSprite;

        if (newDamage > 0f)
            damage = newDamage + damageBonus;

        if (newFireRate > 0f)
            fireRate = newFireRate;

        if (newShootSound != null)
            shootSound = newShootSound;

        fireTimer = fireRate;
    }

    public void AddDamage(float amount)
    {
        damageBonus += amount;
        damage += amount;
    }

    private void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}