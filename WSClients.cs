using UnityEngine;
using NativeWebSocket;
using System.Text;
using System.Threading.Tasks;

public class WSClients : MonoBehaviour
{
    public static WSClients Instance;
    WebSocket webSocket;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public async void Connect()
    {
        if (webSocket != null &&
            webSocket.State == WebSocketState.Open)
        {
            Debug.Log("Already connected");
            return;
        }
        //create websocket
        webSocket = new WebSocket("ws://10.71.94.237:8080/ws"); 

        //when open connection
        webSocket.OnOpen += () =>
        {
            Debug.Log("Connected to server");
            SendJoin();
        };

        //msg from server
        webSocket.OnMessage +=(bytes) =>
        {
            string msg= Encoding.UTF8.GetString(bytes);
            Debug.Log("Msg from server" + msg);
            var serverMsg = JsonUtility.FromJson<ServerMessage>(msg);
                HandleServerMessage(serverMsg);
        };

        webSocket.OnError +=(e) =>
        {
            Debug.Log("error in connection" +e);
        };
        webSocket.OnClose += (e) =>
        {
            Debug.Log("conection close");
        };
        await webSocket.Connect();
    }

    void Update()
    {
        webSocket?.DispatchMessageQueue();
    }
    async void SendJoin()
    {
        if (webSocket.State == WebSocketState.Open)
        {
            string joinmsg = "{\"type\":\"JOIN\"}";
            await webSocket.SendText(joinmsg);
            Debug.Log("sent join");
        }
    }
    public async void SendMove(int cell)
    {
        Debug.Log("CLIENT: SendMove called with " + cell);

        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            Debug.LogError("CLIENT: WebSocket not open");
            return;
        }
        MoveMessage msg = new MoveMessage
        {
            type = "MOVE",
            cell = cell
        };
        string json = JsonUtility.ToJson(msg);
        Debug.Log("CLIENT: Sending JSON -> " + json);

        await webSocket.SendText(json);
    }
    public async void SendRematch()
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            return;
        }

        await webSocket.SendText("{\"type\":\"REMATCH_REQUEST\"}");
    }
    void HandleServerMessage(ServerMessage msg)
    {
        Debug.Log("Handling server msg type: " + msg.type);

        switch (msg.type)
        {
            case "WAITING":
                Debug.Log("Waiting for opponent");
                break;

            case "START":
                Debug.Log("Game started");
                OnlineGameManager.Instance.StartOnlineGame(msg.playerIndex);
                break;

            case "MOVE":
                Debug.Log("MOVE RECEIVED | board len = " + msg.board?.Length + " turn = " + msg.turn);
                OnlineGameManager.Instance.ServerBoardApply(msg.board, msg.turn);
                break;

            case "OPPONENT_LEFT":
                Debug.Log("On oppoent left the game");
                OnlineGameManager.Instance.OnOpponentLeft();
                break;

             case "REMATCH_UPDATE":
                Debug.Log("Opponent requested rematch");
                OnlineGameManager.Instance.OnRematchUpdate(msg.accepted);
                break;

            case "REMATCH_START":
                Debug.Log("Rematch starting");
                OnlineGameManager.Instance.StartRematch();
                break;

            case "GAME_OVER":
                Debug.Log("Game over received");
                OnlineGameManager.Instance.ServerBoardApply(msg.board, -1);
                OnlineGameManager.Instance.OnGameOver(msg.winner);
               // GameManager.Instance.SetGameState(GameEnums.GameState.GameOver);
                break;

            case "GAME_DRAW":
                Debug.Log("Game draw received");
                OnlineGameManager.Instance.ServerBoardApply(msg.board, -1);
                OnlineGameManager.Instance.OnGameOver(-1);
                break;
            default:
                Debug.LogWarning("Unknown message type: " + msg.type);
                break;
        }
    }

    public async void LeaveRoomAndDisconnect()
    {
        if (webSocket == null)
        {
            Debug.Log("WebSocket already null");
            return;
        }

        if (webSocket.State == WebSocketState.Open)
        {
            Debug.Log("CLIENT: Sending LEAVE");
            await webSocket.SendText("{\"type\":\"LEAVE\"}");

            Debug.Log("CLIENT: Closing socket");
            await webSocket.Close();
        }
        else
        {
            Debug.Log("Socket not open, state = " + webSocket.State);
        }

        webSocket = null;
    }

    async void OnApplicationQuit()
    {
        if(webSocket!=null && webSocket.State== WebSocketState.Open)
        {
            await webSocket.Close();
        }
    }

}


