using UnityEngine;

public class MailQuest : MonoBehaviour
{
    private bool isNearLetter = false;
    private GameObject currentLetter;
    private bool taskDone = false;
    private bool isEscaping = false;

    private bool uiShown = false;

    private TongueQuest tongueQuest;
    private BreakfastQuest breakfastQuest;

    void Start()
    {
        tongueQuest = GetComponent<TongueQuest>();
        breakfastQuest = GetComponent<BreakfastQuest>();
    }

    void Update()
    {
        if (isEscaping && currentLetter == null && !taskDone)
        {
            taskDone = true;
            tongueQuest.ShowDialog("A water bill for the indoor pond... I'm not paying this.", 5f);
            GetComponent<FrogStatus>().TaskComplete_CheckMail();
        }

        if (isNearLetter && !taskDone && !isEscaping && !uiShown)
        {
            tongueQuest.ShowInteractUI(currentLetter);
            uiShown = true;
        }

        if (isNearLetter && Input.GetKeyDown(KeyCode.E) && !taskDone && !isEscaping)
        {
            if (!breakfastQuest.tongueAttackUnlocked)
            {
                tongueQuest.ShowDialog("Just some boring mail. I'll read it after breakfast.", 3f);
                tongueQuest.HideInteractUI();
                uiShown = false;
                return;
            }

            isEscaping = true;
            uiShown = false;

            LetterEscape escapeScript = currentLetter.GetComponent<LetterEscape>();
            if (escapeScript != null)
            {
                escapeScript.enabled = true;
                escapeScript.StartRunning();
            }

            tongueQuest.ShowDialog("I HAVE SUCH BAD LUCK TODAY! EVEN PAPER RUNS AWAY!", 5f);
            tongueQuest.HideInteractUI();
            isNearLetter = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Letter") && !taskDone && !isEscaping)
        {
            isNearLetter = true;
            currentLetter = collision.gameObject;
            uiShown = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Letter"))
        {
            isNearLetter = false;
            uiShown = false;
            if (!isEscaping) tongueQuest.HideInteractUI();
        }
    }
}