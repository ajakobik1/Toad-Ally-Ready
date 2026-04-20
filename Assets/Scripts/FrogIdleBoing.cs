using UnityEngine;
using System.Collections;

public class FrogIdleBoing : MonoBehaviour
{
    [Header("Idle settings")]
    public float idleSpeed = 5f;
    public float idleAmount = 0.05f;

    [Header("Talking settings")]
    public float talkSpeed = 25f;
    public float talkAmount = 0.08f;
    public float spriteSwapRate = 0.1f;

    [Header("Walking settings")]
    public float walkSpeed = 15f;        
    public float walkAmount = 0.08f;       
    public float walkSpriteSwapRate = 0.15f; 

    [Header("Tongue Attack")]
    public Sprite tongueSprite1;
    public Sprite tongueSprite2;
    public GameObject tongueHitbox;

    [Header("Frog Graphics")]
    public Sprite normalSprite;
    public Sprite talkSprite1;
    public Sprite talkSprite2;
    public Sprite walkSprite1;
    public Sprite walkSprite2; 

    private Vector3 originalScale;
    private SpriteRenderer rend;
    private TongueQuest tongueQuest;
    private BreakfastQuest breakfastQuest;

    private float talkTimer = 0f;
    private float walkTimer = 0f; 
    private bool isAttacking = false;

    void Start()
    {
        originalScale = transform.localScale;
        rend = GetComponent<SpriteRenderer>();
        tongueQuest = GetComponent<TongueQuest>();
        breakfastQuest = GetComponent<BreakfastQuest>();
    }

    void Update()
    {
        if (tongueQuest != null && tongueQuest.isMinigameActive) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Tongue hitbox adjustment based on facing direction
        AdjustTongueHitbox();

        if (breakfastQuest != null && breakfastQuest.tongueAttackUnlocked && Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            StartCoroutine(TongueAttackSequence());
        }

        if (isAttacking) return;

        bool isMoving = (moveX != 0 || moveY != 0);
        bool isTalking = tongueQuest != null && tongueQuest.IsTalking();

        if (isTalking) ApplySquashAndStretch(talkSpeed, talkAmount);
        else if (isMoving) ApplySquashAndStretch(walkSpeed, walkAmount);
        else ApplySquashAndStretch(idleSpeed, idleAmount);

        // Sprite swapping logic
        HandleSpriteSwapping(isTalking, isMoving);
    }

    private void ApplySquashAndStretch(float speed, float amount)
    {
        float sine = Mathf.Abs(Mathf.Sin(Time.time * speed));
        float squashFactorY = 1.0f - (sine * amount);
        float stretchFactorX = 1.0f + (sine * amount);
        transform.localScale = new Vector3(originalScale.x * stretchFactorX, originalScale.y * squashFactorY, originalScale.z);
    }

    private void HandleSpriteSwapping(bool isTalking, bool isMoving)
    {
        if (isTalking)
        {
            talkTimer += Time.deltaTime;
            if (talkTimer >= spriteSwapRate)
            {
                talkTimer = 0f;
                rend.sprite = (rend.sprite == talkSprite1) ? talkSprite2 : talkSprite1;
            }
        }
        else if (isMoving)
        {
            walkTimer += Time.deltaTime;
            if (walkTimer >= walkSpriteSwapRate)
            {
                walkTimer = 0f;
                rend.sprite = (rend.sprite == walkSprite1) ? walkSprite2 : walkSprite1;
            }
        }
        else
        {
            if (rend.sprite != normalSprite) rend.sprite = normalSprite;
            talkTimer = 0f;
            walkTimer = 0f;
        }
    }

    private void AdjustTongueHitbox()
    {
        if (tongueHitbox == null) return;

        BoxCollider2D col = tongueHitbox.GetComponent<BoxCollider2D>();
        Vector3 hitboxPos = tongueHitbox.transform.localPosition;
        Vector2 hitboxOffset = col != null ? col.offset : Vector2.zero;

        int directionMultiplier = rend.flipX ? 1 : -1;

        if (hitboxPos.x != 0) hitboxPos.x = Mathf.Abs(hitboxPos.x) * directionMultiplier;
        if (hitboxOffset.x != 0) hitboxOffset.x = Mathf.Abs(hitboxOffset.x) * directionMultiplier;

        tongueHitbox.transform.localPosition = hitboxPos;
        if (col != null) col.offset = hitboxOffset;
    }

    IEnumerator TongueAttackSequence()
    {
        isAttacking = true;
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        yield return new WaitForSeconds(0.15f);
        transform.localScale = new Vector3(originalScale.x * 0.9f, originalScale.y * 1.3f, originalScale.z);
        rend.sprite = tongueSprite1;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(originalScale.x * 1.15f, originalScale.y * 1.15f, originalScale.z);
        rend.sprite = tongueSprite2;
        if (tongueHitbox != null) tongueHitbox.SetActive(true);
        yield return new WaitForSeconds(0.25f);

        if (tongueHitbox != null) tongueHitbox.SetActive(false);
        rend.sprite = normalSprite;
        transform.localScale = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z);

        isAttacking = false;
    }
}