using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnefallGames;
using UnityEngine.SceneManagement;
using System.IO;

public enum GameState
{
    Prepare,
    Playing,
    Pause,
    Revive,
    PassLevel,
    GameOver,
}


[System.Serializable]
public struct LevelData
{
    public int MinLevel;
    public int MaxLevel;
    public int MinHelixNumber;
    public int MaxHelixNumber;
    public int MinDisablePiecesNumber;
    public int MaxDisablePiecesNumber;
    public int MinDeadPiecesNumber;
    public int MaxDeadPiecesNumber;
    public int MinTimeToPassLevel;
    public int MaxTimeToPassLevel;
    public Color DeadPieceColor;
    public Color NormalPieceColor;
    public Color BrokenPieceColor;
    public Color BallColor;
    public Color PillarColor;
}



public class GameManager : MonoBehaviour {

    public static GameManager Instance { private set; get; }
    public static event System.Action<GameState> GameStateChanged = delegate { };
    public static int CurrentLevel { private set; get; }
    public const string MaxPassedLevel_PPK = "MaxPassedLevel";
    public static bool IsRestart { private set; get; }


    public GameState GameState
    {
        get
        {
            return gameState;
        }
        private set
        {
            if (value != gameState)
            {
                gameState = value;
                GameStateChanged(gameState);
            }
        }
    }

    [Header("Gameplay Testing")]
    [Header("Put a level number to test that level. Set back to 0 to disable this feature.")]
    [SerializeField] private int testingLevel = 0;


    [Header("Gameplay Config")]
    [SerializeField] private float reviveWaitTime = 4f;
    [SerializeField] private Vector3 firstHelixPosition = new Vector3(0, -1f, 2.5f);
    [SerializeField] private float helixSpace = 5f;
    [SerializeField] private float fadingHelixScale = 4f;
    [SerializeField] private float fadingHelixTime = 0.5f;
    [SerializeField] private int helixPassedCountForBreak = 2;
    [SerializeField] private int threeStarPercentTime = 50;
    [SerializeField] private int twoStarPercentTime = 30;
    [SerializeField] private int oneStarPercentTime = 10;
    [SerializeField] private float uIFadingTime = 2f;
    [SerializeField] private float ballSplatFadingTime = 2f;
    [SerializeField] private int[] savedLevels = null;
    [SerializeField] private LevelData[] levelData = null;


    [Header("Gameplay References")]
    [SerializeField] private Material pillarMaterial = null;
    [SerializeField] private Material deadPieceMaterial = null;
    [SerializeField] private Material normalPieceMaterial = null;
    [SerializeField] private Material brokenPieceMaterial = null;
    [SerializeField] private Transform rotaterTrans = null;
    [SerializeField] private GameObject pillar = null;
    [SerializeField] private GameObject bottomPillar = null;
    [SerializeField] private GameObject helixPrefab = null;
    [SerializeField] private GameObject fadingHelixPrefab = null;
    [SerializeField] private GameObject ballSplatPrefab = null;
    [SerializeField] private GameObject splatShatterPrefab = null;


    public Material DeadPieceMaterial { private set; get; }
    public Material NomarPieceMaterial { private set; get; }
    public Material BrokenPieceMaterial { private set; get; }
    public float ReviveWaitTime { private set; get; }
    public int PassedCountForBreakHelix { private set; get; }
    public int TimeToPassLevel { private set; get; }
    public int ThreeStarTime { private set; get; }
    public int TwoStarTime { private set; get; }
    public int OneStarTime { private set; get; }
    public bool IsFinishedFading { private set; get; }
    public bool IsRevived { private set; get; }


    private GameState gameState = GameState.GameOver;
    private List<BallSplatController> listBallSplatControl = new List<BallSplatController>();
    private List<FadingHelixController> listFadingHelixControl = new List<FadingHelixController>();
    private List<ParticleSystem> listSplatShatter = new List<ParticleSystem>();


    void Awake()
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

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Use this for initialization
    void Start () {

        Application.targetFrameRate = 60;

        //Fire event
        GameState = GameState.Prepare;
        gameState = GameState.Prepare;

        //Add another actions here

        DeadPieceMaterial = deadPieceMaterial;
        NomarPieceMaterial = normalPieceMaterial;
        BrokenPieceMaterial = brokenPieceMaterial;
        PassedCountForBreakHelix = helixPassedCountForBreak;
        ReviveWaitTime = reviveWaitTime;
        ThreeStarTime = threeStarPercentTime;
        TwoStarTime = twoStarPercentTime;
        OneStarTime = oneStarPercentTime;
        IsRevived = false;


        //Set current level
        if (!PlayerPrefs.HasKey(MaxPassedLevel_PPK))
        {
            PlayerPrefs.SetInt(MaxPassedLevel_PPK, 1);
            CurrentLevel = 1;
        }

        if (!IsRestart)
            CurrentLevel = PlayerPrefs.GetInt(MaxPassedLevel_PPK);

        if (testingLevel != 0)
            CurrentLevel = testingLevel;

        //Show level on UI
        UIManager.Instance.SetLevelTxt(CurrentLevel);

        //Create level
        foreach (LevelData o in levelData)
        {
            if (CurrentLevel >= o.MinLevel && CurrentLevel < o.MaxLevel)
            {
                CreateLevel(o);
                break;
            }
        }


        StartCoroutine(ResetIsFinishedFadingValue());
        UIManager.Instance.FadeOutPanel(uIFadingTime);
        if (IsRestart)
            PlayingGame();
    }

    /// <summary>
    /// Actual start the game
    /// </summary>
    public void PlayingGame()
    {
        //Fire event
        GameState = GameState.Playing;
        gameState = GameState.Playing;

        //Add another actions here
    }


    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        //Fire event
        GameState = GameState.Pause;
        gameState = GameState.Pause;

        //Add another actions here
    }

    /// <summary>
    /// Call Revive event
    /// </summary>
    public void Revive()
    {
        //Fire event
        GameState = GameState.Revive;
        gameState = GameState.Revive;

        //Add another actions here
    }


    /// <summary>
    /// Call PassLevel event
    /// </summary>
    public void PassLevel()
    {
        //Fire event
        GameState = GameState.PassLevel;
        gameState = GameState.PassLevel;

        //Add another actions here
        ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.passLevel);
        ServicesManager.Instance.ShareManager.CreateScreenshot();
        IsRestart = true;

        if (testingLevel == 0) //Isn't on testing level
        {
            //Save the current level if it in savedLevels
            foreach (int o in savedLevels)
            {
                if (o == CurrentLevel)
                {
                    PlayerPrefs.SetInt(MaxPassedLevel_PPK, CurrentLevel);
                    break;
                }
            }

            //Report level to leaderboard
            if (ServicesManager.Instance.LeaderboardManager.IsSetUsername())
            {
                ServicesManager.Instance.LeaderboardManager.SetPlayerLeaderboardData();
            }
            CurrentLevel++;
        }     
    }

    /// <summary>
    /// Call GameOver event
    /// </summary>
    public void GameOver()
    {
        //Fire event
        GameState = GameState.GameOver;
        gameState = GameState.GameOver;

        //Add another actions here
        ServicesManager.Instance.ShareManager.CreateScreenshot();
        IsRestart = true;
        CurrentLevel = PlayerPrefs.GetInt(MaxPassedLevel_PPK);
    }


    public void LoadScene(string sceneName, float delay)
    {
        StartCoroutine(LoadingScene(sceneName, delay));
    }

    private IEnumerator LoadingScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ResetIsFinishedFadingValue()
    {
        IsFinishedFading = false;       
        yield return new WaitForSeconds(uIFadingTime);
        IsFinishedFading = true;
    }

    private IEnumerator PlayParticle(ParticleSystem par)
    {
        par.Play();
        yield return new WaitForSeconds(par.main.startLifetimeMultiplier);
        par.gameObject.SetActive(false);
    }


    //Get an inactive fading helix
    private FadingHelixController GetFadingHelixControl()
    {
        //Find on the list
        foreach (FadingHelixController o in listFadingHelixControl)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new one
        FadingHelixController fadingHelixControl = Instantiate(fadingHelixPrefab, Vector3.zero, Quaternion.identity).GetComponent<FadingHelixController>();
        listFadingHelixControl.Add(fadingHelixControl);
        fadingHelixControl.gameObject.SetActive(false);
        return fadingHelixControl;
    }


    //Get an inactive ballSplatControl
    private BallSplatController GetBallSplatControl()
    {
        //Find on the list
        foreach(BallSplatController o in listBallSplatControl)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new one
        BallSplatController ballSplatControl = Instantiate(ballSplatPrefab, Vector3.zero, Quaternion.identity).GetComponent<BallSplatController>();
        listBallSplatControl.Add(ballSplatControl);
        ballSplatControl.gameObject.SetActive(false);
        return ballSplatControl;
    }

    //Get an inactive splatShatter
    private ParticleSystem GetSplatShatter()
    {
        //Find on the list
        foreach (ParticleSystem o in listSplatShatter)
        {
            if (!o.gameObject.activeInHierarchy)
                return o;
        }

        //Didn't find one -> create new one
        ParticleSystem splatShatter = Instantiate(splatShatterPrefab, Vector3.zero, Quaternion.identity).GetComponent<ParticleSystem>();
        listSplatShatter.Add(splatShatter);
        splatShatter.gameObject.SetActive(false);
        return splatShatter;
    }



    private void CreateLevel(LevelData levelData)
    {
        //Random helix number
        int helixNumber = Random.Range(levelData.MinHelixNumber, levelData.MaxHelixNumber);

        //Assign values
        deadPieceMaterial.color = levelData.DeadPieceColor;
        normalPieceMaterial.color = levelData.NormalPieceColor;
        brokenPieceMaterial.color = levelData.BrokenPieceColor;
        PlayerController.Instance.SetBallColor(levelData.BallColor);
        pillarMaterial.color = levelData.PillarColor;
        TimeToPassLevel = Random.Range(levelData.MinTimeToPassLevel, levelData.MaxTimeToPassLevel);

        //Create the first helix
        HelixController firstHelixControl = Instantiate(helixPrefab, firstHelixPosition, Quaternion.identity).GetComponent<HelixController>();
        firstHelixControl.HandleHelix(Random.Range(levelData.MinDisablePiecesNumber, levelData.MaxDeadPiecesNumber), 0, levelData.NormalPieceColor, levelData.DeadPieceColor);
        firstHelixControl.transform.SetParent(rotaterTrans);

        //Calculate the height of all helixs, space and distance between the pillar and the first helix
        float oneHelixHeight = helixPrefab.transform.GetChild(0).GetComponent<Renderer>().bounds.size.y;
        float totalHelixHeight = oneHelixHeight * helixNumber - (helixNumber - 1) * oneHelixHeight + helixSpace;
        float totalSpace = helixSpace * (helixNumber - 1);
        float distance = Vector3.Distance(firstHelixPosition + Vector3.up * oneHelixHeight, pillar.transform.position);

        //Calculate and set the pillar's height
        float pillarHeight = totalSpace + totalHelixHeight + Mathf.Round(distance);
        pillar.transform.localScale = new Vector3(1, pillarHeight, 1);

        //Create helixs
        Vector3 nextHelixPos = firstHelixPosition + Vector3.down * helixSpace;
        for (int i = 0; i < helixNumber - 1; i++)
        {
            HelixController helixControl = Instantiate(helixPrefab, nextHelixPos, Quaternion.identity).GetComponent<HelixController>();
            helixControl.HandleHelix(Random.Range(levelData.MinDisablePiecesNumber, levelData.MaxDisablePiecesNumber),
                                     Random.Range(levelData.MinDeadPiecesNumber, levelData.MaxDeadPiecesNumber),
                                     levelData.NormalPieceColor, levelData.DeadPieceColor);
            helixControl.transform.SetParent(rotaterTrans);
            nextHelixPos = helixControl.transform.position + Vector3.down * helixSpace;
        }

        //Move bottomHelix object to the bottom
        bottomPillar.transform.position = nextHelixPos + Vector3.up * oneHelixHeight; ;
    }


    //////////////////////////////////////Publish functions

    /// <summary>
    /// Continue the game
    /// </summary>
    public void SetContinueGame()
    {
        IsRevived = true;
        PlayingGame();
    }

    /// <summary>
    /// Create a fading helix object at given position
    /// </summary>
    /// <param name="pos"></param>
    public void CreateFadingHelix(Vector3 pos)
    {
        FadingHelixController fadingHelixControl = GetFadingHelixControl();
        fadingHelixControl.transform.position = pos;
        fadingHelixControl.gameObject.SetActive(true);
        fadingHelixControl.FadingHelix(brokenPieceMaterial.color, fadingHelixScale, fadingHelixTime);
    }

    /// <summary>
    /// Create a ballSplat object at given position
    /// </summary>
    /// <param name="pos"></param>
    public void CreateBallSplat(Vector3 pos, Color playerColor, Transform parent)
    {
        BallSplatController ballSplatControl = GetBallSplatControl();
        ballSplatControl.transform.position = pos;
        ballSplatControl.transform.eulerAngles = new Vector3(90, Random.Range(0f, 360f), 0);
        ballSplatControl.gameObject.SetActive(true);
        ballSplatControl.FadeOut(playerColor, ballSplatFadingTime);
        ballSplatControl.transform.SetParent(parent);
    }


    /// <summary>
    /// Play splatShatter at given position
    /// </summary>
    /// <param name="pos"></param>
    public void PlaySplatShatter(Vector3 pos)
    {
        ParticleSystem splatShatter = GetSplatShatter();
        splatShatter.transform.position = pos;
        splatShatter.gameObject.SetActive(true);
        StartCoroutine(PlayParticle(splatShatter));
    }


    /// <summary>
    /// Decrease CurrentLevel by 1
    /// </summary>
    public void DecreaseCurrentLevel()
    {
        if (testingLevel == 0) //Isn't on testing level
            CurrentLevel--;
    }
}
