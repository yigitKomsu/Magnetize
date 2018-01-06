using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource LoopingPlayer, OneShotPlayer;
    [SerializeField]
    private AudioClip NumberPickup, NumberDrop, ScoreSound;
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
