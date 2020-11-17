using UnityEngine;

namespace Assets.Scripts.GameState
{
    public class GameStateManager : MonoBehaviour
    {
        #region Singleton
        public static GameStateManager Instance { private set; get; }
        private void Awake()
        {
            // if the singleton hasn't been initialized yet
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #endregion Singleton

        public GameState gameState;
        public GameObject playerPrefab;
        public GameObject puppetPrefab;
        public Transform playersContainer;

        void Start()
        {
            gameState = new GameState();
            // AddLocalPlayer(Random.Range(0, 1000000), Vector3.zero, Quaternion.identity);
            // SpawnRandomPlayer();
            // InvokeRepeating("SpawnRandomPlayer", 0.25f, 0.25f);
        }

        // Update is called once per frame
        void Update()
        {
            foreach (PlayerState playerState in gameState.players)
            {
                if (playerState.gameObject == null)
                {
                    GameObject prefab = playerState.id == gameState.currentPlayerId ? playerPrefab : puppetPrefab;
                    GameObject go = Instantiate(prefab, playerState.position, playerState.rotation, playersContainer);
                    Puppet puppet = go.GetComponent<Puppet>();
                    if (puppet) puppet.SubscribeToPlayerId(playerState.id);
                    playerState.gameObject = go;
                }
            }

            // gameState.playersById[5].position += new Vector3(0, 0, 1) * Time.deltaTime;
        }

        public void AddLocalPlayer(int id, Vector3 position, Quaternion rotation)
        {
            gameState.SetCurrentPlayerId(id);
            gameState.AddPlayer(id, position, rotation);
        }

        public void SpawnRandomPlayer()
        {
            UpsertPlayer(5, new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20)), Quaternion.identity);
        }

        public static void UpsertPlayer(int id, Vector3 position, Quaternion rotation)
        {
            Instance.gameState.AddPlayer(id, position, rotation);
        }

        public static GameState GetState()
        {
            return Instance.gameState;
        }

        public static void SetGameState(GameState nextState)
        {
            Instance.gameState = nextState;
        }
    }
}