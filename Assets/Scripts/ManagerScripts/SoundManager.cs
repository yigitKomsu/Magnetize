using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource LoopingPlayer, OneShotPlayer;
    [SerializeField]
    private AudioClip NumberPickup, NumberDrop, ScoreSound;
    [SerializeField]
    private AudioClip Refill, Magnetize, Double;
    [SerializeField]
    private AudioClip RefillScore, MagnetizeScore, DoubleScore;
    [SerializeField]
    private AudioClip LoopingTimer;
    public static SoundManager GetSoundManager { get; private set; }

    private void Awake()
    {
        GetSoundManager = this;
    }

    public void PickNumber()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        OneShotPlayer.PlayOneShot(NumberPickup);
    }

    public void PlayRefill()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        OneShotPlayer.PlayOneShot(Refill);
    }

    public void PlayRefillScore()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        OneShotPlayer.PlayOneShot(RefillScore);
    }

    public void DropNumber()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        OneShotPlayer.PlayOneShot(NumberDrop);
    }

    public void PlayScore()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        OneShotPlayer.PlayOneShot(ScoreSound);
    }

    public void StartTimer()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        LoopingPlayer.clip = LoopingTimer;
        LoopingPlayer.Play();
    }

    public void StopLoop()
    {
        if (PlayerPrefs.GetInt("Muted") == 1) return;
        LoopingPlayer.Stop();
    }
}
