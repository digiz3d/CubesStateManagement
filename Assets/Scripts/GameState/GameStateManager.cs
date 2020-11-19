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
                    Debug.Log($"{playerState.id} == {gameState.currentPlayerId}");
                    GameObject prefab = playerState.id == gameState.currentPlayerId ? playerPrefab : puppetPrefab;
                    GameObject go = Instantiate(prefab, playerState.position, playerState.rotation, playersContainer);
                    Puppet puppet = go.GetComponent<Puppet>();
                    if (puppet) puppet.SubscribeToPlayerId(playerState.id);
                    PlayerInput playerInput = go.GetComponent<PlayerInput>();
                    if (playerInput) playerInput.AttachToPlayer(playerState.id);
                    playersReconciliation.Add(kvp.Key, go);
                }
            }
        }

        public static void SetCurrentPlayerId(byte id)
        {
            Instance.gameState.SetCurrentPlayerId(id);
        }

        public static void UpsertPlayer(byte id, Vector3 position, Quaternion rotation)
        {
            Instance.gameState.UpsertPlayer(id, position, rotation);
        }

        public static void SetGameState(GameState nextState)
        {
            Instance.gameState = nextState;
        }

        public static void Move(Vector3 position, Quaternion rotation)
        {
            GameState gameState = Instance.gameState;
            byte playerId = gameState.currentPlayerId;
            gameState.players[playerId].position = position;
            gameState.players[playerId].rotation = rotation;
            RetardedNetworking.NetworkManager.Instance.ClientMove(position, rotation);
        }
    }
}