package game

type Player interface {
	Send(msg any)
	GetRoomID() string
	SetRoomID(id string)
	GetIndex() int
	SetIndex(int)
}
