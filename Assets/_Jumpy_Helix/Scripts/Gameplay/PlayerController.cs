using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OnefallGames;


public enum PlayerState
{
    Prepare,
    Living,
    Pause,
    PassLevel,
    Die,
}

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { private set; get; }
    public static event System.Action<PlayerState> PlayerStateChanged = delegate { };

    public PlayerState PlayerState
    {
        get
        {
            return playerState;
        }

        private set
        {
            if (value != playerState)
            {
                value = playerState;
                PlayerStateChanged(playerState);
            }
        }
    }


    private PlayerState playerState = PlayerState.Die;


    [Header("Player Config")]
    [SerializeField] private float jumpVelocity = 8;
    [SerializeField] private float fallingSpeed = -15;
    [SerializeField] private float minScale = 0.6f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float scalingFactor = 2;

    [Header("Player References")]
    [SerializeField] private HelixDetector helixDetector = null;
    [SerializeField] private ParticleSystem ballExplode = null;
    [SerializeField] private MeshRenderer meshRender = null;
    [SerializeField] private SphereCollider sphereCollider = null;

    public float TargetY { private set; get; }

    private RaycastHit hit;
    private Vector3 originalScale = Vector3.zero;
    private float currentJumpVelocity = 0;
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
        if (obj == GameState.Playing)
        {
            PlayerLiving();
        }
        else if (obj == GameState.Pause)
        {
            PlayerPause();
        }
    }



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



    void Start () {

        //Fire event
        PlayerState = PlayerState.Prepare;
        playerState = PlayerState.Prepare;

        //Add another actions here
        currentJumpVelocity = jumpVelocity;
        TargetY = transform.position.y;
        originalScale = transform.localScale;
        ballExplode.gameObject.SetActive(false);
    }

    void Update () {

        if (playerState == PlayerState.Living || playerState == PlayerState.PassLevel)
        {
            transform.position = transform.position + Vector3.up * (currentJumpVelocity * Time.deltaTime + fallingSpeed * Time.deltaTime * Time.deltaTime / 2);

            if (currentJumpVelocity < fallingSpeed)
                currentJumpVelocity = fallingSpeed;
            else
                currentJumpVelocity = currentJumpVelocity + fallingSpeed * Time.deltaTime;

            if (currentJumpVelocity > 0)
            {
                sphereCollider.enabled = false;
                Vector3 scale = transform.localScale;
                if (scale.x < maxScale)
                {
                    scale.x += scalingFactor * Time.deltaTime;
                }
                else
                    scale.x = maxScale;
                transform.localScale = scale;
            }
            else
            {
                sphereCollider.enabled = true;
                Vector3 scale = transform.localScale;
                if (scale.x > minScale)
                {
                    scale.x -= scalingFactor * Time.deltaTime;
                }
                else
                    scale.x = minScale;
                transform.localScale = scale;

                if (transform.position.y < TargetY)
                {
                    TargetY = transform.position.y;
                }

                //Check colliding
                Ray rayCenter = new Ray(transform.position, Vector3.down);
                Ray rayLeft = new Ray(transform.position + Vector3.left * meshRender.bounds.size.x, Vector3.down);
                Ray rayRight = new Ray(transform.position + Vector3.right * meshRender.bounds.size.x, Vector3.down);
                float rayLength = 100f;
                bool isHit = Physics.Raycast(rayCenter, out hit, rayLength);
                if (!isHit)
                    isHit = Physics.Raycast(rayLeft, out hit, rayLength);
                if(!isHit)
                    Physics.Raycast(rayRight, out hit, rayLength);
                if (isHit)
                {
                    float bottomY = (transform.position + Vector3.down * (meshRender.bounds.size.y / 2f)).y;
                    float distance = bottomY - hit.point.y;

                    if (distance <= 0.1f)
                    {
                        currentJumpVelocity = jumpVelocity;
                        TargetY = hit.point.y;
                        Vector3 pos = hit.point + Vector3.up * 0.01f; ;
                        GameManager.Instance.CreateBallSplat(pos, meshRender.material.color, hit.transform);
                        GameManager.Instance.PlaySplatShatter(pos);

                        if (playerState == PlayerState.Living)
                        {
                            if (hit.collider.CompareTag("Respawn")) //Hit the bottom pillar -> win the level
                            {
                                Vector3 fadingHelixPos = hit.transform.position + Vector3.down * 0.05f;
                                GameManager.Instance.CreateFadingHelix(fadingHelixPos);
                                PlayerPassLevel();
                                GameManager.Instance.PassLevel();
                            }
                            else
                            {
                                if (helixDetector.PassedCount >= GameManager.Instance.PassedCountForBreakHelix)
                                {
                                    ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.breakPieces);
                                    hit.collider.transform.parent.GetComponent<HelixController>().ShatterAllPieces();
                                }
                                else
                                {
                                    if (hit.collider.CompareTag("Finish"))//Hit dead piece -> game over
                                    {
                                        PlayerDie();
                                        if (GameManager.Instance.IsRevived)
                                        {
                                            GameManager.Instance.GameOver();
                                        }
                                        else
                                        {
                                            if (ServicesManager.Instance.AdManager.IsRewardedVideoAdReady())
                                            {
                                                GameManager.Instance.Revive();
                                            }
                                            else
                                            {
                                                GameManager.Instance.GameOver();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.bounce);
                                    }
                                }
                            }

                            helixDetector.ResetPassedCount();
                        }
                    }
                }
            }
        }      
	}

    private void PlayerLiving()
    {
        //Fire event
        PlayerState = PlayerState.Living;
        playerState = PlayerState.Living;

        //Add another actions here
        meshRender.enabled = true;
        if (GameManager.Instance.IsRevived)
        {
            currentJumpVelocity = jumpVelocity;
        }
    }

    private void PlayerPause()
    {
        //Fire event
        PlayerState = PlayerState.Pause;
        playerState = PlayerState.Pause;

        //Add another actions here
    }

    public void PlayerDie()
    {
        //Fire event
        PlayerState = PlayerState.Die;
        playerState = PlayerState.Die;

        //Add another actions here
        ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.playerExplode);
        meshRender.enabled = false;
        transform.localScale = originalScale;
        StartCoroutine(PlayBallExplode());
    }

    private void PlayerPassLevel()
    {
        //Fire event
        PlayerState = PlayerState.PassLevel;
        playerState = PlayerState.PassLevel;

        //Add another actions here

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Helix_Piece") && playerState == PlayerState.Living)
        {
            currentJumpVelocity = jumpVelocity;
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.bounce);
        }
    }


    //Play ball explode particle
    private IEnumerator PlayBallExplode()
    {
        ballExplode.transform.position = transform.position;
        ballExplode.gameObject.SetActive(true);
        ballExplode.Play();
        yield return new WaitForSeconds(ballExplode.main.startLifetimeMultiplier);
        ballExplode.gameObject.SetActive(false);
    }



    /// <summary>
    /// Set color for this ball (player)
    /// </summary>
    /// <param name="color"></param>
    public void SetBallColor(Color color)
    {
        meshRender.sharedMaterial.color = color;
    }
}
