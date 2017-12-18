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
    public int Limit;
    public LimitTypes Type;
    private BannerView bannerView;

    public static LevelManager Manager;

    public enum LimitTypes
    {
        Limitless = 0,
        Time = 1,
        Score = 2
    }
    
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
        if (Manager == null) Manager = this;
        StartCoroutine(Load());
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            GameManager.Manager.Limit = Limit;
            GameManager.Manager.LimitType = (int)Type;
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
        //string adUnitId = "ca-app-pub-9092467960609637/7412064397";
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //ALWAYS test with TEST apps
#endif
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public void ChangePosHorizontal(int direction)
    {
        MenuButtonPanel.GetComponent<RectTransform>().localPosition += new Vector3(1440 * direction, 0, 0);
    }

    public void ChangePosVertical(int direction)
    {
        MenuButtonPanel.GetComponent<RectTransform>().localPosition += new Vector3(0, 2560 * direction, 0);
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }

    public void SetTime()
    {
        this.Type = LimitTypes.Time;
        ChangePosVertical(-1);
    }

    public void SetScore()
    {
        this.Type = LimitTypes.Score;
        ChangePosVertical(1);
    }

    public void SetLimit(int limit)
    {
        Limit = limit;
        DontDestroyOnLoad(gameObject);
        LoadLevel("CouchPlay");
    }

    private void LoadLevel(string level)
    {
        Debug.Log("Swishhh animation");
        bannerView.Destroy();
        SceneManager.LoadScene(level, LoadSceneMode.Single);
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
