using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public enum Element
    {
        Water,
        Earth,
        Fire,
        Air,
        Missing,
    }

    public enum Team
    {
        Red,
        Blue,
        Spectator,
        Missing,
    }

    public enum GameAction
    {
        Start,
        End,
        AddRound,
        LowerArena
    }
}

public struct CreateFPSPlayerMessage : NetworkMessage
{
    public string playerName;
    //public Sprite sprite;
}