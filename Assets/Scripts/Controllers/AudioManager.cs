using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip clickClip;
    public AudioClip StartClip;
    public AudioClip explodeClip;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        PlayBGM();
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlayButtonClick();
        }
    }
    private void PlayBGM()
    {
        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void PlayButtonClick()
    {
        sfxSource.PlayOneShot(clickClip);
    }
    public void PlayExplodeSFX()
    {
        sfxSource.PlayOneShot(explodeClip);
    }
}
