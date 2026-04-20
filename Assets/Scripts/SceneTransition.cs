using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;

    [Header("Curtain Settings")]
    public Image transitionImage; 
    public float transitionSpeed = 1.2f;

    void Awake()
    {
        if (transitionImage != null) transitionImage.gameObject.SetActive(true);
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (transitionImage != null)
        {
            StartCoroutine(RevealScene());
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(HideSceneAndLoad(sceneName));
    }

    IEnumerator RevealScene()
    {
        transitionImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        transitionImage.fillAmount = 1f;

        float time = 0;
        while (time < 1f)
        {
            time += Time.unscaledDeltaTime * transitionSpeed;
            transitionImage.fillAmount = Mathf.Lerp(1f, 0f, time);
            yield return null;
        }
        transitionImage.fillAmount = 0f; 
    }

    IEnumerator HideSceneAndLoad(string sceneName)
    {
        transitionImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        transitionImage.fillAmount = 0f; 

        float time = 0;
        while (time < 1f)
        {
            time += Time.unscaledDeltaTime * transitionSpeed;
            transitionImage.fillAmount = Mathf.Lerp(0f, 1f, time);
            yield return null;
        }
        transitionImage.fillAmount = 1f; 

        SceneManager.LoadScene(sceneName);
    }
}