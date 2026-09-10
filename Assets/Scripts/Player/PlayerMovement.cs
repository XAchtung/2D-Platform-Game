using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 16f;

    [Header("Dash Settings")]
    [SerializeField] private bool canDashUpgrade = false; // Odblokowywane z level upa
    [SerializeField] private float dashPower = 24f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps = 0;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX = 10f;
    [SerializeField] private float wallJumpY = 16f;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;

    [Header("Invincibility Layers")]
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string enemyLayerName = "Enemy";

    [Header("Visual Effects")]
    [SerializeField] private TrailRenderer trailRenderer;

    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float originalGravity;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalGravity = body.gravityScale;

        if (trailRenderer == null)
            trailRenderer = GetComponent<TrailRenderer>();

        // Upewniamy siê, ¿e na starcie smuga jest wy³¹czona
        if (trailRenderer != null)
            trailRenderer.emitting = false;
    }

    private void Update()
    {
        // 1. Jeœli trwa dash - ca³kowicie ignorujemy resztê Update
        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Obrót postaci
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        // Animacje
        if (animator != null)
        {
            animator.SetBool("Run", horizontalInput != 0);
            animator.SetBool("Grounded", isGrounded());
        }

        // 2. Klawisz Dash (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && canDashUpgrade)
        {
            StartCoroutine(PerformDash());
            return; // <-- BARDZO WA¯NE: natychmiast koñczymy Update, ¿eby nie nadpisaæ prêdkoœci dasha!
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        // Skrócony skok po puszczeniu spacji
        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2f);

        if (onWall())
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = originalGravity;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        // 1. W³¹czamy smugê dasha
        if (trailRenderer != null)
            trailRenderer.emitting = true;

        // Warstwy nietykalnoœci
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 0.5f);
        }

        if (SoundManager.instance != null && dashSound != null)
            SoundManager.instance.PlaySound(dashSound);

        body.gravityScale = 0f;

        float dashDirection = transform.localScale.x > 0 ? 1f : -1f;
        body.linearVelocity = new Vector2(dashDirection * dashPower, 0f);

        yield return new WaitForSeconds(dashDuration);

        // 2. Po zakoñczeniu dasha wy³¹czamy smugê i przywracamy fizykê
        if (trailRenderer != null)
            trailRenderer.emitting = false;

        body.gravityScale = originalGravity;
        body.linearVelocity = new Vector2(0f, body.linearVelocity.y);

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        if (sr != null)
        {
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 1f);
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Jump()
    {
        bool canCoyote = coyoteCounter > 0;
        bool hasExtraJump = jumpCounter > 0;

        if (!canCoyote && !onWall() && !hasExtraJump) return;

        if (SoundManager.instance != null && jumpSound != null)
            SoundManager.instance.PlaySound(jumpSound);

        if (onWall())
        {
            WallJump();
            coyoteCounter = 0;
        }
        else if (isGrounded() || canCoyote)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            coyoteCounter = 0;
        }
        else if (hasExtraJump)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
            jumpCounter--;
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
    }

    private bool isGrounded()
    {
        if (body.linearVelocity.y > 0.1f) return false;

        Physics2D.queriesStartInColliders = false;

        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            new Vector2(boxCollider.bounds.size.x * 0.9f, boxCollider.bounds.size.y),
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );

        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            new Vector2(transform.localScale.x, 0),
            0.1f,
            wallLayer
        );

        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall() && !isDashing;
    }

    public void AddSpeed(float amount)
    {
        speed += amount;
    }

    public void EnableDoubleJump()
    {
        extraJumps = 1;
        jumpCounter = extraJumps;
    }

    // Wywo³ywane przez LevelUpManager
    public void EnableDash()
    {
        canDashUpgrade = true;
    }
}