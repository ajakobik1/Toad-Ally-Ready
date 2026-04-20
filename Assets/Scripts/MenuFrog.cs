using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class MenuBubblyFrog : MonoBehaviour, IPointerClickHandler
{
    [Header("Animation")]
    public float boingSpeed = 4f;
    public float boingAmount = 0.05f;

    [Header("Talking")]
    public Sprite mouthClosed;
    public Sprite mouthOpen;
    public float talkSpeed = 0.2f;

    private Image rend;
    private Vector3 originalScale;
    private float talkTimer = 0f;

    private float clickPopMultiplier = 1f;

    void Start()
    {
        rend = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayClickSound();
        clickPopMultiplier = 1.3f;
    }

    void Update()
    {
        HandleBoingAnimation();
        HandleTalkingAnimation();
    }

    private void PlayClickSound()
    {
        if (AudioManager.instance != null && AudioManager.instance.frogClickSound != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.frogClickSound);
        }
    }

    private void HandleBoingAnimation()
    {
        clickPopMultiplier = Mathf.Lerp(clickPopMultiplier, 1f, Time.deltaTime * 10f);

        float sine = Mathf.Sin(Time.time * boingSpeed);
        float squashFactorY = 1.0f + (sine * boingAmount);
        float stretchFactorX = 1.0f - (sine * boingAmount);

        transform.localScale = new Vector3(
            originalScale.x * stretchFactorX * clickPopMultiplier,
            originalScale.y * squashFactorY * clickPopMultiplier,
            originalScale.z
        );
    }

    private void HandleTalkingAnimation()
    {
        if (mouthClosed == null || mouthOpen == null) return;

        talkTimer += Time.deltaTime;
        if (talkTimer >= talkSpeed)
        {
            talkTimer = 0f;
            rend.sprite = (rend.sprite == mouthClosed) ? mouthOpen : mouthClosed;
        }
    }
}