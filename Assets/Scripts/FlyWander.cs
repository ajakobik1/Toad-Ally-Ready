using UnityEngine;

public class FlyWander : MonoBehaviour
{
    [Header("Flight settings")]
    public float speed = 3f;
    public float wanderRadius = 2.5f;

    [Header("Graphics")]
    public Sprite frame1;
    public Sprite frame2;
    public float animationSpeed = 0.05f;

    [Header("Animation")]
    public float boingSpeed = 30f;
    public float boingAmount = 0.15f;

    private SpriteRenderer rend;
    private Rigidbody2D rb; 
    private Vector3 originalScale;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float talkTimer = 0f;
    private float randomTimeOffset;
    private float stuckTimer = 0f;
    private BreakfastQuest breakfastQuest;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        breakfastQuest = FindFirstObjectByType<BreakfastQuest>();

        originalScale = transform.localScale;
        startPos = transform.position;
        randomTimeOffset = Random.Range(0f, 100f);
        targetPos = startPos + (Vector3)Random.insideUnitCircle * wanderRadius;
    }

    void FixedUpdate()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        stuckTimer += Time.fixedDeltaTime; 

        if (Vector2.Distance(rb.position, targetPos) < 0.2f || stuckTimer > 2f)
        {
            targetPos = startPos + (Vector3)Random.insideUnitCircle * wanderRadius;
            stuckTimer = 0f; 
        }
    }
    void Update()
    {
        float timeWithOffset = Time.time + randomTimeOffset;
        float sine = Mathf.Abs(Mathf.Sin(timeWithOffset * boingSpeed));
        float squashFactorY = 1.0f - (sine * boingAmount);
        float stretchFactorX = 1.0f + (sine * boingAmount);
        transform.localScale = new Vector3(originalScale.x * stretchFactorX, originalScale.y * squashFactorY, originalScale.z);

        if (rend != null)
        {
            rend.color = Color.Lerp(Color.white, Color.white * 1.5f, Mathf.PingPong(Time.time * 6f, 1f));
        }
        talkTimer += Time.deltaTime;
        if (talkTimer >= animationSpeed)
        {
            talkTimer = 0f;
            if (rend != null) rend.sprite = (rend.sprite == frame1) ? frame2 : frame1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tongue"))
        {
            if (breakfastQuest != null) breakfastQuest.CatchFly();
            Destroy(gameObject);
        }
    }
}