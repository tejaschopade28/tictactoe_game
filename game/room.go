package game

import (
	"log"
)

type Room struct {
	ID             string //same as RoomId for cleint
	Size           int
	Board          []int     // inside board it can be 0 1(X) 2(O)
	Players        [2]Player //two players in one room
	Turn           int       // 0 or 1 based on which players turn is in the room
	Over           bool      // its game over or not
	RematchRequest [2]bool   //rematch required both yes
}

// create new room , turn start at player 0
func NewRoom(id string, size int) *Room {
	return &Room{
		ID:    id,
		Size:  size,
		Board: make([]int, size*size),
		Turn:  0, //at first by default 0's player has furst chance
		Over:  false,
	}
}

// adding player to room
func (r *Room) AddPlayer(p Player, index int) {
	r.Players[index] = p
	p.SetRoomID(r.ID)
	p.SetIndex(index)
}

func (r *Room) StartGame() {
	r.Turn = 0

	r.Players[0].Send(map[string]any{
		"type":        "START",
		"playerIndex": 0,
		"size":        r.Size,
	})

	r.Players[1].Send(map[string]any{
		"type":        "START",
		"playerIndex": 1,
		"size":        r.Size,
	})
}

func (r *Room) MakeMove(p Player, cell int) {
	log.Println("SERVER: MakeMove called by player", p.GetIndex(), "cell", cell)
	//is game over
	//check room exist
	//check players room
	//check valid cell apply move
	// check win
	//check draw
	//broadcast the meessage

	if r.Over == true {
		return
	}
	if p.GetRoomID() != r.ID {
		return
	}
	if p.GetIndex() != r.Turn {
		log.Println("SERVER: Wrong turn")
		return
	}
	if cell < 0 || cell >= r.Size*r.Size {
		return
	}
	if r.Board[cell] != 0 {
		log.Println("SERVER: Cell not empty")
		return
	}

	playerValue := p.GetIndex() + 1
	r.Board[cell] = playerValue

	//  Win check
	if checkWin(r.Board, r.Size, playerValue) {
		r.Over = true
		r.Broadcast(map[string]any{
			"type":   "GAME_OVER",
			"winner": p.GetIndex(),
			"board":  r.Board,
		})
		return
	}
	//  Draw check
	if checkDraw(r.Board) {
		r.Over = true
		r.Broadcast(map[string]any{
			"type":  "GAME_DRAW",
			"board": r.Board,
		})
		return
	}
	r.Turn = 1 - r.Turn
	log.Println("SERVER: Broadcasting MOVE, turn:", r.Turn)
	r.Broadcast(map[string]any{
		"type":  "MOVE",
		"board": r.Board,
		"turn":  r.Turn,
	})
}

func (r *Room) RematchingRequest(p Player) {
	if !r.Over {
		return
	}
	player_index := p.GetIndex()
	r.RematchRequest[player_index] = true
	log.Println("REMATCH_REQUEST from player", player_index)

	r.Broadcast(map[string]any{
		"type":     "REMATCH_UPDATE",
		"accepted": r.RematchRequest,
	})
	if r.RematchRequest[0] && r.RematchRequest[1] {
		r.ResetMatch()
	}
}

func (r *Room) ResetMatch() {
	r.Board = make([]int, r.Size*r.Size)
	r.Turn = 0
	r.Over = false
	r.RematchRequest = [2]bool{false, false}

	r.Broadcast(map[string]any{
		"type":  "REMATCH_START",
		"board": r.Board,
		"turn":  r.Turn,
	})

}
// broadcast helper

func (r *Room) Broadcast(msg any) {
	for _, p := range r.Players {
		if p != nil {
			p.Send(msg)
		}
	}
}
