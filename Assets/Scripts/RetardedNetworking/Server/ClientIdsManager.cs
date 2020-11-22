using System;
using System.Collections.Generic;

namespace RetardedNetworking
{
    public class ClientIdsManager
    {
        public const byte MAX_PLAYERS = 254;
        public const byte SERVER_CLIENT_ID = 0;

        private readonly List<byte> _ids = new List<byte>();

        public byte GetAvailableId()
        {
            if (_ids.Count >= MAX_PLAYERS)
                throw new Exception("[ClientIdsManager] Maximum clients number reached.");

            for (byte i = 1; i < MAX_PLAYERS; i++)
            {
                if (!_ids.Contains(i))
                {
                    _ids.Add(i);
                    return i;
                }
            }

            throw new Exception("[ClientIdsManager] No free Id available. Too many players.");
        }

        public void FreeId(byte id)
        {
            _ids.Remove(id);
        }
    }
}