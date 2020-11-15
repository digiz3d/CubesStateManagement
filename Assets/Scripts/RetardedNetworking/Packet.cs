﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Assets.Scripts.GameState;

namespace RetardedNetworking
{
    public class Packet
    {
        // Network packet id, client id, packet's data portion length (1+1+4=6)
        private const byte headerSize = sizeof(byte) + sizeof(byte) + sizeof(int);
        public PacketType Type { private set; get; }
        public byte SenderClientId { private set; get; }
        private List<byte> buffer = new List<byte>(headerSize);
        private int readPos = 0;

        public static Packet ReadFrom(NetworkStream stream)
        {
            byte[] headerBuffer = new byte[headerSize];
            stream.Read(headerBuffer, 0, headerSize);

            PacketType type = (PacketType)headerBuffer[0];
            byte clientId = headerBuffer[1];

            int dataLength = BitConverter.ToInt32(headerBuffer, 2 * sizeof(byte));
            byte[] data = new byte[dataLength];

            stream.Read(data, 0, dataLength);

            return new Packet(type, clientId, data);
        }

        public Packet(PacketType type, byte senderId, byte[] data)
        {
            Type = type;
            buffer.Add((byte)type);
            SenderClientId = senderId;
            buffer.Add(senderId);
            buffer.AddRange(data);
            readPos = 2;
        }

        public void Write(byte data)
        {
            buffer.Add(data);
        }
        public byte ReadByte()
        {
            byte val = buffer[readPos];
            readPos++;
            return val;
        }

        public void Write(byte[] data)
        {
            buffer.AddRange(data);
        }
        public byte[] ReadBytes(int quantity)
        {
            byte[] val = buffer.GetRange(readPos, quantity).ToArray();
            readPos += val.Length;
            return val;
        }

        public void Write(bool data)
        {
            buffer.AddRange(BitConverter.GetBytes(data));
        }
        public bool ReadBool()
        {
            bool val = BitConverter.ToBoolean(buffer.GetRange(readPos, sizeof(bool)).ToArray(), 0);
            readPos += sizeof(bool);
            return val;
        }
        public void Write(short data)
        {
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        public short ReadShort()
        {
            short val = BitConverter.ToInt16(buffer.GetRange(readPos, sizeof(short)).ToArray(), 0);
            readPos += sizeof(short);
            return val;
        }

        public void Write(int data)
        {
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        public int ReadInt()
        {
            int val = BitConverter.ToInt32(buffer.GetRange(readPos, sizeof(int)).ToArray(), 0);
            readPos += sizeof(int);
            return val;
        }

        public void Write(float data)
        {
            buffer.AddRange(BitConverter.GetBytes(data));
        }

        public float ReadFloat()
        {
            float val = BitConverter.ToSingle(buffer.GetRange(readPos, sizeof(float)).ToArray(), 0);
            readPos += sizeof(float);
            return val;
        }

        public void Write(string data)
        {
            short len = (short)data.Length;
            Write(len);
            Write(Encoding.UTF8.GetBytes(data));
        }
        public string ReadString()
        {
            short len = ReadShort();
            byte[] bytes = ReadBytes(len);
            return Encoding.UTF8.GetString(bytes);
        }

        public void Write(Vector2 data)
        {
            Write(data.x);
            Write(data.y);
        }
        public Vector2 ReadVector2()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            return new Vector2(x, y);
        }

        public void Write(Vector3 data)
        {
            Write(data.x);
            Write(data.y);
            Write(data.z);
        }
        public Vector3 ReadVector3()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();
            return new Vector3(x, y, z);
        }

        public void Write(Quaternion data)
        {
            Write(data.x);
            Write(data.y);
            Write(data.z);
            Write(data.w);
        }
        public Quaternion ReadQuaternion()
        {
            float x = ReadFloat();
            float y = ReadFloat();
            float z = ReadFloat();
            float w = ReadFloat();
            return new Quaternion(x, y, z, w);
        }




        public void Write(GameState gameState)
        {
            Write(gameState.currentPlayerId);
            Write(gameState.players);
            Write(gameState.serverName);
        }

        public GameState ReadGameState()
        {
            int currentPlayerId = ReadInt();
            List<PlayerState> players = ReadListPlayerState();
            string serverName = ReadString();

            return new GameState()
            {
                currentPlayerId = currentPlayerId,
                players = players,
                serverName = serverName
            };
        }

        public void Write(List<PlayerState> players)
        {
            int len = players.Count;
            Write(len);
            foreach (PlayerState player in players)
            {
                Write(player);
            }
        }

        public List<PlayerState> ReadListPlayerState()
        {
            List<PlayerState> list = new List<PlayerState>();
            int len = ReadInt();
            for (int i = 0; i < len; i++)
            {
                list.Add(ReadPlayerState());
            }
            return list;
        }

        public void Write(PlayerState player)
        {
            Write(player.id);
            Write(player.position);
            Write(player.rotation);
        }

        public PlayerState ReadPlayerState()
        {
            int id = ReadInt();
            Vector3 pos = ReadVector3();
            Quaternion rot = ReadQuaternion();


            return new PlayerState(id, pos, rot);
        }

        public void SendToStream(NetworkStream stream)
        {
            int len = buffer.Count;
            buffer.InsertRange(2, BitConverter.GetBytes(len));
            byte[] bytes = buffer.ToArray();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}