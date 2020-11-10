using UnityEngine;

namespace OnefallGames
{
    public class ServicesManager : MonoBehaviour
    {
        public static ServicesManager Instance { private set; get; }

        [SerializeField] private SoundManager soundManager = null;
        [SerializeField] private ShareManager shareManager = null;
        [SerializeField] private AdManager adManager = null;
        [SerializeField] private LeaderboardManager leaderboardManager = null;

        public SoundManager SoundManager { get { return soundManager; } }
        public ShareManager ShareManager { get { return shareManager; } }
        public AdManager AdManager { get { return adManager; } }
        public LeaderboardManager LeaderboardManager { get { return leaderboardManager; } }

        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

