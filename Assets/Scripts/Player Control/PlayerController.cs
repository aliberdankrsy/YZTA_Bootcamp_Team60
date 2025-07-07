using UnityEngine;
using System.Collections; 
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlar�")]
    public float moveSpeed = 4f;
    public float jumpForce = 5f;

    [Header("Dash Ayarlar�")] 
    public float dashSpeed = 15f; 
    public float dashDuration = 0.2f; 
    public float dashCooldown = 2f; // �ki dash aras� bekleme s�resi (saniye)

    [Header("Sald�r� Ayarlar�")] 
    public float attackCooldown = 0.5f; // �ki sald�r� aras� bekleme s�resi (saniye)
    public Animator characterAnimator; 

    [Header("Z�plama Kontrol�")]
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
    void Awake()
    {
        // Rigidbody2D bile�enini al ve rb de�i�kenine ata
        rb = GetComponent<Rigidbody2D>();
        // E�er Rigidbody2D bile�eni bulunamazsa hata mesaj� i�in
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D bile�eni bulunamad�! Karakter hareket edemeyecek. L�tfen karakterinize bir Rigidbody2D ekledi�inizden emin olun.");
        }
        // GroundCheck transform'unun atan�p atanmad���n� kontrol et
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck Transform'u atanmad�! Z�plama kontrol� do�ru �al��mayabilir. L�tfen GroundCheck GameObject'ini Inspector'dan atay�n.");
        }
        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
            if (characterAnimator == null)
            {
                Debug.LogWarning("Animator bile�eni bulunamad�! Sald�r� animasyonlar� �al��mayabilir. L�tfen karakterinize bir Animator ekleyin veya Inspector'dan atay�n.");
            }
        }
        // Oyun ba�lad���nda dash cooldown'� haz�rla
        dashCooldownTimer = dashCooldown;
        // Oyun ba�lad���nda sald�r� cooldown'� haz�rla
        attackCooldownTimer = attackCooldown;
    }
    void FixedUpdate()
    {
        // E�er dash yap�yorsak veya sald�r�yorsak, normal hareket girdisini yok say
        if (isDashing || !canAttack) 
        {
            return; 
        }
        // Zemin kontrol� yap
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f; // Sa�a git
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f; // Sola git
        }
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        // Karakterin y�n�n� �evir
        FlipCharacter(moveInput);
    }
    void Update()
    {
        // Z�plama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing && canAttack) // Sald�r� s�ras�nda z�plamay� engelle
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        // Dash
        if (Input.GetKeyDown(KeyCode.E) && canDash && !isDashing && canAttack) // Sald�r� s�ras�nda dash'i engelle
        {
            StartCoroutine(Dash());
        }
        // Sald�r� (Sol T�k)
        if (Input.GetMouseButtonDown(0) && canAttack && !isDashing) 
        {
            StartCoroutine(Attack());
        }
        // Dash Cooldown Sayac�
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
                dashCooldownTimer = dashCooldown;
            }
        }
        // Sald�r� Cooldown Sayac�
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
        canDash = false; // Dash cooldown'a girdi
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
        yield return new WaitForSeconds(attackCooldown); // Cooldown s�resi kadar bekle
    }
    // Karakterin Sprite'�n� �evirme fonksiyonu
    void FlipCharacter(float moveInput)
    {
        // Dash veya sald�r� s�ras�nda karakterin d�nmesini engelle
        if (!isDashing && canAttack) 
        {
            if (moveInput > 0 && transform.localScale.x < 0)
            {
                Flip();
            }
            else if (moveInput < 0 && transform.localScale.x > 0)
            {
                Flip();
            }
        }
    }
    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1; // X skalas�n� tersine �evir
        transform.localScale = currentScale;
    }
    // DEBUG: GroundCheck'in nerede oldu�unu g�rmek i�in Editor'da k�rm�z� bir daire �izer
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}