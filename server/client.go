package server

import (
	"encoding/json"
	"log"
	"net/http"
	"tictactoe-server/game"

	"github.com/gorilla/websocket"
)

var GameManager = game.NewManager()

type Client struct { //conection hub
	conn   *websocket.Conn //connection
	send   chan []byte     //
	RoomID string          // clients room id
	Index  int             // 0 and 1 depends on which player the client is
}

var upgrader = websocket.Upgrader{
	CheckOrigin: func(r *http.Request) bool {
		return true
	},
}

func ServerWS(w http.ResponseWriter, r *http.Request) {

	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println("Upgrade error:", err)
		return
	}
	client := &Client{
		conn: conn,
		send: make(chan []byte, 256),
	}

	//client.hub.register <- client

	log.Println("client connected")

	go client.reading()
	go client.writing()
}

func (c *Client) Send(msg any) {
	data, err := json.Marshal(msg)
	if err != nil {
		return
	}
	c.send <- data
}

func (c *Client) reading() {

	defer func() {
		//c.hub.unregister <- c
		c.conn.Close()
		log.Println("client disconnected")
	}()
	for {
		_, message, err := c.conn.ReadMessage()
		if err != nil {
			break
		}
		var msg Message
		json.Unmarshal(message, &msg)

		log.Printf("RAW MSG: %s\n", string(message))

		switch msg.Type {
		case "JOIN":
			log.Println("JOIN received")
			GameManager.Join(c)

		case "MOVE":
			log.Printf("MOVE received: cell=%d\n", msg.Cell)
			GameManager.HandleMove(c, msg.Cell)

		case "LEAVE":
			log.Println("leave clicled")
			GameManager.HandleLeave(c)

		case "REMATCH_REQUEST":
			log.Println("REmatching request")
			GameManager.HandleRematch(c)
		default:
			log.Println("UNKNOWN TYPE:", msg.Type)
		}

	}
}

func (c *Client) writing() {
	for msg := range c.send {
		err := c.conn.WriteMessage(websocket.TextMessage, msg)
		if err != nil {
			break
		}
	}
}

func (c *Client) SetRoomID(Id string) {
	c.RoomID = Id
}

func (c *Client) GetRoomID() string {
	return c.RoomID
}
func (c *Client) SetIndex(i int) {
	c.Index = i
}
func (c *Client) GetIndex() int {
	return c.Index
}
