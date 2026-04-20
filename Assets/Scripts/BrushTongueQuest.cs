
/* * This file needs a huge refactor and transforming into a few smaller ones, 
 * but during the jam I was running out of time and just didn't really care at that point, 
 * only wanted to finish in time. I will definitely come back here in the future to optimize!
 */

using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TongueQuest : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI interactText;
    public GameObject dialogBackground;
    public GameObject interactPanelParent;

    [Header("Frog Animations")]
    public SpriteRenderer frogRenderer;
    public Sprite normalSprite;
    public Sprite surprisedSprite;

    [Range(1f, 10f)]
    public float shockDuration = 3.5f;

    [Header("Scrub Minigame")]
    public Sprite tongueOutSprite;

    private Coroutine surpriseCoroutine;

    private string currentTrigger = "";
    private GameObject currentObject;
    private GameObject uiTargetObject;

    private bool hasCheckedSink = false;
    private bool hasBrush = false;
    private bool hasPondScum = false;
    private bool hasKeys = false;

    public bool isMinigameActive = false;
    private int mashCount = 0;
    private int requiredMashes = 20;
    private bool waitingForD = false;
    private float dialogTimer = 0f;

    private Vector3 bgOriginalScale;
    private Dictionary<GameObject, Vector3> originalObjectScales = new Dictionary<GameObject, Vector3>();

    [Header("Dialogue Settings")]
    public float typeSpeed = 0.05f;
    private Coroutine typingCoroutine;

    [Header("Typing Sound")]
    public AudioClip typingSound;
    public AudioSource typingAudioSource;

    private FrogMovement cachedFrogMovement;
    private Rigidbody2D cachedRb;
    private Animator cachedAnimator;
    private FrogStatus cachedFrogStatus;
    private BreakfastQuest cachedBreakfastQuest;
    private SceneTransition cachedSceneTransition;

    void Start()
    {
        cachedFrogMovement = GetComponent<FrogMovement>();
        cachedRb = GetComponent<Rigidbody2D>();
        cachedAnimator = GetComponent<Animator>();
        cachedFrogStatus = GetComponent<FrogStatus>();
        cachedBreakfastQuest = GetComponent<BreakfastQuest>();

        cachedSceneTransition = FindFirstObjectByType<SceneTransition>();

        HideInteractUI();
        if (dialogBackground != null)
        {
            bgOriginalScale = dialogBackground.transform.localScale;
            dialogBackground.SetActive(false);
        }
        dialogText.text = "";

        if (frogRenderer != null && normalSprite != null) frogRenderer.sprite = normalSprite;

        Invoke("PlayIntroDialog", 0.5f);
    }

    void PlayIntroDialog()
    {
        string introText = "Meeting at the coffee shop? Sounds lovely! Need to clear my 'To-Do' list and go.";
        ShowDialog(introText, 4f);
    }

    void Update()
    {
        if (dialogTimer > 0 && typingCoroutine == null)
        {
            dialogTimer -= Time.deltaTime;
            if (dialogTimer <= 0)
            {
                dialogText.text = "";
                if (dialogBackground != null) dialogBackground.SetActive(false);
            }
        }

        if (isMinigameActive)
        {
            if (cachedFrogMovement != null) cachedFrogMovement.enabled = false;
            if (cachedRb != null) cachedRb.linearVelocity = Vector2.zero;
            PlayMinigame();
            return;
        }

        if (uiTargetObject != null && interactPanelParent != null && interactPanelParent.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(uiTargetObject.transform.position);

            if (uiTargetObject.CompareTag("Door"))
            {
                interactPanelParent.transform.position = screenPos + new Vector3(0, -60, 0);
            }
            else
            {
                interactPanelParent.transform.position = screenPos + new Vector3(0, 80, 0);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && currentTrigger != "") Interact();
    }

    public void ShowDialog(string text, float timeToShow = 3f)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            if (typingAudioSource != null) typingAudioSource.Stop();
        }

        dialogTimer = timeToShow;
        if (dialogBackground != null)
        {
            dialogBackground.SetActive(true);
            StartCoroutine(JuicyPopGeneric(dialogBackground.transform, bgOriginalScale, 1.6f));
        }

        if (AudioManager.instance != null && AudioManager.instance.dialogSound != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.dialogSound);
        }

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string fullText)
    {
        dialogText.text = "";

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

        foreach (char c in fullText.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (typingAudioSource != null)
        {
            typingAudioSource.Stop();
        }

        typingCoroutine = null;
    }

    public void ShowSurprisedFrog()
    {
        if (frogRenderer == null || surprisedSprite == null) return;
        if (surpriseCoroutine != null) StopCoroutine(surpriseCoroutine);
        surpriseCoroutine = StartCoroutine(JuicyPopSurprise(shockDuration));
    }

    IEnumerator JuicyPopSurprise(float duration)
    {
        if (cachedAnimator != null) cachedAnimator.enabled = false;
        frogRenderer.sprite = surprisedSprite;
        Vector3 baseScale = transform.localScale;
        Vector3 targetScale = baseScale * 1.3f;
        float animSpeed = 0.2f;
        float time = 0;
        while (time < animSpeed) { time += Time.deltaTime; transform.localScale = Vector3.Lerp(baseScale, targetScale, time / animSpeed); yield return null; }
        time = 0;
        while (time < animSpeed) { time += Time.deltaTime; transform.localScale = Vector3.Lerp(targetScale, baseScale, time / animSpeed); yield return null; }
        transform.localScale = baseScale;
        yield return new WaitForSeconds(duration);
        frogRenderer.sprite = normalSprite;
        if (cachedAnimator != null) cachedAnimator.enabled = true;
        surpriseCoroutine = null;
    }

    IEnumerator JuicyPopGeneric(Transform objTransform, Vector3 baseScale, float multiplier)
    {
        Vector3 targetScale = baseScale * multiplier;
        float animSpeed = 0.15f;
        float time = 0;
        while (time < animSpeed) { if (objTransform == null) yield break; time += Time.deltaTime; objTransform.localScale = Vector3.Lerp(baseScale, targetScale, time / animSpeed); yield return null; }
        time = 0;
        while (time < animSpeed) { if (objTransform == null) yield break; time += Time.deltaTime; objTransform.localScale = Vector3.Lerp(targetScale, baseScale, time / animSpeed); yield return null; }
        if (objTransform != null) objTransform.localScale = baseScale;
    }

    void Interact()
    {
        if (currentTrigger == "Sink")
        {
            if (cachedFrogStatus != null && cachedFrogStatus.isTongueDone)
            {
                ShowDialog("My tongue is squeaky clean already.", 3f);
            }
            else
            {
                hasCheckedSink = true;
                if (!hasBrush || !hasPondScum) ShowDialog("My tongue is so long... I need a HUGE brush. And some tonguepaste.");
                else
                {
                    ShowDialog("MASH 'A' and 'D' TO SCRUB!", 99f);
                    isMinigameActive = true;
                    mashCount = 0;
                    HideInteractUI();

                    if (cachedAnimator != null) cachedAnimator.enabled = false;

                    if (frogRenderer != null && tongueOutSprite != null)
                    {
                        frogRenderer.sprite = tongueOutSprite;
                    }
                }
            }
        }
        else if (currentTrigger == "Brush" && !hasBrush)
        {
            if (!hasCheckedSink) ShowDialog("It's a toilet brush. I have no business with it right now.");
            else
            {
                hasBrush = true;
                ShowDialog("Got the HUGE brush!");
                Destroy(currentObject);
                HideInteractUI();
                currentTrigger = "";
            }
        }
        else if (currentTrigger == "Pond")
        {
            if (cachedBreakfastQuest != null && cachedBreakfastQuest.questStage == 1) return;

            if (!hasCheckedSink) ShowDialog("Just a smelly indoor pond. Better not touch it for no reason.");
            else if (!hasPondScum)
            {
                hasPondScum = true;
                ShowDialog("Got pond scum... smells minty!");
            }
            else
            {
                ShowDialog("The water is calm. I don't need any more scum.");
            }
        }
        else if (currentTrigger == "Keys" && !hasKeys)
        {
            if (cachedFrogStatus != null && !cachedFrogStatus.AreAllTasksDone())
            {
                ShowDialog("I'm not ready yet, I can't go.");
            }
            else
            {
                hasKeys = true;
                ShowDialog("Got the keys!");
                Destroy(currentObject);
                HideInteractUI();
                currentTrigger = "";
            }
        }
        else if (currentTrigger == "Door")
        {
            if (!hasKeys) ShowDialog("I need my keys to lock the door.");
            else
            {
                ShowDialog("Time to go!", 1f);

                if (cachedSceneTransition != null) cachedSceneTransition.LoadScene("Coffee");
                else UnityEngine.SceneManagement.SceneManager.LoadScene("Coffee");
            }
        }
    }

    void PlayMinigame()
    {
        if (!waitingForD && Input.GetKeyDown(KeyCode.A)) { mashCount++; waitingForD = true; if (typingCoroutine != null) StopCoroutine(typingCoroutine); dialogText.text = "SCRUB! (A)"; }
        else if (waitingForD && Input.GetKeyDown(KeyCode.D)) { mashCount++; waitingForD = false; if (typingCoroutine != null) StopCoroutine(typingCoroutine); dialogText.text = "SCRUB! (D)"; }
        if (mashCount >= requiredMashes)
        {
            isMinigameActive = false;
            ShowDialog("TONGUE CLEAN! (Smells like swamp)", 4f);

            if (cachedFrogStatus != null) cachedFrogStatus.TaskComplete_ScrubTongue();
            if (cachedFrogMovement != null) cachedFrogMovement.enabled = true;

            currentTrigger = "";

            if (cachedAnimator != null) cachedAnimator.enabled = true;

            if (frogRenderer != null && normalSprite != null)
            {
                frogRenderer.sprite = normalSprite;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            if (cachedFrogStatus != null && !cachedFrogStatus.AreAllTasksDone())
            {
                return;
            }
        }
        if (collision.gameObject.CompareTag("Sink") || collision.gameObject.CompareTag("Brush") || collision.gameObject.CompareTag("Pond") || collision.gameObject.CompareTag("Keys") || collision.gameObject.CompareTag("Door"))
        {
            currentTrigger = collision.gameObject.tag;
            currentObject = collision.gameObject;
            ShowInteractUI(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentTrigger == collision.gameObject.tag)
        {
            ClearTrigger();
        }
    }

    public void ClearTrigger()
    {
        currentTrigger = "";
        currentObject = null;
        HideInteractUI();
    }

    public void ShowInteractUI(GameObject target)
    {
        uiTargetObject = target;
        if (target != null && !originalObjectScales.ContainsKey(target)) { originalObjectScales[target] = target.transform.localScale; }
        if (target != null) { StartCoroutine(JuicyPopGeneric(target.transform, originalObjectScales[target], 1.2f)); }
        if (interactText != null) { interactText.text = "Press 'E'"; interactText.gameObject.SetActive(true); }
        if (interactPanelParent != null) interactPanelParent.SetActive(true);

        if (AudioManager.instance != null && AudioManager.instance.popupSound != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.popupSound);
        }
    }

    public void HideInteractUI()
    {
        uiTargetObject = null;
        if (interactText != null) interactText.gameObject.SetActive(false);
        if (interactPanelParent != null) interactPanelParent.SetActive(false);
    }

    public bool IsTalking()
    {
        return typingCoroutine != null && !isMinigameActive;
    }
}