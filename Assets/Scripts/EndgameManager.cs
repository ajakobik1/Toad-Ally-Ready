using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndgameManager : MonoBehaviour
{
    [Header("interaction")]
    public GameObject interactTextTMP;

    [Header("Coffee cups in the scene")]
    public Transform coffeeCupTransform;
    public GameObject spilledCoffeeObject;

    [Header("Frogs")]
    public SpriteRenderer frogRenderer;
    public Sprite surpriseSprite;
    public SpriteRenderer frogRenderer2;
    public Sprite surpriseSprite2;

    [Header("Coffee Flood UI")]
    public Image coffeeFloodImage;
    public float floodDuration = 3f;

    [Header("Final Message")]
    public TextMeshProUGUI finalTextTMP;
    [TextArea(3, 10)]
    public string endMessage = "Thank you for playing my game, hope you enjoyed!";
    public float typeSpeed = 0.05f;

    [Header("Return to Menu")]
    public GameObject menuButton;

    [Header("Final Sounds")]
    public AudioClip cupSpillSound;
    public AudioClip frogSurpriseSound;

    [Header("Typing Sound")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;

    private bool sequenceStarted = false;

    void Start()
    {
        if (interactTextTMP != null) interactTextTMP.SetActive(false);
        if (spilledCoffeeObject != null) spilledCoffeeObject.SetActive(false);
        if (coffeeFloodImage != null) coffeeFloodImage.fillAmount = 0f;
        if (finalTextTMP != null)
        {
            finalTextTMP.text = "";
            finalTextTMP.gameObject.SetActive(false);
        }
        if (menuButton != null) menuButton.SetActive(false);

        Invoke("ShowPressE", 1.5f);
    }

    void ShowPressE()
    {
        if (interactTextTMP != null)
        {
            interactTextTMP.SetActive(true);
            StartCoroutine(JuicyPopGeneric(interactTextTMP.transform, 1.3f));
        }
    }

    void Update()
    {
        if (!sequenceStarted && interactTextTMP != null && interactTextTMP.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(StartEndingSequence());
            }
        }
    }

    IEnumerator StartEndingSequence()
    {
        sequenceStarted = true;
        if (interactTextTMP != null) interactTextTMP.SetActive(false);

        if (AudioManager.instance != null && cupSpillSound != null)
        {
            AudioManager.instance.PlaySFX(cupSpillSound);
        }

        if (coffeeCupTransform != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, -90f);
            float rotTime = 0;
            while (rotTime < 0.3f)
            {
                rotTime += Time.deltaTime;
                coffeeCupTransform.rotation = Quaternion.Lerp(coffeeCupTransform.rotation, targetRotation, rotTime / 0.3f);
                yield return null;
            }
        }

        if (spilledCoffeeObject != null) spilledCoffeeObject.SetActive(true);

        if (AudioManager.instance != null && frogSurpriseSound != null)
        {
            AudioManager.instance.PlaySFX(frogSurpriseSound);
        }

        if (frogRenderer != null && surpriseSprite != null)
        {
            frogRenderer.sprite = surpriseSprite;
            StartCoroutine(JuicyPopGeneric(frogRenderer.transform, 1.4f));
        }
        if (frogRenderer2 != null && surpriseSprite2 != null)
        {
            frogRenderer2.sprite = surpriseSprite2;
            StartCoroutine(JuicyPopGeneric(frogRenderer2.transform, 1.4f));
        }

        yield return new WaitForSeconds(2.5f);

        if (coffeeFloodImage != null)
        {
            float fillTime = 0;
            while (fillTime < 1f)
            {
                fillTime += Time.deltaTime / floodDuration;
                float bouncyTime = fillTime + (Mathf.Sin(fillTime * Mathf.PI * 4f) * 0.05f);
                coffeeFloodImage.fillAmount = Mathf.Lerp(0f, 1f, bouncyTime);
                yield return null;
            }
            coffeeFloodImage.fillAmount = 1f;
        }

        yield return new WaitForSeconds(1f);

        if (finalTextTMP != null)
        {
            finalTextTMP.gameObject.SetActive(true);
            finalTextTMP.text = "";

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

            foreach (char c in endMessage.ToCharArray())
            {
                finalTextTMP.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            if (typingAudioSource != null)
            {
                typingAudioSource.Stop();
            }
        }

        yield return new WaitForSeconds(1f);

        if (menuButton != null)
        {
            menuButton.SetActive(true);
            StartCoroutine(JuicyPopGeneric(menuButton.transform, 1.2f));
        }
    }

    public void ReturnToMenu()
    {
        SceneTransition curtain = FindFirstObjectByType<SceneTransition>();
        if (curtain != null) curtain.LoadScene("Menu");
        else SceneManager.LoadScene("Menu");
    }

    IEnumerator JuicyPopGeneric(Transform objTransform, float multiplier = 1.3f)
    {
        Vector3 baseScale = objTransform.localScale;
        Vector3 targetScale = baseScale * multiplier;
        float animSpeed = 0.15f;
        float time = 0;
        while (time < animSpeed) { time += Time.deltaTime; objTransform.localScale = Vector3.Lerp(baseScale, targetScale, time / animSpeed); yield return null; }
        time = 0;
        while (time < animSpeed) { time += Time.deltaTime; objTransform.localScale = Vector3.Lerp(targetScale, baseScale, time / animSpeed); yield return null; }
        objTransform.localScale = baseScale;
    }
}