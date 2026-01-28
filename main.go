package main

import (
	"log"
	"net/http"
	"tictactoe-server/server"
)

func main() {
	http.HandleFunc("/ws", func(w http.ResponseWriter, r *http.Request) {
		server.ServerWS(w, r)
	})

	log.Println("Server start at :8080")
	err := http.ListenAndServe(":8080", nil)
	if err != nil {
		log.Fatal("listenandserver ", err)
	}
}
