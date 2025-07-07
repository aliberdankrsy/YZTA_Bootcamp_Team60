using UnityEngine;
using System.Collections; 
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 4f;
    public float jumpForce = 5f;

    [Header("Dash Ayarlarý")] 
    public float dashSpeed = 15f; 
    public float dashDuration = 0.2f; 
    public float dashCooldown = 2f; // Ýki dash arasý bekleme süresi (saniye)

    [Header("Saldýrý Ayarlarý")] 
    public float attackCooldown = 0.5f; // Ýki saldýrý arasý bekleme süresi (saniye)
    public Animator characterAnimator; 

    [Header("Zýplama Kontrolü")]
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
        // Rigidbody2D bileþenini al ve rb deðiþkenine ata
        rb = GetComponent<Rigidbody2D>();
        // Eðer Rigidbody2D bileþeni bulunamazsa hata mesajý için
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D bileþeni bulunamadý! Karakter hareket edemeyecek. Lütfen karakterinize bir Rigidbody2D eklediðinizden emin olun.");
        }
        // GroundCheck transform'unun atanýp atanmadýðýný kontrol et
        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck Transform'u atanmadý! Zýplama kontrolü doðru çalýþmayabilir. Lütfen GroundCheck GameObject'ini Inspector'dan atayýn.");
        }
        if (characterAnimator == null)
        {
            characterAnimator = GetComponent<Animator>();
            if (characterAnimator == null)
            {
                Debug.LogWarning("Animator bileþeni bulunamadý! Saldýrý animasyonlarý çalýþmayabilir. Lütfen karakterinize bir Animator ekleyin veya Inspector'dan atayýn.");
            }
        }
        // Oyun baþladýðýnda dash cooldown'ý hazýrla
        dashCooldownTimer = dashCooldown;
        // Oyun baþladýðýnda saldýrý cooldown'ý hazýrla
        attackCooldownTimer = attackCooldown;
    }
    void FixedUpdate()
    {
        // Eðer dash yapýyorsak veya saldýrýyorsak, normal hareket girdisini yok say
        if (isDashing || !canAttack) 
        {
            return; 
        }
        // Zemin kontrolü yap
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D))
        {
            moveInput = 1f; // Saða git
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveInput = -1f; // Sola git
        }
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        // Karakterin yönünü çevir
        FlipCharacter(moveInput);
    }
    void Update()
    {
        // Zýplama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing && canAttack) // Saldýrý sýrasýnda zýplamayý engelle
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        // Dash
        if (Input.GetKeyDown(KeyCode.E) && canDash && !isDashing && canAttack) // Saldýrý sýrasýnda dash'i engelle
        {
            StartCoroutine(Dash());
        }
        // Saldýrý (Sol Týk)
        if (Input.GetMouseButtonDown(0) && canAttack && !isDashing) 
        {
            StartCoroutine(Attack());
        }
        // Dash Cooldown Sayacý
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
                dashCooldownTimer = dashCooldown;
            }
        }
        // Saldýrý Cooldown Sayacý
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
        yield return new WaitForSeconds(attackCooldown); // Cooldown süresi kadar bekle
    }
    // Karakterin Sprite'ýný çevirme fonksiyonu
    void FlipCharacter(float moveInput)
    {
        // Dash veya saldýrý sýrasýnda karakterin dönmesini engelle
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
        currentScale.x *= -1; // X skalasýný tersine çevir
        transform.localScale = currentScale;
    }
    // DEBUG: GroundCheck'in nerede olduðunu görmek için Editor'da kýrmýzý bir daire çizer
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}