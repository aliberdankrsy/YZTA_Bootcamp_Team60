using UnityEngine;
using System.Collections; // Coroutine'ler i�in

public class BossAI2D : MonoBehaviour
{
    [Header("Hareket Ayarlar�")]
    public float moveSpeed = 3f; // Boss'un hareket h�z� (Art�r�ld�)
    public float idleMoveRange = 3f; // Idle durumunda sa�a sola ne kadar gidecek (Art�r�ld�)
    public float idleMoveDuration = 1.5f; // Her bir idle hareketi ne kadar s�recek (Art�r�ld�)
    public float idleDelay = 1f; // Idle hareketleri aras�ndaki bekleme s�resi

    [Header("Sald�r� Ayarlar�")]
    public float detectionRange = 6f; // Oyuncuyu alg�lama menzili
    public float attackRange = 1f; // Sald�r� menzili
    public float attackCooldown = 2f; // Sald�r�lar aras� bekleme s�resi
    public int attackDamage = 10; // Sald�r� hasar�

    [Header("Referanslar")]
    public Transform playerTransform; // Oyuncunun Transform bile�eni
    public Animator bossAnimator; // Boss'un Animator bile�eni
    public LayerMask playerLayer; // Oyuncu Layer'�
    public SpriteRenderer bossSpriteRenderer; // Boss'un SpriteRenderer bile�eni (Y�n �evirme i�in)

    private Vector2 initialPosition; // Boss'un ba�lang�� pozisyonu
    private bool isPlayerDetected = false;
    private bool canAttack = true;
    private bool isMovingIdle = false; // Idle hareketin devam edip etmedi�ini kontrol eder

    // Animasyon Parametre �simleri (Animator Controller'daki isimlerle B�REB�R AYNI olmal�)
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_MOVE = "Walk"; // Animator'daki parametre ad� "Move" ise bu �ekilde olmal�
    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_HIT = "Hurt"; // Animator'daki parametre ad� "Hit" ise bu �ekilde olmal�
    private const string ANIM_DIE = "Death"; // Animator'daki parametre ad� "Die" ise bu �ekilde olmal�

    // Boss'un mevcut sa�l�k durumu
    private int currentHealth = 100;

    void Start()
    {
        initialPosition = transform.position; // Ba�lang�� pozisyonunu kaydet
        Debug.Log("Boss Ba�lang�� Pozisyonu: " + initialPosition); // Debug: Ba�lang�� pozisyonunu kontrol et

        // PlayerTransform atanmad�ysa, "Player" tag'ine sahip objeyi bulmaya �al��
        if (playerTransform == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player nesnesi bulunamad�. L�tfen oyuncunuza 'Player' tag'ini atay�n veya Inspector'dan s�r�kleyin.");
            }
        }

        // SpriteRenderer atanmad�ysa, otomatik bulmaya �al��
        if (bossSpriteRenderer == null)
        {
            bossSpriteRenderer = GetComponent<SpriteRenderer>();
            if (bossSpriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer bile�eni bulunamad�! L�tfen atay�n veya GameObject �zerinde oldu�undan emin olun.");
            }
        }

        // Animator atanmad�ysa, otomatik bulmaya �al��
        if (bossAnimator == null)
        {
            bossAnimator = GetComponent<Animator>();
            if (bossAnimator == null)
            {
                Debug.LogError("Animator bile�eni bulunamad�! L�tfen atay�n veya GameObject �zerinde oldu�undan emin olun.");
            }
        }

        // Oyun ba�lad���nda idle hareket rutinini ba�lat
        StartCoroutine(IdleMovementRoutine());
    }

    void Update()
    {
        CheckForPlayer(); // Her frame oyuncuyu kontrol et

        if (isPlayerDetected)
        {
            // Oyuncu alg�land�ysa, idle hareketini durdur
            if (isMovingIdle)
            {
                StopCoroutine(IdleMovementRoutine());
                isMovingIdle = false;
                Debug.Log("Idle rutini durduruldu (oyuncu alg�land�).");
            }
            AttackPlayer(); // Oyuncuya sald�r
        }
        else // Oyuncu alg�lanmad�ysa
        {
            // E�er idle hareket �al��m�yorsa ba�lat
            if (!isMovingIdle)
            {
                StartCoroutine(IdleMovementRoutine());
                Debug.Log("Idle rutini yeniden ba�lat�ld� (oyuncu alg�lanmad�).");
            }
            // Idle durumundayken hareket animasyonunu a� (idle hareketini g�sterir)
            if (bossAnimator != null)
            {
                bossAnimator.SetBool(ANIM_MOVE, true); // Idle hareket ederken Move animasyonu
                bossAnimator.SetBool(ANIM_ATTACK, false); // Sald�r� animasyonunu kapat
            }
        }
    }

    // --- Oyuncu Alg�lama ---
    void CheckForPlayer()
    {
        // Belirtilen menzil ve layer'daki collider'� kontrol et
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        isPlayerDetected = (hitCollider != null);

        // Debug: Oyuncu alg�lama durumunu kontrol et
        if (isPlayerDetected)
        {
            // Debug.Log("Oyuncu alg�land�! Boss art�k sald�rma modunda olmal�.");
        }
        else
        {
            // Debug.Log("Oyuncu alg�lanmad�. Boss idle/hareket modunda.");
        }
    }

    // --- Oyuncuya Sald�r� ---
    void AttackPlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("AttackPlayer �a�r�ld� ama playerTransform atanmam��!");
            return;
        }

        // Oyuncuya do�ru hareket et
        Vector2 targetPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Y�n� ayarla (sprite'� �evir)
        FlipSprite(targetPosition.x - transform.position.x);

        // Move animasyonunu a� (sald�r�rken de hareket edebilir)
        if (bossAnimator != null)
        {
            bossAnimator.SetBool(ANIM_MOVE, true);
        }

        // Sald�r� menzilinde miyiz ve sald�rabilir miyiz?
        float distanceToPlayer = Vector2.Distance(transform.position, targetPosition);
        // Debug.Log("Oyuncuya mesafe: " + distanceToPlayer + ", Sald�r� menzili: " + attackRange);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false; // Sald�r� yapamaz.
        if (bossAnimator != null)
        {
            bossAnimator.SetBool(ANIM_MOVE, false); // Hareket animasyonunu kapat
            bossAnimator.SetTrigger(ANIM_ATTACK); // Sald�r� animasyonunu tetikle
            Debug.Log("Sald�r� animasyonu tetiklendi.");
        }

        // Animasyonun orta noktas�na kadar bekle (Hasar�n verilece�i nokta)
        yield return new WaitForSeconds(0.5f);

        // Oyuncuya hasar ver (oyuncunuzda TakeDamage methodu oldu�unu varsayar)
        if (playerTransform != null)
        {
            // Oyuncunun script'ine eri�ip TakeDamage methodunu �a��r�n
            // �rnek: playerTransform.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            Debug.Log("Oyuncuya " + attackDamage + " hasar verildi!");
        }

        // Sald�r� cooldown'� kadar bekle
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true; // Tekrar sald�rabilir.
        Debug.Log("Sald�r� cooldown'� bitti. Tekrar sald�rabilir.");
    }

    // --- Idle Hareket ---
    IEnumerator IdleMovementRoutine()
    {
        isMovingIdle = true;
        Debug.Log("IdleMovementRoutine ba�lat�ld�.");

        while (!isPlayerDetected) // Player alg�lanmad��� s�rece devam et
        {
            // Rastgele sa�a veya sola hareket i�in hedef belirle
            Vector2 targetOffset = new Vector2(Random.Range(-idleMoveRange, idleMoveRange), 0);
            Vector2 targetPosition = initialPosition + targetOffset; // Ba�lang�� pozisyonuna g�re offset

            Debug.Log("Yeni Idle Hedef: " + targetPosition + " (offset: " + targetOffset + "). Ba�lang��: " + initialPosition);
            Debug.Log("Mevcut Pozisyon (Idle Hareket Ba�lang�c�): " + transform.position);

            float startTime = Time.time;
            // Hareket y�n�ne g�re sprite'� �evir
            FlipSprite(targetPosition.x - transform.position.x);

            // Hedefe do�ru hareket etme d�ng�s�
            while (Vector2.Distance(transform.position, targetPosition) > 0.1f && Time.time < startTime + idleMoveDuration)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                if (bossAnimator != null)
                {
                    bossAnimator.SetBool(ANIM_MOVE, true); // Hareket ederken Move animasyonunu a��k tut
                }
                // Debug.Log("Idle hareket. Kalan mesafe: " + Vector2.Distance(transform.position, targetPosition) + ", S�re: " + (Time.time - startTime));
                yield return null; // Bir sonraki frame'e kadar bekle
            }

            // Hedefe ula��ld���nda veya s�re doldu�unda hareketi durdur
            if (bossAnimator != null)
            {
                bossAnimator.SetBool(ANIM_MOVE, false); // Hareket bitince Move animasyonunu kapat (Idle'a d�ner)
                Debug.Log("ANIM_MOVE (Move) false yap�ld�. Hedefe ula��ld� veya s�re doldu. Mevcut pozisyon: " + transform.position);
            }

            yield return new WaitForSeconds(idleDelay); // Hareketler aras� bekleme s�resi
            Debug.Log("Idle hareketler aras� bekleme bitti.");
        }
        isMovingIdle = false; // Oyuncu alg�land��� i�in rutin durdu
        Debug.Log("Idle rutini durduruldu (isPlayerDetected true oldu).");
    }

    // --- Sprite'� Y�ne �evirme ---
    void FlipSprite(float directionX)
    {
        if (bossSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer referans� yok! Sprite �evrilemiyor.");
            return;
        }

        if (directionX > 0) // Sa�a gidiyorsa (hedef mevcut konumdan sa�da)
        {
            if (bossSpriteRenderer.flipX == true) // Sola d�n�kse sa�a �evir
            {
                bossSpriteRenderer.flipX = false;
                Debug.Log("Sprite sa�a d�nd� (directionX: " + directionX + ")");
            }
        }
        else if (directionX < 0) // Sola gidiyorsa (hedef mevcut konumdan solda)
        {
            if (bossSpriteRenderer.flipX == false) // Sa�a d�n�kse sola �evir
            {
                bossSpriteRenderer.flipX = true;
                Debug.Log("Sprite sola d�nd� (directionX: " + directionX + ")");
            }
        }
        // directionX 0 ise (duruyorsa veya tam hedefteyse) bir �ey yapma
    }

    // --- Hasar Alma ---
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss TakeDamage fonksiyonu �a�r�ld�! Hasar: " + damage + ", Kalan Can: " + currentHealth);

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

    // --- �lme ---
    void Die()
    {
        // E�er zaten �ld�yse tekrar �a��rmay� engelle
        if (currentHealth <= 0 && !enabled) return;

        if (bossAnimator != null)
        {
            bossAnimator.SetTrigger(ANIM_DIE); // �l�m animasyonunu tetikle
            Debug.Log("�l�m animasyonu tetiklendi.");
        }

        Debug.Log("Boss �ld�!");

        // Script'i, collider'� ve Rigidbody'yi devre d��� b�rak
        enabled = false;
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        if (GetComponent<Rigidbody2D>() != null)
        {
            // RigidbodyType2D.Static yap�yoruz ki fiziksel olarak etkile�imde olmas�n
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }

        // Belirli bir s�re sonra GameObject'i yok et
        Destroy(gameObject, 2f);
    }

    // --- Gizmos (Alg�lama ve Sald�r� Menzili G�rselle�tirme) ---
    void OnDrawGizmosSelected()
    {
        // Alg�lama menzili
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Sald�r� menzili
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}