using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject mainMenuGroup;
    public GameObject loadPanel;
    public GameObject settingsPanel;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
    
    public void OpenLoadPanel()
    {
        mainMenuGroup.SetActive(false);
        loadPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        mainMenuGroup.SetActive(false);
        loadPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ClosePanels()
    {
        loadPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainMenuGroup.SetActive(true);
    }
    public void PlayGame()
    {
        SceneController.LoadSceneWithFade("IntroStoryboard");

    }
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
