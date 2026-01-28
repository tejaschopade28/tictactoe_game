using UnityEngine;


[System.Serializable]
public class ServerMessage
{
    public string type;
    public int[] board;
    public int turn;
    public int winner;
    public int playerIndex;
    public bool[] accepted;
}

[System.Serializable]
public class MoveMessage
{
    public string type;
    public int cell;
}
