﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager GetLevelManager { get; private set; }
    [SerializeField]
    private GameObject OnlineStatusPanel, BetPanel;
    [SerializeField]
    private GameObject MenuButtonPanel;
    [SerializeField]
    private Text OnlineStatusText, CurrentIncreaseText, CurrentBetText, NotificationText, RemainingCreditsTest;
    private GameManager _manager;
    private bool isOnline = false;
    public int Limit;
    public LimitTypes Type;
    private int timeLimit, scoreLimit, pieceLimit;
    public int opponentBet;

    private void Awake()
    {
        GetLevelManager = this;
    }

    private void Start()
    {
        BetText();
        OnlineStatusPanel.SetActive(false);
        BetPanel.SetActive(false);
        AdController.ConnectAds();
        if (!GPGController.IsAuthenticated())
            GPGController.GetGpgController.LoginToGPG();
        Limit = timeLimit = scoreLimit = pieceLimit = 30;
        if (GetLevelManager == null) GetLevelManager = this;
        SetDevicePrefs();
    }

    public void BetText()
    {
        CurrentBetText.text = "CURRENT BET: " + Bet.TotalBet.ToString();
        RemainingCreditsTest.text = ProjectConstants.userCredit.ToString();
    }

    public void BetEventText(string message)
    {
        NotificationText.text = message.ToUpper();
    }

    public void WaitingRoomClose()
    {
        GPGController.LeaveRoom();
        //Button kodu
    }

    public void OpenBetPanel()
    {
        BetPanel.SetActive(true);
    }

    public void WaitingPanelClosed()
    {
        OnlineStatusPanel.SetActive(false);
    }

    public void Call()
    {
        Bet.Call();
    }

    public void CallAndIncrease(int amount)
    {
        Bet.CallAndIncrease(amount);
    }

    public void UpdateOnlineStatusText(string text)
    {
        Debug.Log(text);
        OnlineStatusText.text = text;
    }

    public void MuteOrUnmute()
    {
        if (PlayerPrefs.GetInt("Muted") == 0)
        {
            PlayerPrefs.SetInt("Muted", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Muted", 0);
        }
        Debug.Log(PlayerPrefs.GetInt("Muted"));
    }

    private void SetDevicePrefs()
    {
        if (!PlayerPrefs.HasKey("Credit"))
        {
            PlayerPrefs.SetInt("Credit", 500);
        }
        if (!PlayerPrefs.HasKey("Muted"))
        {
            PlayerPrefs.SetInt("Muted", 0);
        }
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            PlayerPrefs.SetInt("Tutorial", 0);
        }
        if (PlayerPrefs.GetInt("Tutorial") == 0)
        {
            LoadLevel(ProjectConstants.Tutorial);
            return;
        }
        else
        {
            GPGController.UnlockAchievement(GPGSIds.achievement_welcome);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        _manager = GameManager.GetGameManager;
        if (level == 1)
        {
            _manager.Limit = Limit;
            _manager.LimitType = (int)Type;
        }
        if (!GPGController.IsAuthenticated())
            GPGController.GetGpgController.LoginToGPG();

        if(isOnline)
        {
            GPGController.GetGpgController.AssignTurns();
            _manager.SetSecondNonPlayable();
            _manager.isOnline = isOnline;
        }
    }

    public void RequestVideo()
    {
        AdController.ShowVideo(); //Button kodu
    }

    public void ShowAchievements()
    {
        GPGController.ShowAchievements(); //Button kodu
    }

    public void LoadOnlineGame(int type)
    {
        GPGController.GetGpgController.CreateOrJoinQuickMatch(type); //Button kodu
        OnlineStatusPanel.SetActive(true);
    }    

    public void LoadLocalGame()
    {
        isOnline = false;
        DontDestroyOnLoad(gameObject); //Button kodu
        LoadLevel(ProjectConstants.CouchPlay);
    }

    public void LoadOnlineGame()
    {
        DontDestroyOnLoad(gameObject);
        isOnline = true;
        LoadLevel(ProjectConstants.CouchPlay);
    }

    private void LoadLevel(string level)
    {
        AdController.DestroyAds(); //Button kodu
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void SetLimit(int value)
    {
        if (Limit + value > 0)
            Limit += value;
        int type = (int)Type;
        switch (type)
        {
            case 0:
                Type = LimitTypes.Charge;
                pieceLimit = Limit;
                break;
            case 1:
                Type = LimitTypes.Time;
                timeLimit = Limit;
                break;
            case 2:
                Type = LimitTypes.Score;
                scoreLimit = Limit;
                break;
            default:
                break;
        }
    }

    public void SetType(int type)
    {
        switch (type)
        {
            case 0:
                Type = LimitTypes.Charge;
                Limit = pieceLimit;
                break;
            case 1:
                Type = LimitTypes.Time;
                Limit = timeLimit;
                break;
            case 2:
                Type = LimitTypes.Score;
                Limit = scoreLimit;
                break;
            default:
                break;
        }
    }

    public void LoadLevelWithoutBanner(string level)
    {
        SceneManager.LoadSceneAsync(level);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
