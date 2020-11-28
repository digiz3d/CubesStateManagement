using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RetardedNetworking
{
    public class Server
    {
        private Dictionary<byte, ServerClient> _clients = new Dictionary<byte, ServerClient>();
        private Thread _serverThread;
        private bool _stopping = false;
        public delegate void OnServerReadyCallback();
        public OnServerReadyCallback onServerReady;

        public Server(int port)
        {
            _serverThread = new Thread(() =>
            {
                _stopping = false;
                ClientIdsManager idsManager = new ClientIdsManager();
                Debug.Log("[Server Thread] Hi.");

                TcpListener listener = TcpListener.Create(27015);

                listener.Start();
                onServerReady?.Invoke();

                while (!_stopping)
                {
                    while (listener.Pending())
                    {
                        Debug.Log("[Server Thread] Accepting new client.");
                        TcpClient tcpListener = listener.AcceptTcpClient();

                        try
                        {
                            byte newClientId = idsManager.GetAvailableId();
                            ServerClient client = new ServerClient(newClientId, tcpListener);
                            _clients.Add(newClientId, client);
                            Packet clientId = new Packet(PacketType.GIVE_CLIENT_ID);
                            clientId.Write(newClientId);
                            SendPacketToClient(clientId, client);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("[Server thread] an exception occured.");
                            Debug.Log(e.Message);
                            Debug.Log(e.Source);
                            Debug.Log(e.StackTrace);
                        }
                    }

                    foreach (KeyValuePair<byte, ServerClient> kvp in _clients)
                    {
                        ServerClient client = kvp.Value;
                        NetworkStream stream = client.NetworkStream;

                        lock (client.packetsToSend)
                        {
                            while (stream.CanWrite && client.packetsToSend.Count > 0)
                            {
                                client.packetsToSend.Dequeue().SendToStream(stream);
                            }
                        }

                        while (stream.CanRead && stream.DataAvailable)
                        {
                            Packet packet = Packet.ReadFrom(stream);
                            if (packet.SenderClientId != client.Id)
                            {
                                Debug.Log("A client sent a packet as someone else.");
                                stream.Close();
                                stream.Dispose();
                                client.Tcp.Close();
                                client.Tcp.Dispose();
                            }
                            else
                            {
                                NetworkManager.Instance.ServerReceivePacket(packet);
                            }
                        }
                    }

                    Thread.Sleep(1);
                }

                listener.Stop();
                _stopping = false;
            })
            {
                IsBackground = true,
            };

            _serverThread.Start();
        }

        public void Stop()
        {
            _stopping = true;
            _serverThread.Join();
            _serverThread = null;
            _clients.Clear();
        }

        public void SendPacketToClient(Packet packet, byte clientId)
        {
            ServerClient client = _clients[clientId];
            SendPacketToClient(packet, client);
        }

        public void SendPacketToClient(Packet packet, ServerClient client)
        {
            lock (client.packetsToSend)
            {
                client.packetsToSend.Enqueue(packet);
            }
        }

        public void SendPacketToAllClients(Packet packet)
        {
            foreach (KeyValuePair<byte, ServerClient> kvp in _clients)
            {
                lock (kvp.Value.packetsToSend)
                {
                    kvp.Value.packetsToSend.Enqueue(packet);
                }
            }
        }
    }
}