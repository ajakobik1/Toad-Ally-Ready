using UnityEngine;
using UnityEngine.UI; 

public class AudioToggleSprite : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite speakerOn;
    public Sprite speakerOff;

    [Header("Default Settings")]
    public bool isMutedAtStart = false; 

    private Image buttonImage;
    private bool isMuted;

    void Start()
    {
        buttonImage = GetComponent<Image>();

        isMuted = isMutedAtStart;
        UpdateIcon(); 
    }

    public void ToggleIcon()
    {
        isMuted = !isMuted; 
        UpdateIcon(); 
    }

    private void UpdateIcon()
    {
        if (buttonImage != null && speakerOn != null && speakerOff != null)
        {
            buttonImage.sprite = isMuted ? speakerOff : speakerOn;
        }
    }
}