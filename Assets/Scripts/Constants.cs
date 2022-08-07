using Mirror;
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

    // Color doesn't work to set image colors for some goddamn reason
    public static Color32 redTeamColor = new Color32(220, 20, 60, 100);
    public static Color32 blueTeamColor = new Color32(0, 191, 255, 100);

    public static int timeAfterRoundEnd = 4;    // Time to wait after round/game ends
}

public struct CreateFPSPlayerMessage : NetworkMessage
{
    public string playerName;
    //public Sprite sprite;
}