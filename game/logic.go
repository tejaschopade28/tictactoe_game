package game

func checkRow(board []int, size, row, player int) bool {
	for col := 0; col < size; col++ {
		if board[row*size+col] != player {
			return false
		}
	}
	return true
}

func checkCol(board []int, size, col, player int) bool {
	for row := 0; row < size; row++ {
		if board[row*size+col] != player {
			return false
		}
	}
	return true
}

func checkMainDiagonal(board []int, size, player int) bool {
	for i := 0; i < size; i++ {
		if board[i*size+i] != player {
			return false
		}
	}
	return true
}

func checkAntiDiagonal(board []int, size, player int) bool {
	for i := 0; i < size; i++ {
		if board[i*size+(size-1-i)] != player {
			return false
		}
	}
	return true
}

func checkWin(board []int, size, player int) bool {
	for i := 0; i < size; i++ {
		if checkRow(board, size, i, player) ||
			checkCol(board, size, i, player) {
			return true
		}
	}
	return checkMainDiagonal(board, size, player) ||
		checkAntiDiagonal(board, size, player)
}

func checkDraw(board []int) bool {
	for _, cell := range board {
		if cell == 0 {
			return false
		}
	}
	return true
}
