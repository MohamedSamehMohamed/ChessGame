using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace ChessGame
{
    public class ChessGame
    {
        private ChessBoard _board;
        private Player _player1;
        private Player _player2;
        private bool _player1Turn;
        private List<Move> _moves;
        private GameStatus _gameStatus;

        public ChessGame()
        {
            _board = new ChessBoard();
            _player1 = new Player();
            _player2 = new Player();
            _moves = new List<Move>();
            _gameStatus = GameStatus.Active;
            _player1Turn = true;
        }

        public Position ConvertToPosition(string str)
        {
            if (str[0] < 'a' || str[0] > 'h' || str[1] < '1' || str[1] > '8')
                throw new IndexOutOfRangeException();
            var row = 8 - int.Parse(str[1].ToString());
            var column = str[0] - 'a';
            return new Position(row,column);
        }
        public void Play(Move move)
        {
            MakeAMove(move);
        }
        public bool MakeAMove(Move move)
        {
            var sourcePiece = _board.GetPiece(move.Source);
            if (sourcePiece.White != _player1Turn)
                return false;
            if (!sourcePiece.CanMoveLogical(_board, move) || !sourcePiece.CanMove(_board, move))
                return false;
            _board.UpdateBoard(move.Source, new Empty());
            _board.UpdateBoard(move.Destination, sourcePiece);
            _player1Turn ^= true;
            _moves.Add(move);
            _updateGameStatus();
            return true;
        }

        public void Simulate()
        {
            while (_gameStatus == GameStatus.Active)
            {
                var move = _getRandomMove();
                Play(move);
                Console.Clear();
                Console.WriteLine($"total number of moves made is:{_moves.Count}");
                Console.WriteLine($"move made is {move}");
                _board.PrintBoard();
                Thread.Sleep(500);
            }
        }

        private Move _getRandomMove()
        {
            List<int[]> myPiceces = new List<int[]>();
            List<int[]> targetPlaces = new List<int[]>();
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var currentPiece = _board.GetPiece(new Position(i, j));
                    if (currentPiece is Empty || currentPiece.White != _player1Turn)
                    {
                        targetPlaces.Add(new []{i, j});
                    }
                    else
                    {
                        myPiceces.Add(new []{i, j});
                    }
                }
            }

            Random random = new Random();
            targetPlaces = targetPlaces.OrderBy(o => random.Next()).ToList();
            while (true)
            {
                var index = random.Next(myPiceces.Count - 1);
                var from = new Position(myPiceces[index][0], myPiceces[index][1]);
                var myPiece = _board.GetPiece(from);
                for (var index2 = 0; index2 < targetPlaces.Count; index2++)
                {
                    var to = new Position(targetPlaces[index2][0], targetPlaces[index2][1]);
                    var move = new Move(from, to);
                    if (!myPiece.CanMoveLogical(_board, move) || !myPiece.CanMove(_board, move))
                    {
                        continue;
                    }
                    return move;
                }
            }
        }

        private void _updateGameStatus()
        {
            // TODO 
        }
    }
}