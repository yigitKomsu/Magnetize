using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject SplashScreen;
    [SerializeField]
    private GameObject MenuButtonPanel;
    private GameManager _manager;
    public int Limit;
    public LimitTypes Type;
    private BannerView bannerView;

    private int timeLimit, scoreLimit, pieceLimit;

    public static LevelManager Manager;


    private void Awake()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-9092467960609637~7439984559";
#endif
        MobileAds.Initialize(appId);
        RequestBanner();
        bannerView.OnAdLoaded += BannerView_OnAdLoaded;
    }

    private void Start()
    {
        Limit = timeLimit = scoreLimit = pieceLimit = 30;
        if (Manager == null) Manager = this;
        //StartCoroutine(Load());
    }

    private void OnLevelWasLoaded(int level)
    {
        _manager = GameManager.Manager;
        if (level == 1)
        {
            _manager.Limit = Limit;
            _manager.LimitType = (int)Type;
        }
        else if (level == 0)
        {
            SplashScreen.SetActive(false);
        }
    }

    private void BannerView_OnAdLoaded(object sender, EventArgs e)
    {
        bannerView.IsLoaded = true;
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        //string adUnitId = ProjectConstants.UnityAdId;
        string adUnitId = ProjectConstants.TestAdId; //ALWAYS test with TEST apps
#endif
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }

    public void LoadLevel()
    {
        DontDestroyOnLoad(gameObject);
        LoadLevel(ProjectConstants.CouchPlay);
    }

    private void LoadLevel(string level)
    {
        bannerView.Destroy();
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void SetLimit(int value)
    {
        if(Limit + value > 0)
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

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(2);
        
        SplashScreen.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
