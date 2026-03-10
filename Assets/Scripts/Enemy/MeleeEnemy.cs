using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [Header("Attack parameters")]
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float range;
    [SerializeField] protected int damage;

    [Header("Collider parameters")]
    [SerializeField] protected float colliderDistance;
    [SerializeField] protected BoxCollider2D boxCollider;

    [Header("Player layer")]
    [SerializeField] protected LayerMask playerLayer;
    protected float cooldownTimer = Mathf.Infinity;

    [Header("Attack sound")]
    [SerializeField] private AudioClip attackSound;

    [Header("Enemy follow Player")]
    [SerializeField]public float speed = 3f;
    private Transform player;

    protected Animator anim;
    protected Health playerHealth;
    protected EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        bool canAttack = PlayerInSight();

        if (canAttack)
        {
            anim.SetBool("Moving", false);
            if (cooldownTimer >= attackCooldown && playerHealth != null && playerHealth.currentHealth > 0)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
                SoundManager.instance.PlaySound(attackSound);
            }
        }
        else
        {
            if (player != null)
            {
                anim.SetBool("Moving", true);

                Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);


                if (player.position.x < transform.position.x)
                    transform.localScale = new Vector3(-1, 1, 1); // LEFT
                else
                    transform.localScale = new Vector3(1, 1, 1);  // RIGHT
            }
        }

        if(enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
              0, Vector2.left, 0, playerLayer);

        if(hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void OnDisable()
    {
        anim.SetBool("Moving", false);
    }
}
