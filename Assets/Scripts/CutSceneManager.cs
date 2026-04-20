using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PhoneCutscene : MonoBehaviour
{
    [Header("Message")]
    public TextMeshProUGUI phoneText;
    [TextArea(3, 10)]
    public string message = "Hi Froggo...\nI really wanna\ngo to town\ntoday. Meet me\nat the coffee\nshop when you\nget ready.";
    public float typeSpeed = 0.05f;

    [Header("Button and Transition")]
    public GameObject letsGoButton;
    public string gameSceneName = "SampleScene";

    [Header("Sounds")]
    public AudioClip notificationSound;
    public AudioClip typingSound;
    public AudioClip buttonPopSound;      
    public AudioSource typingAudioSource;

    void Start()
    {
        if (phoneText != null) phoneText.text = "";
        if (letsGoButton != null) letsGoButton.SetActive(false);

        Invoke("StartPhoneSequence", 0.5f);
    }

    void StartPhoneSequence()
    {
        if (AudioManager.instance != null && notificationSound != null)
        {
            AudioManager.instance.PlaySFX(notificationSound);
        }

        if (typingAudioSource != null && typingSound != null)
        {
            typingAudioSource.clip = typingSound;
            typingAudioSource.loop = true;

            if (AudioManager.instance != null)
            {
                typingAudioSource.mute = AudioManager.instance.musicSource.mute;
            }

            typingAudioSource.Play();
        }

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (char c in message.ToCharArray())
        {
            phoneText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (typingAudioSource != null)
        {
            typingAudioSource.Stop();
        }

        if (letsGoButton != null) letsGoButton.SetActive(true);

        if (AudioManager.instance != null && buttonPopSound != null)
        {
            AudioManager.instance.PlaySFX(buttonPopSound);
        }
    }

    public void LoadMainGame()
    {
        SceneTransition curtain = FindFirstObjectByType<SceneTransition>();

        if (curtain != null)
        {
            curtain.LoadScene(gameSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}