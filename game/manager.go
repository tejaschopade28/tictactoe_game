package game

import (
	"log"
	"sync"
	
	"github.com/google/uuid"
)

type Manager struct {
	Waiting *Room
	Rooms   map[string]*Room
	mu      sync.Mutex
}

func NewManager() *Manager {
	return &Manager{
		Rooms: make(map[string]*Room),
	}
}

func (m *Manager) Join(p Player) {
	m.mu.Lock()
	defer m.mu.Unlock()
	if m.Waiting == nil {
		RoomId := uuid.NewString()
		room := NewRoom(RoomId, 3) // (roomid , size)
		room.AddPlayer(p, 0)
		m.Waiting = room
		m.Rooms[room.ID] = room
		p.Send(map[string]any{
			"type": "WAITING",
		})
		log.Println("room has one member")

	} else {
		room := m.Waiting
		room.AddPlayer(p, 1)
		m.Waiting = nil
		room.StartGame()
		log.Println("another member is joined")
	}
}

// handlemove
func (m *Manager) HandleMove(p Player, cell int) {

	if p.GetRoomID() == "" {
		return
	}
	//find room
	room, ok := m.Rooms[p.GetRoomID()]
	if !ok {
		return
	}
	//pass move to game logic
	room.MakeMove(p, cell)
}

func (m *Manager) HandleLeave(p Player) {
	m.mu.Lock()
	defer m.mu.Unlock()
	roomId := p.GetRoomID()
	if roomId == "" {
		return
	}
	room, ok := m.Rooms[roomId]

	if !ok {
		return
	}
	log.Println("Server leave the room ", roomId)

	// only waiting player in room
	if m.Waiting != nil && m.Waiting.ID == roomId {
		log.Println("Removing waiting room:", roomId)
		m.Waiting = nil
		delete(m.Rooms, roomId)
		return
	}

	room.Broadcast(map[string]any{
		"type": "OPPONENT_LEFT",
	})
	delete(m.Rooms, roomId)
}

func (m *Manager) HandleRematch(p Player) {
	roomId := p.GetRoomID()

	if roomId == "" {
		return
	}
	room, ok := m.Rooms[roomId]
	if !ok {
		return
	}
	room.RematchingRequest(p)

}
