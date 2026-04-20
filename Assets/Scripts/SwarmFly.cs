using UnityEngine;

public class SwarmFly : MonoBehaviour
{
    [Header("Animation")]
    public Sprite frame1;
    public Sprite frame2;
    public float flashSpeed = 0.05f;

    [Header("Squash & Stretch")]
    public float boingSpeed = 35f;
    public float boingAmount = 0.1f;

    [Header("Flight")]
    public float flightSpeed = 10f;
    public float flightRange = 0.5f;

    private SpriteRenderer rend;
    private Vector3 originalScale;
    private Vector3 startPos;
    private float randomOffset;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        startPos = transform.position;
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float timeWithOffset = Time.time + randomOffset;

        HandleFlight(timeWithOffset);
        HandleSquashAndStretch(timeWithOffset);
        HandleSpriteAnimation();
    }

    private void HandleFlight(float time)
    {
        float moveX = Mathf.Sin(time * flightSpeed) * flightRange;
        float moveY = Mathf.Cos(time * (flightSpeed * 1.3f)) * flightRange;

        transform.position = startPos + new Vector3(moveX, moveY, 0);
    }

    private void HandleSquashAndStretch(float time)
    {
        float sineScale = Mathf.Abs(Mathf.Sin(time * boingSpeed));
        float squashFactorY = 1.0f - (sineScale * boingAmount);
        float stretchFactorX = 1.0f + (sineScale * boingAmount);

        transform.localScale = new Vector3(
            originalScale.x * stretchFactorX,
            originalScale.y * squashFactorY,
            originalScale.z
        );
    }

    private void HandleSpriteAnimation()
    {
        if (frame1 == null || frame2 == null) return;

        timer += Time.deltaTime;
        if (timer >= flashSpeed)
        {
            timer = 0f;
            rend.sprite = (rend.sprite == frame1) ? frame2 : frame1;
        }
    }
}