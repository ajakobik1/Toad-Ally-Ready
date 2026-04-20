using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Sound clips - Menu and General")]
    public AudioClip musicClip;
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public AudioClip frogClickSound;

    [Header("Sound clips - Main scene")]
    public AudioClip popupSound;       
    public AudioClip dialogSound;     
    public AudioClip taskUpdateSound;  
    public AudioClip letterRunSound;   

    private bool isMuted = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (isMuted || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // Mute button functionality
    public void ToggleMute()
    {
        isMuted = !isMuted;
        musicSource.mute = isMuted;
        sfxSource.mute = isMuted;
    }
}