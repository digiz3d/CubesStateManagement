using UnityEngine;
using System.Collections.Generic;

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

        private delegate void PacketHandler(Packet pck, Server server, Client client);

        public bool IsClient { get; internal set; }
        private Client _client;
        private Queue<Packet> _clientReceivedPackets = new Queue<Packet>();
        private Dictionary<PacketType, PacketHandler> _clientPacketHandlers = new Dictionary<PacketType, PacketHandler>();


        public bool IsServer { get; internal set; }
        private Server _server;
        private Queue<Packet> _serverReceivedpackets = new Queue<Packet>();
        private Dictionary<PacketType, PacketHandler> _serverPacketHandlers = new Dictionary<PacketType, PacketHandler>();

        public bool IsHost { get; internal set; }

        private bool IsStarted => IsClient || IsHost || IsServer;


        private void Update()
        {
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
        }

        public void StartClient()
        {
            if (IsStarted) return;

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
        }

        public void StartHost()
        {
            if (IsStarted) return;

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
            _clientPacketHandlers = new Dictionary<PacketType, PacketHandler>() {
                { PacketType.GIVE_CLIENT_ID, ClientHandler.GetMyClientId }
            };
            _serverPacketHandlers = new Dictionary<PacketType, PacketHandler>(){
                { PacketType.THANKS, ServerHandler.ClientSaidThanks }
            };
        }
    }
}