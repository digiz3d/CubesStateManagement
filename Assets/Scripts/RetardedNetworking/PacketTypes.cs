namespace RetardedNetworking
{
    public enum PacketType : byte
    {
        GIVE_CLIENT_ID = 5,
        THANKS,
        GIVE_CLIENT_GAME_STATE,
        SPAWN_PLAYER,
        CLIENT_TRANSFORM,
        CLIENTS_TRANSFORMS
    }
}