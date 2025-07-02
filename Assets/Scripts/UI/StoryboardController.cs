using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryboardController : MonoBehaviour
{
    public GameObject[] storyboards;
    public Button nextButton;

    private int currentIndex = 0;

    private void Start()
    {
        ShowStoryboard(currentIndex);
        nextButton.onClick.AddListener(Next);
    }

    private void ShowStoryboard(int index)
    {
        for (int i = 0; i < storyboards.Length; i++)
        {
            storyboards[i].SetActive(i == index);
        }
    }

    private void Next()
    {
        currentIndex++;

        if (currentIndex >= storyboards.Length)
        {
            SceneController.LoadSceneWithFade("Level1");
            return;
        }

        ShowStoryboard(currentIndex);
    }
}
