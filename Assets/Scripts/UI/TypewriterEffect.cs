using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Metin Ayarları")]
    public TextMeshProUGUI textComponent;
    [TextArea] public string fullText;
    public float typingSpeed = 0.03f;

    [Header("Ses")]
    public AudioSource audioSource;
    public AudioClip typeSound;
    public float soundPitchRandomness = 0.1f; // Her harfte küçük varyasyon için

    [Header("Geçiş")]
    public GameObject nextButton;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool textFullyShown = false;

    void Start()
    {
        textComponent.text = "";
        nextButton.SetActive(false);
        typingCoroutine = StartCoroutine(TypeText());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipTyping();
            }
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        textFullyShown = false;

        yield return new WaitForSeconds(1f); // Başlamadan önce bekle

        foreach (char c in fullText)
        {
            textComponent.text += c;

            if (char.IsLetterOrDigit(c) || char.IsPunctuation(c)) // Sadece anlamlı karakterlerde ses çal
            {
                PlayTypeSound();
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        FinishTyping();
    }

    void PlayTypeSound()
    {
        if (audioSource != null && typeSound != null)
        {
            audioSource.pitch = 0.75f + Random.Range(-soundPitchRandomness, soundPitchRandomness);
            audioSource.PlayOneShot(typeSound, 0.1f);
        }
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textComponent.text = fullText;
        FinishTyping();
    }

    void FinishTyping()
    {
        isTyping = false;
        textFullyShown = true;
        nextButton.SetActive(true);
    }

    public bool IsTextComplete()
    {
        return textFullyShown;
    }
}
