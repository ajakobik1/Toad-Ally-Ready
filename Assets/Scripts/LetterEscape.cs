using UnityEngine;

public class LetterEscape : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 7f;
    public float wanderRadius = 5f;

    [Header("Flight Zone")]
    public Transform roomCenter;

    [Header("Animation")]
    public float boingSpeed = 40f;
    public float boingAmount = 0.2f;

    private Rigidbody2D rb;
    private Vector3 originalScale;
    private Vector3 targetPos;
    private float stuckTimer = 0f;

    private bool isRunningAway = false;

    private TongueQuest cachedTongueQuest;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        if (originalScale == Vector3.zero) originalScale = Vector3.one;
        cachedTongueQuest = FindFirstObjectByType<TongueQuest>();
    }

    void FixedUpdate()
    {
        if (!isRunningAway) return;

        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        stuckTimer += Time.fixedDeltaTime;

        if (Vector2.Distance(rb.position, targetPos) < 0.2f || stuckTimer > 1f)
        {
            PickNewTarget();
        }
    }

    void Update()
    {
        if (!isRunningAway) return;

        float sine = Mathf.Abs(Mathf.Sin(Time.time * boingSpeed));
        float squashFactorY = 1.0f - (sine * boingAmount);
        float stretchFactorX = 1.0f + (sine * boingAmount);
        transform.localScale = new Vector3(originalScale.x * stretchFactorX, originalScale.y * squashFactorY, originalScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tongue") && isRunningAway)
        {
            if (cachedTongueQuest != null)
            {
                cachedTongueQuest.ShowSurprisedFrog();
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall") && isRunningAway)
        {
            PickNewTarget();
        }
    }

    public void StartRunning()
    {
        isRunningAway = true;
        if (AudioManager.instance != null && AudioManager.instance.letterRunSound != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.letterRunSound);
        }

        PickNewTarget();
    }

    private void PickNewTarget()
    {
        Vector3 center = roomCenter != null ? roomCenter.position : transform.position;
        targetPos = center + (Vector3)Random.insideUnitCircle * wanderRadius;
        stuckTimer = 0f;
    }
}