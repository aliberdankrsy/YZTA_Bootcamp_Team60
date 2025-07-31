using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 10f;
    public float jumpForce = 6f;

    [Header("Dash Ayarları")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    [Header("Saldırı Ayarları")]
    public float attackCooldown = 0.5f;
    public Animator characterAnimator;

    [Header("Zıplama Kontrolü")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float groundCheckRadius = 0.2f;

    private bool canDash = true;
    private bool isDashing = false;
    private float dashCooldownTimer;

    private bool canAttack = true;
    private float attackCooldownTimer;

    private Vector3 initialScale; // Karakterin orijinal scale değeri

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D bileşeni bulunamadı!");
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck atanmadı!");
        }

        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
            if (characterAnimator == null)
            {
                Debug.LogWarning("Animator bulunamadı!");
            }
        }

        dashCooldownTimer = dashCooldown;
        attackCooldownTimer = attackCooldown;

        initialScale = transform.localScale; // Scale kaydediliyor
    }

    void FixedUpdate()
    {
        if (isDashing || !canAttack) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;
        else if (Input.GetKey(KeyCode.A)) moveInput = -1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        FlipCharacter(moveInput);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing && canAttack)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.E) && canDash && !isDashing && canAttack)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetMouseButtonDown(0) && canAttack && !isDashing)
        {
            StartCoroutine(Attack());
        }

        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
                dashCooldownTimer = dashCooldown;
            }
        }

        if (!canAttack)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
            {
                canAttack = true;
                attackCooldownTimer = attackCooldown;
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float dashDirection = transform.localScale.x > 0 ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    IEnumerator Attack()
    {
        canAttack = false;
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("Attack");
        }
        yield return new WaitForSeconds(attackCooldown);
    }

    void FlipCharacter(float moveInput)
    {
        if (!isDashing && canAttack)
        {
            if (moveInput > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
            }
            else if (moveInput < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
