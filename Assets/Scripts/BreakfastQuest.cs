using UnityEngine;

public class BreakfastQuest : MonoBehaviour
{
    [Header("Objects")]
    public GameObject dessertFly;

    [Header("Swarm and Flies")]
    public GameObject swarmAtPlant;
    public GameObject swarmAtPond;
    public GameObject[] realFlies;

    public bool tongueAttackUnlocked = false;

    private bool isNearFridge = false;
    private bool isNearPlant = false;
    private bool isNearPond = false;
    private GameObject currentObject;
    private bool hasCheckedFridge = false;
    public int questStage = 0;
    private int caughtFlies = 0;
    private TongueQuest tongueQuest;

    void Start() { tongueQuest = GetComponent<TongueQuest>(); }


    // Main interaction logic
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isNearFridge)
            {
                if (!hasCheckedFridge)
                {
                    hasCheckedFridge = true;
                    tongueQuest.ShowDialog("Oh no, my prepared meal! It's flying away! I need to find something elses now...", 6f);
                    if (dessertFly != null) { dessertFly.SetActive(true); dessertFly.GetComponent<FlyEscape>().FlyAway(); }
                    tongueQuest.HideInteractUI();
                }
                else { tongueQuest.ShowDialog("The fridge is empty. Just a sad puddle of water now.", 4f); }
            }
            else if (isNearPlant)
            {
                if (!hasCheckedFridge) { tongueQuest.ShowDialog("My personal fly farm. I'm saving them for later.", 4f); }
                else if (questStage == 0)
                {
                    questStage = 1;
                    tongueQuest.ShowDialog("Time for breakfast! Wait, come back here!", 4f);
                    swarmAtPlant.SetActive(false);
                    swarmAtPond.SetActive(true);
                    tongueQuest.HideInteractUI();
                }
                else { tongueQuest.ShowDialog("No more flies for me today.", 3f); }
            }
            else if (isNearPond && questStage == 1)
            {
                questStage = 2;
                tongueAttackUnlocked = true;
                swarmAtPond.SetActive(false);
                foreach (GameObject fly in realFlies) { fly.SetActive(true); }
                tongueQuest.ShowDialog("They scattered! I need to use my TONGUE to catch them! (Press SPACE)", 6f);
                tongueQuest.ClearTrigger();
                GetComponent<FrogStatus>().UnlockMailTask();
            }
        }
    }

    private void HideE()
    {
        isNearFridge = false; isNearPlant = false; isNearPond = false;
        if (tongueQuest != null) tongueQuest.HideInteractUI();
    }


    // Interactions with kitchen objects
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fridge"))
        {
            isNearFridge = true; currentObject = collision.gameObject;
            if (tongueQuest != null) tongueQuest.ShowInteractUI(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("FlyPlant"))
        {
            isNearPlant = true; currentObject = collision.gameObject;
            if (tongueQuest != null) tongueQuest.ShowInteractUI(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Pond"))
        {
            isNearPond = true; currentObject = collision.gameObject;
            if (tongueQuest != null) tongueQuest.ShowInteractUI(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fridge") || collision.gameObject.CompareTag("FlyPlant") || collision.gameObject.CompareTag("Pond")) HideE();
    }



    // Fly catching minigame
    private static readonly string[] numberWords = { "zero", "one", "two", "three", "four", "five" };
    public void CatchFly()
    {
        caughtFlies++;
        string caughtWord = numberWords[caughtFlies];
        string totalWord = numberWords[realFlies.Length];

        if (caughtFlies >= realFlies.Length)
        {
            tongueQuest.ShowDialog("FULL TUMMY! That was delicious.", 5f);
            GetComponent<FrogStatus>().TaskComplete_EatBreakfast();
        }
        else tongueQuest.ShowDialog("Yum! " + caughtWord + " out of " + totalWord + " flies caught.", 2f);
    }
}