using UnityEngine;

public class FlyEscape : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 5f;
    public Vector2 flightDirection = new Vector2(1f, 1f); 

    [Header("Squash & Stretch")]
    public float boingSpeed = 35f; 
    public float boingAmount = 0.1f;

    [Header("Animation Frames")]
    public Sprite frame1;
    public Sprite frame2;
    public float animationSpeed = 0.05f; 

    private bool isEscaping = false;
    private SpriteRenderer rend;
    private Vector3 originalScale;
    private float timer = 0f;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }
    public void FlyAway()
    {
        isEscaping = true;
        Destroy(gameObject, 6f); 
    }

    void Update()
    {
        if (!isEscaping) return; 

        transform.Translate(flightDirection.normalized * speed * Time.deltaTime);

        float sine = Mathf.Abs(Mathf.Sin(Time.time * boingSpeed));
        float squashFactorY = 1.0f - (sine * boingAmount);
        float stretchFactorX = 1.0f + (sine * boingAmount);
        transform.localScale = new Vector3(originalScale.x * stretchFactorX, originalScale.y * squashFactorY, originalScale.z);

        timer += Time.deltaTime;
        if (timer >= animationSpeed)
        {
            timer = 0f;
            rend.sprite = (rend.sprite == frame1) ? frame2 : frame1;
        }
    }
}