using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public static LevelManager GetLevelManager { get; private set; }
    [SerializeField]
    private GameObject SplashScreen;
    [SerializeField]
    private GameObject MenuButtonPanel;
    [SerializeField]
    private Text UsernameText;
    private GameManager _manager;
    public int Limit;
    public LimitTypes Type;
    private int timeLimit, scoreLimit, pieceLimit;

    private void Awake()
    {
        GetLevelManager = this;
    }

    private void Start()
    {
        AdController.ConnectAds();
        if (!GPGController.IsAuthenticated())
            GPGController.LoginToGPG();
        Limit = timeLimit = scoreLimit = pieceLimit = 30;
        if (GetLevelManager == null) GetLevelManager = this;
        SetDevicePrefs();
    }

    public void UpdateUsernameText(string name)
    {
        UsernameText.text = name;
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
            GPGController.LoginToGPG();
        UsernameText.text = GPGController.GetUsername();
    }

    public void RequestVideo()
    {
        AdController.ShowVideo();
    }

    public void ShowAchievements()
    {
        GPGController.ShowAchievements();
    }

    public void LoadLocalGame()
    {
        DontDestroyOnLoad(gameObject);
        LoadLevel(ProjectConstants.CouchPlay);
    }

    private void LoadLevel(string level)
    {
        AdController.DestroyAds();
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
