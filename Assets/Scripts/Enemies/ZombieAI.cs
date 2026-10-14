using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ZombieAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float hopForce = 6f;
    [SerializeField] private float wallCheckDistance = 0.25f;
    [SerializeField] private float groundCheckDistance = 0.12f;

    [Header("Melee")]
    [SerializeField] private float attackRange = 1.15f;
    [SerializeField] private float attackCooldown = 1.25f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private AudioClip attackSound;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private Transform player;
    private Health playerHealth;
    private Health ownHealth;
    private Animator anim;
    private float attackTimer;
    private float baseScaleX = 1f;
    private bool deathNotified;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        ownHealth = GetComponent<Health>();
        anim = GetComponent<Animator>();
        baseScaleX = Mathf.Abs(transform.localScale.x);

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    private void Start()
    {
        CachePlayer();
    }

    public void SetupZombie(float maxHealth, float newMoveSpeed, float newAttackDamage)
    {
        moveSpeed = Mathf.Max(0.1f, newMoveSpeed);
        attackDamage = Mathf.Max(0f, newAttackDamage);

        if (ownHealth == null)
            ownHealth = GetComponent<Health>();

        if (ownHealth != null)
            ownHealth.SetMaxHealth(maxHealth);
    }

    public void NotifyKilled()
    {
        if (deathNotified)
            return;

        deathNotified = true;

        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (WaveManager.instance != null)
            WaveManager.instance.EnemyKilled();
    }

    private void Update()
    {
        if (ownHealth != null && ownHealth.IsDead)
            return;

        attackTimer += Time.deltaTime;

        if (player == null)
        {
            CachePlayer();
            return;
        }

        FacePlayer();

        if (CanMeleePlayer())
            TryMeleeAttack();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (ownHealth != null && ownHealth.IsDead)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (player == null || (playerHealth != null && playerHealth.IsDead))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (CanMeleePlayer())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (anim != null)
                anim.SetBool("Moving", false);
            return;
        }

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (Mathf.Abs(player.position.x - transform.position.x) < 0.05f)
            dir = 0f;

        float targetX = dir * moveSpeed;

        if (dir != 0f && IsGrounded() && IsBlockedAhead(dir))
            rb.linearVelocity = new Vector2(targetX, hopForce);
        else
            rb.linearVelocity = new Vector2(targetX, rb.linearVelocity.y);

        if (anim != null)
            anim.SetBool("Moving", Mathf.Abs(targetX) > 0.01f);
    }

    private void FacePlayer()
    {
        if (player == null)
            return;

        float dir = player.position.x >= transform.position.x ? 1f : -1f;
        Vector3 scale = transform.localScale;
        scale.x = baseScaleX * dir;
        transform.localScale = scale;
    }

    private bool CanMeleePlayer()
    {
        if (player == null)
            return false;

        if (playerHealth != null && (playerHealth.IsDead || playerHealth.currentHealth <= 0f))
            return false;

        return Vector2.Distance(transform.position, player.position) <= attackRange;
    }

    private void TryMeleeAttack()
    {
        if (attackTimer < attackCooldown)
            return;

        attackTimer = 0f;

        if (anim != null)
            anim.SetTrigger("meleeAttack");

        if (SoundManager.instance != null && attackSound != null)
            SoundManager.instance.PlaySound(attackSound);

        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
    }

    private bool IsGrounded()
    {
        if (bodyCollider == null)
            return false;

        Bounds bounds = bodyCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    private bool IsBlockedAhead(float dir)
    {
        if (bodyCollider == null)
            return false;

        Bounds bounds = bodyCollider.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + bounds.size.y * 0.35f);
        RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(dir, 0f), wallCheckDistance, obstacleLayer);
        return hit.collider != null;
    }

    private void CachePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null)
            return;

        player = playerObj.transform;
        playerHealth = playerObj.GetComponent<Health>();
        if (playerHealth == null)
            playerHealth = playerObj.GetComponentInParent<Health>();
    }

    private void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (anim != null)
            anim.SetBool("Moving", false);
    }

    private void OnDestroy()
    {
        NotifyKilled();
    }
}
