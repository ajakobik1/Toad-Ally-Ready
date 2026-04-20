using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FrogStatus : MonoBehaviour
{
    [Header("Checkboxes")]
    public Image tongueCheckbox;
    public Image breakfastCheckbox;
    public Image pantsCheckbox;
    public Image mailCheckbox;

    [Header("Mail Task to Unlock")]
    public GameObject mailTextObject;
    public GameObject mailCheckboxObject;

    [Header("Sprites")]
    public Sprite emptyBoxSprite;
    public Sprite checkedBoxSprite;

    public bool isTongueDone = false;
    public bool isBreakfastDone = false;
    public bool isPantsDone = false;
    public bool isMailDone = false;

    void Start()
    {
        if (tongueCheckbox != null) tongueCheckbox.sprite = emptyBoxSprite;
        if (breakfastCheckbox != null) breakfastCheckbox.sprite = emptyBoxSprite;
        if (pantsCheckbox != null) pantsCheckbox.sprite = emptyBoxSprite;
        if (mailCheckbox != null) mailCheckbox.sprite = emptyBoxSprite;

        if (mailTextObject != null) mailTextObject.SetActive(false);
        if (mailCheckboxObject != null) mailCheckboxObject.SetActive(false);
    }

    private void PlayTaskSound()
    {
        if (AudioManager.instance != null && AudioManager.instance.taskUpdateSound != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.taskUpdateSound);
        }
    }

    public void UnlockMailTask()
    {
        if (mailTextObject != null)
        {
            mailTextObject.SetActive(true);
            StartCoroutine(JuicyPop(mailTextObject.transform));
        }

        if (mailCheckboxObject != null)
        {
            mailCheckboxObject.SetActive(true);
            StartCoroutine(JuicyPop(mailCheckboxObject.transform));
        }

        PlayTaskSound();
    }
    private void UpdateCheckboxUI(Image checkbox)
    {
        if (checkbox != null)
        {
            checkbox.sprite = checkedBoxSprite;
            StartCoroutine(JuicyPop(checkbox.transform));
        }
    }

    public void TaskComplete_ScrubTongue()
    {
        if (isTongueDone) return; 
        isTongueDone = true;
        UpdateCheckboxUI(tongueCheckbox);
        PlayTaskSound();
    }

    public void TaskComplete_EatBreakfast()
    {
        if (isBreakfastDone) return;
        isBreakfastDone = true;
        UpdateCheckboxUI(breakfastCheckbox);
        PlayTaskSound();
    }

    public void TaskComplete_PutOnPants()
    {
        if (isPantsDone) return;
        isPantsDone = true;
        UpdateCheckboxUI(pantsCheckbox);
        PlayTaskSound();
    }

    public void TaskComplete_CheckMail()
    {
        if (isMailDone) return;
        isMailDone = true;
        UpdateCheckboxUI(mailCheckbox);
        PlayTaskSound();
    }

    public bool AreAllTasksDone()
    {
        return isTongueDone && isBreakfastDone && isPantsDone && isMailDone;
    }

    IEnumerator JuicyPop(Transform objTransform)
    {
        Vector3 originalScale = objTransform.localScale;
        Vector3 targetScale = originalScale * 1.6f;
        float animSpeed = 0.15f;

        float time = 0;
        while (time < animSpeed)
        {
            time += Time.deltaTime;
            objTransform.localScale = Vector3.Lerp(originalScale, targetScale, time / animSpeed);
            yield return null;
        }

        time = 0;
        while (time < animSpeed)
        {
            time += Time.deltaTime;
            objTransform.localScale = Vector3.Lerp(targetScale, originalScale, time / animSpeed);
            yield return null;
        }

        objTransform.localScale = originalScale;
    }
}