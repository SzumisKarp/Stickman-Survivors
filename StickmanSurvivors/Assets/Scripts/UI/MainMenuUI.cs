using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button passivesButton;
    public Button achievementsButton;
    public Button collectionButton;
    public Button quitButton;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject passivePanel;

    [Header("Optional")]
    public AudioSource uiSfx;
    public AudioClip clickClip;

    void Awake()
    {
        // Podpinamy zdarzenia
        startButton.onClick.AddListener(StartGame);
        passivesButton.onClick.AddListener(OpenPassives);
        achievementsButton.onClick.AddListener(OpenAchievements);
        collectionButton.onClick.AddListener(OpenCollection);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void PlayClick()
    {
        if (uiSfx && clickClip) uiSfx.PlayOneShot(clickClip);
    }

    public void StartGame()
    {
        PlayClick();
        SceneManager.LoadScene(1);            // index 1 = Prototype
    }

    public void OpenPassives()
    {
        PlayClick();
        mainPanel.SetActive(false);
        passivePanel.SetActive(true);
    }
    public void ClosePassives()
    {
        PlayClick();
        passivePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OpenAchievements()
    {
        PlayClick();
        Debug.Log("Achievements menu – TODO (później)");
    }

    public void OpenCollection()
    {
        PlayClick();
        Debug.Log("Collection menu – TODO (później)");
    }

    public void QuitGame()
    {
        PlayClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
