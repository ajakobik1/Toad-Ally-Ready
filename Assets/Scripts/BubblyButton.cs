using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BubblyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Bubbly Animation")]
    public float boingSpeed = 5f;
    public float boingAmount = 0.06f;

    [Header("Hover")]
    public float hoverScaleMultiplier = 1.15f;
    public float hoverTransitionSpeed = 15f;

    [Header("Colours")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.7f, 0.7f, 0.7f);

    private Image buttonImage;
    private Vector3 originalScale;
    private float currentHoverMultiplier = 1f;
    private float targetHoverMultiplier = 1f;
    private float randomOffset;

    void Awake()
    {
        originalScale = transform.localScale;
        randomOffset = Random.Range(0f, 100f);

        buttonImage = GetComponent<Image>();
        if (buttonImage != null) buttonImage.color = normalColor;
    }

    // Button animation and colour transition 
    void Update()
    {
        currentHoverMultiplier = Mathf.Lerp(currentHoverMultiplier, targetHoverMultiplier, Time.unscaledDeltaTime * hoverTransitionSpeed);

        float sine = Mathf.Sin((Time.unscaledTime + randomOffset) * boingSpeed);
        float squashFactorY = 1.0f + (sine * boingAmount);
        float stretchFactorX = 1.0f - (sine * boingAmount);

        transform.localScale = new Vector3(
            originalScale.x * currentHoverMultiplier * stretchFactorX,
            originalScale.y * currentHoverMultiplier * squashFactorY,
            originalScale.z
        );

        if (buttonImage != null)
        {
            Color targetColor = (targetHoverMultiplier > 1.01f) ? hoverColor : normalColor;
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.unscaledDeltaTime * hoverTransitionSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetHoverMultiplier = hoverScaleMultiplier;
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetHoverMultiplier = 1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        }
    }

    void OnDisable()
    {
        targetHoverMultiplier = 1f;
        currentHoverMultiplier = 1f;

        if (buttonImage != null) buttonImage.color = normalColor;
        if (originalScale != Vector3.zero)
        {
            transform.localScale = originalScale;
        }
    }
}