using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private static FadeManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        fadeImage.gameObject.SetActive(false);
        SetAlpha(1f);                          
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeAndSwitchScene(sceneName));
    }

    IEnumerator FadeAndSwitchScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(1f);

        SceneManager.LoadScene(sceneName);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(0f);

        fadeImage.gameObject.SetActive(false);
    }

    private void SetAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = Mathf.Clamp01(a);
        fadeImage.color = c;
    }
}
