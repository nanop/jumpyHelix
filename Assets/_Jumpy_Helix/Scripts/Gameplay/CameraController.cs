using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance { private set; get; }


    [Header("Camera Config")]
    [SerializeField] private float smoothTime = 0.2f;


    private Vector3 velocity = Vector3.zero;
    private float originalDistance = 0;


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

    private void Start()
    {
        originalDistance = transform.position.y - PlayerController.Instance.transform.position.y;
    }

    private void Update()
    {
        if (PlayerController.Instance.PlayerState == PlayerState.Living)
        {
            float currentDistance = transform.position.y - PlayerController.Instance.TargetY;
            if (currentDistance > originalDistance)
            {
                float distance = currentDistance - originalDistance;
                Vector3 targetPos = transform.position + Vector3.down * distance;
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            }
        }
    }

}
