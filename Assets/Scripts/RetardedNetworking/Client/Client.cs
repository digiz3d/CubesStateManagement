using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace RetardedNetworking
{
    public class Client
    {
        private Thread _clientThread;
        private bool _stopping = false;
        public byte Id { get; set; }

        private Queue<Packet> _packetsToSend = new Queue<Packet>();

        public Client(string serverIp, int serverPort)
        {
            _clientThread = new Thread(() =>
            {
                _stopping = false;

                Debug.Log($"[Client Thread] Connecting to {serverIp}:{serverPort}");

                TcpClient tcpClient = new TcpClient();

                if (!tcpClient.ConnectAsync(serverIp, serverPort).Wait(1000))
                {
                    Debug.Log("[Client Thread] Could not connect");
                    tcpClient.Close();
                    return;
                }

                NetworkStream stream = tcpClient.GetStream();

                while (!_stopping)
                {
                    lock (_packetsToSend)
                    {
                        if (stream.CanWrite && _packetsToSend.Count > 0)
                        {
                            _packetsToSend.Dequeue().SendToStream(Id, stream);
                        }
                    }

                    if (stream.CanRead && stream.DataAvailable)
                    {
                        Packet packet = Packet.ReadFrom(stream);
                        NetworkManager.Instance.ClientReceivePacket(packet);
                    }

                    Thread.Sleep(7);
                }

                tcpClient.Close();

                _stopping = false;
            })
            {
                IsBackground = true,
            };

            _clientThread.Start();
        }

        public void Stop()
        {
            _stopping = true;
            _clientThread.Join();
            _clientThread = null;
            _stopping = false;
        }

        public void SendPacketToServer(Packet packet)
        {
            lock (_packetsToSend)
            {
                _packetsToSend.Enqueue(packet);
            }
        }
    }
}