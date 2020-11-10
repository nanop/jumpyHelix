using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using OnefallGames;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager Instance { private set; get; }

    //Gameplay UI
    [SerializeField] private GameObject gameplayUI = null;
    [SerializeField] private Text levelTxt = null;
    [SerializeField] private GameObject pauseBtn = null;
    [SerializeField] private GameObject unPauseBtn = null;
    [SerializeField] private Image timeBar = null;


    //Revive UI
    [SerializeField] private GameObject reviveUI = null;
    [SerializeField] private Image reviveCoverImg = null;

    //GameOver UI
    [SerializeField] private GameObject endGameUI = null;
    [SerializeField] private GameObject starCoverUI = null;
    [SerializeField] private GameObject starUI = null;
    [SerializeField] private GameObject star_1 = null;
    [SerializeField] private GameObject star_2 = null;
    [SerializeField] private GameObject star_3 = null;
    [SerializeField] private GameObject playBtns = null;
    [SerializeField] private GameObject playBtn = null;
    [SerializeField] private GameObject nextBtn = null;
    [SerializeField] private GameObject restartBtn = null;
    [SerializeField] private GameObject shareBtn = null;
    [SerializeField] private GameObject soundOnBtn = null;
    [SerializeField] private GameObject soundOffBtn = null;
    [SerializeField] private GameObject musicOnBtn = null;
    [SerializeField] private GameObject musicOffBtn = null;
    [SerializeField] private Image fadingPanel = null;
    [SerializeField] private LeaderboardViewController leaderboardViewController = null;

    //References
    [SerializeField] private AnimationClip servicesBtns_Show = null;
    [SerializeField] private AnimationClip servicesBtns_Hide = null;
    [SerializeField] private AnimationClip settingBtns_Hide = null;
    [SerializeField] private AnimationClip settingBtns_Show = null;
    [SerializeField] private Animator settingAnim = null;
    [SerializeField] private Animator servicesAnim = null;



    private float timeCount = 0;

    private void OnEnable()
    {
        GameManager.GameStateChanged += GameManager_GameStateChanged;
    }


    private void OnDisable()
    {
        GameManager.GameStateChanged -= GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(GameState obj)
    {
        if (obj == GameState.GameOver)
        {
            StartCoroutine(ShowGameOverUI(0.5f));
        }
        else if (obj == GameState.PassLevel)
        {
            StartCoroutine(ShowPassLevelUI(0.5f));
        }
        else if (obj == GameState.Playing)
        {
            if (!GameManager.Instance.IsRevived)
            {
                gameplayUI.SetActive(true);
                unPauseBtn.SetActive(false);
                endGameUI.SetActive(false);
                reviveUI.SetActive(false);
                StartCoroutine(CountingDownTimeBar());
            }          
        }
        else if (obj == GameState.Revive)
        {
            StartCoroutine(ShowReviveUI(0.5f));
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }




    private void Start () 
    {

        if (!GameManager.IsRestart) //This is the first load
        {
            gameplayUI.SetActive(false);
            reviveUI.SetActive(false);
            endGameUI.SetActive(true);

            starCoverUI.SetActive(false);
            starUI.SetActive(false);
            restartBtn.SetActive(false);
            nextBtn.SetActive(false);
            playBtn.SetActive(true);
            shareBtn.SetActive(false);
        }

        leaderboardViewController.gameObject.SetActive(false);
    }
	
	private void Update () 
    {

        UpdateMusicButtons();
        UpdateMuteButtons();
	}


    ////////////////////////////Publish functions
    public void PlayButtonSound()
    {
        ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.button);
    }

    public void PauseBtn()
    {
        pauseBtn.SetActive(false);
        unPauseBtn.SetActive(true);
        GameManager.Instance.PauseGame();
    }
    public void UnPauseBtn()
    {
        pauseBtn.SetActive(true);
        unPauseBtn.SetActive(false);
        GameManager.Instance.PlayingGame();
    }
    public void PlayBtn()
    {
        GameManager.Instance.PlayingGame();
    }
    public void RestartBtn()
    {
        if (GameManager.Instance.GameState == GameState.PassLevel)
            GameManager.Instance.DecreaseCurrentLevel();
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 0.5f);
    }
    public void NextBtn()
    {
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 0.5f);
    }



    public void NativeShareBtn()
    {
        ServicesManager.Instance.ShareManager.NativeShare();
    }
    public void RateAppBtn()
    {
#if UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#elif UNITY_ANDROID
        Application.OpenURL(ServicesManager.Instance.ShareManager.AppUrl);
#endif
    }
    public void SettingBtn()
    {
        servicesAnim.Play(servicesBtns_Hide.name);
        settingAnim.Play(settingBtns_Show.name);
    }
    public void ToggleSound()
    {
        ServicesManager.Instance.SoundManager.ToggleMute();
    }

    public void ToggleMusic()
    {
        ServicesManager.Instance.SoundManager.ToggleMusic();
    }

    public void LeaderboardBtn()
    {
        leaderboardViewController.gameObject.SetActive(true);
        leaderboardViewController.OnShow();
    }

    public void BackBtn()
    {
        settingAnim.Play(settingBtns_Hide.name);
        servicesAnim.Play(servicesBtns_Show.name);
    }

    public void ReviveBtn()
    {
        reviveUI.SetActive(false);
        ServicesManager.Instance.AdManager.ShowRewardedVideoAd();
    }

    public void SkipBtn()
    {
        reviveUI.SetActive(false);
        GameManager.Instance.GameOver();
    }



    /////////////////////////////Private functions
    private void UpdateMuteButtons()
    {
        if (ServicesManager.Instance.SoundManager.IsMuted())
        {
            soundOnBtn.gameObject.SetActive(false);
            soundOffBtn.gameObject.SetActive(true);
        }
        else
        {
            soundOnBtn.gameObject.SetActive(true);
            soundOffBtn.gameObject.SetActive(false);
        }
    }


    private void UpdateMusicButtons()
    {
        if (ServicesManager.Instance.SoundManager.IsMusicOff())
        {
            musicOffBtn.gameObject.SetActive(true);
            musicOnBtn.gameObject.SetActive(false);
        }
        else
        {
            musicOffBtn.gameObject.SetActive(false);
            musicOnBtn.gameObject.SetActive(true);
        }
    }


    private IEnumerator ShowGameOverUI(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameplayUI.SetActive(false);
        endGameUI.SetActive(true);

        starCoverUI.SetActive(true);
        starUI.SetActive(false);
        shareBtn.SetActive(true);
        playBtns.SetActive(false);
        restartBtn.SetActive(true);
    }
    private IEnumerator ShowPassLevelUI(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameplayUI.SetActive(false);
        endGameUI.SetActive(true);

        starCoverUI.SetActive(true);
        starUI.SetActive(true);
        shareBtn.SetActive(true);
        playBtns.SetActive(true);
        playBtn.SetActive(false);
        nextBtn.SetActive(true);
        restartBtn.SetActive(true);

        star_1.SetActive(false);
        star_2.SetActive(false);
        star_3.SetActive(false);

        float timeUse = GameManager.Instance.TimeToPassLevel - timeCount;
        float percent = (timeUse / GameManager.Instance.TimeToPassLevel) * 100f;

        float delayTime = 0.5f;
        if (percent >= GameManager.Instance.ThreeStarTime) //Show three stars
        {
            yield return new WaitForSeconds(delayTime);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.starCount);
            star_1.SetActive(true);
            yield return new WaitForSeconds(delayTime);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.starCount);
            star_2.SetActive(true);
            yield return new WaitForSeconds(delayTime);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.starCount);
            star_3.SetActive(true);
        }
        else if (percent >= GameManager.Instance.TwoStarTime && percent < GameManager.Instance.ThreeStarTime) //Show two stars
        {
            yield return new WaitForSeconds(delayTime);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.starCount);
            star_1.SetActive(true);
            yield return new WaitForSeconds(delayTime);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.starCount);
            star_2.SetActive(true);
            star_3.SetActive(false);
        }
        else //Show one star 
        {
            yield return new WaitForSeconds(delayTime);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.starCount);
            star_1.SetActive(true);
            star_2.SetActive(false);
            star_3.SetActive(false);
        }
    }

    private IEnumerator ShowReviveUI(float delay)
    {
        yield return new WaitForSeconds(delay);

        reviveUI.SetActive(true);
        StartCoroutine(ReviveCountDown());
    }

    private IEnumerator ReviveCountDown()
    {
        float t = 0;
        while (t < GameManager.Instance.ReviveWaitTime)
        {
            if (!reviveUI.activeInHierarchy)
                yield break;
            t += Time.deltaTime;
            float factor = t / GameManager.Instance.ReviveWaitTime;
            reviveCoverImg.fillAmount = Mathf.Lerp(1, 0, factor);
            yield return null;
        }
        reviveUI.SetActive(false);
        GameManager.Instance.GameOver();
    }
    private IEnumerator CountingDownTimeBar()
    {
        //Wait for finished fading
        while (!GameManager.Instance.IsFinishedFading)
        {
            yield return null;
        }

        timeCount = 0;
        while (timeCount < GameManager.Instance.TimeToPassLevel)
        {
            timeCount += Time.deltaTime;
            float factor = timeCount / GameManager.Instance.TimeToPassLevel;
            timeBar.fillAmount = Mathf.Lerp(1, 0, factor);
            yield return null;
            if (PlayerController.Instance.PlayerState == PlayerState.PassLevel)
                yield break;
            while (PlayerController.Instance.PlayerState != PlayerState.Living)
            {
                yield return null;
            }
        }
        PlayerController.Instance.PlayerDie();
        GameManager.Instance.GameOver();
    }




    /// <summary>
    /// Fading the panel out with given fadingTime
    /// </summary>
    /// <param name="fadingTime"></param>
    public void FadeOutPanel(float fadingTime)
    {
        StartCoroutine(FadingOutPanel(fadingTime));
    }
    private IEnumerator FadingOutPanel(float fadingTime)
    {
        fadingPanel.gameObject.SetActive(true);
        float t = 0;
        Color startColor = fadingPanel.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        while (t < fadingTime)
        {
            t += Time.deltaTime;
            float factor = t / fadingTime;
            fadingPanel.color = Color.Lerp(startColor, endColor, factor);
            yield return null;
        }
        fadingPanel.gameObject.SetActive(false);
    }


    /// <summary>
    /// Show level text with given level number
    /// </summary>
    /// <param name="level"></param>
    public void SetLevelTxt(int level)
    {
        levelTxt.text = "LEVEL: " + level.ToString();
    }

   
}
