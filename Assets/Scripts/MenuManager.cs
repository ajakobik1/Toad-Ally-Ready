using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Next scene name")]
    public string nextSceneName = "Phone"; 

    [Header("Interface")]
    public GameObject creditsPanel;

    [Header("Sound")]
    public Image speakerIcon;
    public Sprite speakerOn;
    public Sprite speakerOff;

    private bool isMuted = false;
    private SceneTransition cachedSceneTransition;

    void Start()
    {

        cachedSceneTransition = FindFirstObjectByType<SceneTransition>();
    }

    public void PlayGame()
    {
        if (cachedSceneTransition != null)
        {
            cachedSceneTransition.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransition not found in the scene! Loading game directly.");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void ToggleCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game has been quit!");
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted;

        AudioListener.volume = isMuted ? 0f : 1f;

        if (speakerIcon != null)
        {
            speakerIcon.sprite = isMuted ? speakerOff : speakerOn;
        }
    }
}