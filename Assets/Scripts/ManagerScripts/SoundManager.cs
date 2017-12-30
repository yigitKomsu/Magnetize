using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField]
    private AudioSource LoopingPlayer, OneShotPlayer;
    [SerializeField]
    private AudioClip NumberPickup, NumberDrop, ScoreSound;
    [SerializeField]
    private AudioClip LoopingTimer;

    public static SoundManager Manager;

    private void Awake()
    {
        Manager = this;
    }

    public void PickNumber()
    {
        OneShotPlayer.PlayOneShot(NumberPickup);
    }

    public void DropNumber()
    {
        OneShotPlayer.PlayOneShot(NumberDrop);
    }

    public void PlayScore()
    {
        OneShotPlayer.PlayOneShot(ScoreSound);
    }

    public void StartTimer()
    {
        LoopingPlayer.clip = LoopingTimer;
        LoopingPlayer.Play();
    }

    public void StopLoop()
    {
        LoopingPlayer.Stop();
    }
}
