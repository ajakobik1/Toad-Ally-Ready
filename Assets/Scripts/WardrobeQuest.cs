using UnityEngine;
using TMPro;

public class WardrobeQuest : MonoBehaviour
{
    private bool isNearWardrobe = false;
    private bool taskDone = false;

    private TongueQuest cachedTongueQuest;
    private FrogStatus cachedFrogStatus;

    void Start()
    {
        cachedTongueQuest = GetComponent<TongueQuest>();
        cachedFrogStatus = GetComponent<FrogStatus>();
    }

    void Update()
    {
        if (isNearWardrobe && Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        if (!taskDone)
        {
            taskDone = true;

            if (cachedTongueQuest != null)
            {
                cachedTongueQuest.ShowDialog("Wait a minute... I'm a frog. I don't wear pants!", 4f);
                cachedTongueQuest.HideInteractUI();
            }

            if (cachedFrogStatus != null)
            {
                cachedFrogStatus.TaskComplete_PutOnPants();
            }
        }
        else
        {
            if (cachedTongueQuest != null)
            {
                cachedTongueQuest.ShowDialog("Just a bunch of empty hangers.", 2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wardrobe"))
        {
            isNearWardrobe = true;
            if (cachedTongueQuest != null)
            {
                cachedTongueQuest.ShowInteractUI(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wardrobe"))
        {
            isNearWardrobe = false;
            if (cachedTongueQuest != null)
            {
                cachedTongueQuest.HideInteractUI();
            }
        }
    }
}