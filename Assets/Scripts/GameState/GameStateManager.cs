using System.Collections.Generic;
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
        private Dictionary<byte, GameObject> playersReconciliation = new Dictionary<byte, GameObject>();
        public GameObject playerPrefab;
        public GameObject puppetPrefab;
        public Transform playersContainer;
        public byte currentPlayerId = 0;

        void Start()
        {
            gameState = new GameState();
        }

        void Update()
        {
            foreach (KeyValuePair<byte, PlayerState> kvp in gameState.players)
            {
                PlayerState playerState = kvp.Value;
                if (!playersReconciliation.ContainsKey(kvp.Key))
                {
                    GameObject prefab = playerState.id == currentPlayerId ? playerPrefab : puppetPrefab;
                    PlayerState.TransformState interpolated = playerState.GetLastTransformState();
                    GameObject go = Instantiate(prefab, interpolated.position, interpolated.rotation, playersContainer);
                    Puppet puppet = go.GetComponent<Puppet>();
                    if (puppet) puppet.SubscribeToPlayerId(playerState.id);
                    playersReconciliation.Add(kvp.Key, go);
                }
            }

            List<byte> playersIdsToRemove = new List<byte>();
            foreach (KeyValuePair<byte, GameObject> kvp in playersReconciliation)
            {
                if (!gameState.players.ContainsKey(kvp.Key))
                {
                    playersIdsToRemove.Add(kvp.Key);
                }
            }
            foreach (byte playerId in playersIdsToRemove)
            {
                playersReconciliation.Remove(playerId);
            }
        }

        public static void SetGameState(GameState nextState)
        {
            Instance.gameState = nextState;
        }

        public static void Move(Vector3 position, Quaternion rotation)
        {
            byte playerId = Instance.currentPlayerId;
            Instance.gameState.players[playerId].UpdateTransform(Time.unscaledTime, position, rotation);
        }

        public static void Reset()
        {
            Instance.gameState = new GameState();
            Instance.playersReconciliation.Clear();
        }
    }
}