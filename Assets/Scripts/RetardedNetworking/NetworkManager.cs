using Assets.Scripts.GameState;
using System.Collections.Generic;
using UnityEngine;

namespace RetardedNetworking
{
    public class NetworkManager : MonoBehaviour
    {
        #region Singleton
        public static NetworkManager Instance { get; internal set; }
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

        public bool IsClient { get; private set; }
        private Client _client;
        private Queue<Packet> _clientReceivedPackets = new Queue<Packet>();
        private Dictionary<PacketType, ClientPacketHandler> _clientPacketHandlers = new Dictionary<PacketType, ClientPacketHandler>();
        private delegate void ClientPacketHandler(Packet packet, Server server, Client client);

        public bool IsServer { get; private set; }
        private Server _server;
        private Queue<Packet> _serverReceivedpackets = new Queue<Packet>();
        private Dictionary<PacketType, ServerPacketHandler> _serverPacketHandlers = new Dictionary<PacketType, ServerPacketHandler>();
        private delegate void ServerPacketHandler(Packet packet, Server server, Client client);

        public bool IsHost { get; private set; }

        private bool IsStarted => IsClient || IsHost || IsServer;
        public const float tickrate = 10f;
        public const float tickTime = (1f / tickrate);
        float timeElapsedSinceLastTick = 0;

        private void Update()
        {
            timeElapsedSinceLastTick += Time.unscaledDeltaTime;
            if (timeElapsedSinceLastTick >= tickTime)
            {
                timeElapsedSinceLastTick -= tickTime;
                if (_client != null)
                {
                    Packet clientTransformSnapshot = new Packet(PacketType.CLIENT_TRANSFORM);
                    PlayerState.TransformState myTransform = GameStateManager.GetMyLastPlayerTransform();
                    clientTransformSnapshot.Write(myTransform.position);
                    clientTransformSnapshot.Write(myTransform.rotation);
                    _client.SendPacketToServer(clientTransformSnapshot);
                }

                if (_server != null)
                {
                    Packet clientTransformsSnapshot = new Packet(PacketType.CLIENTS_TRANSFORMS);
                    clientTransformsSnapshot.WritePlayersDictionary(GameStateManager.Instance.gameState.players);
                    _server.SendPacketToAllClients(clientTransformsSnapshot);
                }
            }

            if (_clientPacketHandlers.Count > 0)
            {
                lock (_clientReceivedPackets)
                {
                    while (_clientReceivedPackets.Count > 0)
                    {
                        Packet msg = _clientReceivedPackets.Dequeue();
                        if (_clientPacketHandlers.ContainsKey(msg.Type))
                        {
                            _clientPacketHandlers[msg.Type](msg, _server, _client);
                        }
                        else
                        {
                            Debug.Log($"[NetworkManager:client] Couldn't handle msg {msg.Type}");
                        }
                    }
                }
            }

            if (_serverPacketHandlers.Count > 0)
            {
                lock (_serverReceivedpackets)
                {
                    while (_serverReceivedpackets.Count > 0)
                    {
                        Packet msg = _serverReceivedpackets.Dequeue();
                        if (_serverPacketHandlers.ContainsKey(msg.Type))
                        {
                            _serverPacketHandlers[msg.Type](msg, _server, _client);
                        }
                        else
                        {
                            Debug.Log($"[NetworkManager:server] Couldn't handle msg {msg.Type}");
                        }
                    }
                }
            }
        }

        public void StartServer()
        {
            if (IsStarted) return;

            GameStateManager.Reset();
            InitializePacketHandlers();
            IsServer = true;
            _server = new Server(27015);

        }

        public void StopServer()
        {
            if (!IsServer) return;

            IsServer = false;
            _server.Stop();
            _server = null;
            GameStateManager.Reset();
        }

        public void StartClient()
        {
            if (IsStarted) return;

            GameStateManager.Reset();
            InitializePacketHandlers();
            IsClient = true;
            _client = new Client("127.0.0.1", 27015);
        }

        public void StopClient()
        {
            if (!IsClient) return;

            IsClient = false;
            _client.Stop();
            _client = null;
            GameStateManager.Reset();
        }

        public void StartHost()
        {
            if (IsStarted) return;

            GameStateManager.Reset();
            InitializePacketHandlers();
            IsHost = true;
            _server = new Server(27015)
            {
                onServerReady = () =>
                {
                    _client = new Client("127.0.0.1", 27015);
                }
            };

        }

        public void StopHost()
        {
            if (!IsHost) return;
            IsHost = false;

            _client.Stop();
            _client = null;
            _server.Stop();
            _server = null;
            GameStateManager.Reset();
        }


        public void ClientReceivePacket(Packet msg)
        {
            lock (_clientReceivedPackets)
            {
                _clientReceivedPackets.Enqueue(msg);
            }
        }

        public void ServerReceivePacket(Packet msg)
        {
            lock (_serverReceivedpackets)
            {
                _serverReceivedpackets.Enqueue(msg);
            }
        }

        private void InitializePacketHandlers()
        {
            _clientPacketHandlers = new Dictionary<PacketType, ClientPacketHandler>() {
                { PacketType.GIVE_CLIENT_ID, ClientHandler.GetMyClientId },
                { PacketType.GIVE_CLIENT_GAME_STATE, ClientHandler.GetGameState },
                { PacketType.CLIENTS_TRANSFORMS, ClientHandler.ClientsTransforms }

            };
            _serverPacketHandlers = new Dictionary<PacketType, ServerPacketHandler>(){
                { PacketType.THANKS, ServerHandler.ClientSaidThanks },
                { PacketType.CLIENT_TRANSFORM, ServerHandler.ClientTransform }
            };
        }
    }
}