using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    public bool IsDead => dead;
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRenderer;

    [Header("Components")]
    [SerializeField] private Behaviour[] components;
    private bool invurnelable;

    [Header("Death sound")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    [Header("XP")]
    public GameObject xpPrefab;

    [Header("Points Reward (if Enemy)")]
    [SerializeField] private int hitPointsReward = 10;
    [SerializeField] private int killPointsReward = 100;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float _damage)
    {
        if (invurnelable || dead) return;

        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        // Punkty za kaùde trafienie wroga w stylu CoD (+10 pkt)
        if (CompareTag("Enemy") && PointsManager.instance != null)
        {
            PointsManager.instance.AddPoints(hitPointsReward);
        }

        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
            if (SoundManager.instance != null && hurtSound != null)
                SoundManager.instance.PlaySound(hurtSound);
        }
        else
        {
            if (!dead)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        dead = true;

        // Wyùùczenie skryptùw ruchu, AI czy atakùw
        foreach (Behaviour component in components)
        {
            if (component != null) component.enabled = false;
        }

        if (anim != null)
        {
            anim.SetBool("Grounded", true);
            anim.SetTrigger("die");
        }

        if (SoundManager.instance != null && deathSound != null)
            SoundManager.instance.PlaySound(deathSound);

        if (xpPrefab != null)
            Instantiate(xpPrefab, transform.position, Quaternion.identity);

        // Logika punktùw i fal dla przeciwnika
        if (CompareTag("Enemy"))
        {
            // Punkty za zabùjstwo (+100 pkt)
            if (PointsManager.instance != null)
                PointsManager.instance.AddPoints(killPointsReward);

            // Odliczenie z fali CoD (WaveManager ma pierwsze?stwo)
            ZombieAI zombie = GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.NotifyKilled();
            }
            else if (WaveManager.instance != null)
            {
                WaveManager.instance.EnemyKilled();
            }
            else if (EnemySpawner.instance != null)
            {
                EnemySpawner.instance.EnemyKilled();
            }
        }
    }

    public void SetMaxHealth(float value)
    {
        startingHealth = Mathf.Max(1f, value);
        if (!dead)
            currentHealth = startingHealth;
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    public void MoreHealth(float _value)
    {
        currentHealth += _value;
    }

    private IEnumerator Invunerability()
    {
        invurnelable = true;
        Physics2D.IgnoreLayerCollision(8, 9, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1, 0, 0, 0.5f);

            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));

            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(8, 9, false);
        invurnelable = false;
    }

    // Wywoùywane przez Animation Event na koùcu animacji ùmierci przeciwnika
    private void Deactivate()
    {
        Destroy(gameObject);
    }

    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        if (anim != null)
        {
            anim.ResetTrigger("die");
            anim.Play("Idle");
        }
        StartCoroutine(Invunerability());

        foreach (Behaviour component in components)
        {
            if (component != null) component.enabled = true;
        }
    }
}