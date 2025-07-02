using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void LoadSceneWithFade(string sceneName)
    {
        var fade = UnityEngine.Object.FindFirstObjectByType<FadeManager>();
        if (fade != null)
            fade.FadeToScene(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
