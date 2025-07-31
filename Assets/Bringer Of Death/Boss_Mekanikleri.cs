using UnityEngine;
using System.Collections; // Coroutine'ler için

public class BossAI2D : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3f; // Boss'un hareket hýzý (Artýrýldý)
    public float idleMoveRange = 3f; // Idle durumunda saða sola ne kadar gidecek (Artýrýldý)
    public float idleMoveDuration = 1.5f; // Her bir idle hareketi ne kadar sürecek (Artýrýldý)
    public float idleDelay = 1f; // Idle hareketleri arasýndaki bekleme süresi

    [Header("Saldýrý Ayarlarý")]
    public float detectionRange = 6f; // Oyuncuyu algýlama menzili
    public float attackRange = 1f; // Saldýrý menzili
    public float attackCooldown = 2f; // Saldýrýlar arasý bekleme süresi
    public int attackDamage = 10; // Saldýrý hasarý

    [Header("Referanslar")]
    public Transform playerTransform; // Oyuncunun Transform bileþeni
    public Animator bossAnimator; // Boss'un Animator bileþeni
    public LayerMask playerLayer; // Oyuncu Layer'ý
    public SpriteRenderer bossSpriteRenderer; // Boss'un SpriteRenderer bileþeni (Yön çevirme için)

    private Vector2 initialPosition; // Boss'un baþlangýç pozisyonu
    private bool isPlayerDetected = false;
    private bool canAttack = true;
    private bool isMovingIdle = false; // Idle hareketin devam edip etmediðini kontrol eder

    // Animasyon Parametre Ýsimleri (Animator Controller'daki isimlerle BÝREBÝR AYNI olmalý)
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_MOVE = "Walk"; // Animator'daki parametre adý "Move" ise bu þekilde olmalý
    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_HIT = "Hurt"; // Animator'daki parametre adý "Hit" ise bu þekilde olmalý
    private const string ANIM_DIE = "Death"; // Animator'daki parametre adý "Die" ise bu þekilde olmalý

    // Boss'un mevcut saðlýk durumu
    private int currentHealth = 100;

    void Start()
    {
        initialPosition = transform.position; // Baþlangýç pozisyonunu kaydet
        Debug.Log("Boss Baþlangýç Pozisyonu: " + initialPosition); // Debug: Baþlangýç pozisyonunu kontrol et

        // PlayerTransform atanmadýysa, "Player" tag'ine sahip objeyi bulmaya çalýþ
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player nesnesi bulunamadý. Lütfen oyuncunuza 'Player' tag'ini atayýn veya Inspector'dan sürükleyin.");
            }
        }

        // SpriteRenderer atanmadýysa, otomatik bulmaya çalýþ
        if (bossSpriteRenderer == null)
        {
            bossSpriteRenderer = GetComponent<SpriteRenderer>();
            if (bossSpriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer bileþeni bulunamadý! Lütfen atayýn veya GameObject üzerinde olduðundan emin olun.");
            }
        }

        // Animator atanmadýysa, otomatik bulmaya çalýþ
        if (bossAnimator == null)
        {
            bossAnimator = GetComponent<Animator>();
            if (bossAnimator == null)
            {
                Debug.LogError("Animator bileþeni bulunamadý! Lütfen atayýn veya GameObject üzerinde olduðundan emin olun.");
            }
        }

        // Oyun baþladýðýnda idle hareket rutinini baþlat
        StartCoroutine(IdleMovementRoutine());
    }

    void Update()
    {
        CheckForPlayer(); // Her frame oyuncuyu kontrol et

        if (isPlayerDetected)
        {
            // Oyuncu algýlandýysa, idle hareketini durdur
            if (isMovingIdle)
            {
                StopCoroutine(IdleMovementRoutine());
                isMovingIdle = false;
                Debug.Log("Idle rutini durduruldu (oyuncu algýlandý).");
            }
            AttackPlayer(); // Oyuncuya saldýr
        }
        else // Oyuncu algýlanmadýysa
        {
            // Eðer idle hareket çalýþmýyorsa baþlat
            if (!isMovingIdle)
            {
                StartCoroutine(IdleMovementRoutine());
                Debug.Log("Idle rutini yeniden baþlatýldý (oyuncu algýlanmadý).");
            }
            // Idle durumundayken hareket animasyonunu aç (idle hareketini gösterir)
            if (bossAnimator != null)
            {
                bossAnimator.SetBool(ANIM_MOVE, true); // Idle hareket ederken Move animasyonu
                bossAnimator.SetBool(ANIM_ATTACK, false); // Saldýrý animasyonunu kapat
            }
        }
    }

    // --- Oyuncu Algýlama ---
    void CheckForPlayer()
    {
        // Belirtilen menzil ve layer'daki collider'ý kontrol et
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        isPlayerDetected = (hitCollider != null);

        // Debug: Oyuncu algýlama durumunu kontrol et
        if (isPlayerDetected)
        {
            // Debug.Log("Oyuncu algýlandý! Boss artýk saldýrma modunda olmalý.");
        }
        else
        {
            // Debug.Log("Oyuncu algýlanmadý. Boss idle/hareket modunda.");
        }
    }

    // --- Oyuncuya Saldýrý ---
    void AttackPlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("AttackPlayer çaðrýldý ama playerTransform atanmamýþ!");
            return;
        }

        // Oyuncuya doðru hareket et
        Vector2 targetPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Yönü ayarla (sprite'ý çevir)
        FlipSprite(targetPosition.x - transform.position.x);

        // Move animasyonunu aç (saldýrýrken de hareket edebilir)
        if (bossAnimator != null)
        {
            bossAnimator.SetBool(ANIM_MOVE, true);
        }

        // Saldýrý menzilinde miyiz ve saldýrabilir miyiz?
        float distanceToPlayer = Vector2.Distance(transform.position, targetPosition);
        // Debug.Log("Oyuncuya mesafe: " + distanceToPlayer + ", Saldýrý menzili: " + attackRange);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false; // Saldýrý yapamaz.
        if (bossAnimator != null)
        {
            bossAnimator.SetBool(ANIM_MOVE, false); // Hareket animasyonunu kapat
            bossAnimator.SetTrigger(ANIM_ATTACK); // Saldýrý animasyonunu tetikle
            Debug.Log("Saldýrý animasyonu tetiklendi.");
        }

        // Animasyonun orta noktasýna kadar bekle (Hasarýn verileceði nokta)
        yield return new WaitForSeconds(0.5f);

        // Oyuncuya hasar ver (oyuncunuzda TakeDamage methodu olduðunu varsayar)
        if (playerTransform != null)
        {
            // Oyuncunun script'ine eriþip TakeDamage methodunu çaðýrýn
            // Örnek: playerTransform.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            Debug.Log("Oyuncuya " + attackDamage + " hasar verildi!");
        }

        // Saldýrý cooldown'ý kadar bekle
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; // Tekrar saldýrabilir.
        Debug.Log("Saldýrý cooldown'ý bitti. Tekrar saldýrabilir.");
    }

    // --- Idle Hareket ---
    IEnumerator IdleMovementRoutine()
    {
        isMovingIdle = true;
        Debug.Log("IdleMovementRoutine baþlatýldý.");

        while (!isPlayerDetected) // Player algýlanmadýðý sürece devam et
        {
            // Rastgele saða veya sola hareket için hedef belirle
            Vector2 targetOffset = new Vector2(Random.Range(-idleMoveRange, idleMoveRange), 0);
            Vector2 targetPosition = initialPosition + targetOffset; // Baþlangýç pozisyonuna göre offset

            Debug.Log("Yeni Idle Hedef: " + targetPosition + " (offset: " + targetOffset + "). Baþlangýç: " + initialPosition);
            Debug.Log("Mevcut Pozisyon (Idle Hareket Baþlangýcý): " + transform.position);

            float startTime = Time.time;
            // Hareket yönüne göre sprite'ý çevir
            FlipSprite(targetPosition.x - transform.position.x);

            // Hedefe doðru hareket etme döngüsü
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f && Time.time < startTime + idleMoveDuration)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (bossAnimator != null)
                {
                    bossAnimator.SetBool(ANIM_MOVE, true); // Hareket ederken Move animasyonunu açýk tut
                }
                // Debug.Log("Idle hareket. Kalan mesafe: " + Vector2.Distance(transform.position, targetPosition) + ", Süre: " + (Time.time - startTime));
                yield return null; // Bir sonraki frame'e kadar bekle
            }

            // Hedefe ulaþýldýðýnda veya süre dolduðunda hareketi durdur
            if (bossAnimator != null)
            {
                bossAnimator.SetBool(ANIM_MOVE, false); // Hareket bitince Move animasyonunu kapat (Idle'a döner)
                Debug.Log("ANIM_MOVE (Move) false yapýldý. Hedefe ulaþýldý veya süre doldu. Mevcut pozisyon: " + transform.position);
            }

            yield return new WaitForSeconds(idleDelay); // Hareketler arasý bekleme süresi
            Debug.Log("Idle hareketler arasý bekleme bitti.");
        }
        isMovingIdle = false; // Oyuncu algýlandýðý için rutin durdu
        Debug.Log("Idle rutini durduruldu (isPlayerDetected true oldu).");
    }

    // --- Sprite'ý Yöne Çevirme ---
    void FlipSprite(float directionX)
    {
        if (bossSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer referansý yok! Sprite çevrilemiyor.");
            return;
        }

        if (directionX > 0) // Saða gidiyorsa (hedef mevcut konumdan saðda)
        {
            if (bossSpriteRenderer.flipX == true) // Sola dönükse saða çevir
            {
                bossSpriteRenderer.flipX = false;
                Debug.Log("Sprite saða döndü (directionX: " + directionX + ")");
            }
        }
        else if (directionX < 0) // Sola gidiyorsa (hedef mevcut konumdan solda)
        {
            if (bossSpriteRenderer.flipX == false) // Saða dönükse sola çevir
            {
                bossSpriteRenderer.flipX = true;
                Debug.Log("Sprite sola döndü (directionX: " + directionX + ")");
            }
        }
        // directionX 0 ise (duruyorsa veya tam hedefteyse) bir þey yapma
    }

    // --- Hasar Alma ---
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss TakeDamage fonksiyonu çaðrýldý! Hasar: " + damage + ", Kalan Can: " + currentHealth);

        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger(ANIM_HIT); // Hasar alma animasyonunu tetikle
            Debug.Log("Hurt animasyonu tetiklendi.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- Ölme ---
    void Die()
    {
        // Eðer zaten öldüyse tekrar çaðýrmayý engelle
        if (currentHealth <= 0 && !enabled) return;

        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger(ANIM_DIE); // Ölüm animasyonunu tetikle
            Debug.Log("Ölüm animasyonu tetiklendi.");
        }

        Debug.Log("Boss öldü!");

        // Script'i, collider'ý ve Rigidbody'yi devre dýþý býrak
        enabled = false;
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        if (GetComponent<Rigidbody2D>() != null)
        {
            // RigidbodyType2D.Static yapýyoruz ki fiziksel olarak etkileþimde olmasýn
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        // Belirli bir süre sonra GameObject'i yok et
        Destroy(gameObject, 2f);
    }

    // --- Gizmos (Algýlama ve Saldýrý Menzili Görselleþtirme) ---
    void OnDrawGizmosSelected()
    {
        // Algýlama menzili
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Saldýrý menzili
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}