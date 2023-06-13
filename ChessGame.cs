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
        private Random _random;

        public ChessGame()
        {
            _board = new ChessBoard();
            _player1 = new Player();
            _player2 = new Player();
            _moves = new List<Move>();
            _gameStatus = GameStatus.Active;
            _player1Turn = true;
            _random = new Random();
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
            if (sourcePiece is King king && king.IsCastleMove(_board, move))
            {
                Position kingPlace, rookPlace;
                var theRock = _board.GetPiece(move.Destination);
                if (move.Source.Column < move.Destination.Column)
                {
                    kingPlace = new Position(move.Source.Row,move.Source.Column + 2);
                    rookPlace = new Position(move.Source.Row, move.Source.Column + 1);
                }
                else
                {
                    kingPlace = new Position(move.Source.Row,move.Source.Column - 2);
                    rookPlace = new Position(move.Source.Row, move.Source.Column - 1);
                }
                _board.UpdateBoard(kingPlace, sourcePiece);
                _board.UpdateBoard(rookPlace, theRock);
                _board.UpdateBoard(move.Source, new Empty());
                _board.UpdateBoard(move.Destination, new Empty());
                theRock.FirstMove = false;
            }
            else
            {
                _board.UpdateBoard(move.Source, new Empty());
                _board.UpdateBoard(move.Destination, sourcePiece);
                sourcePiece.FirstMove = false;
            }

            _player1Turn ^= true;
            _moves.Add(move);
            _updateGameStatus();
            return true;
        }

        public void Simulate()
        {
            while (_gameStatus == GameStatus.Active)
            {
                var move = _getGoodMove();
                Play(move);
                Console.Clear();
                Console.WriteLine($"total number of moves made is:{_moves.Count}");
                Console.WriteLine($"move made is {move}");
                _board.PrintBoard();
                Console.ReadLine();
            }
        }

        private Move _getRandomMove()
        {
            var moves = _getValidMoves();
            if (moves.Count == 0)
                throw new Exception("No valid move exist");
            var len = moves.Count;
            return moves[_random.Next(len)];
        }

        private Move _getGoodMove()
        {
            var moves = _getValidMoves();
            if (moves.Count == 0)
                throw new Exception("No valid move exist");
            var maxScore = -1;
            var goodMove = moves[0];
            foreach (var move in moves)
            {
                var currentScore = _getMoveScore(move);
                if (currentScore <= maxScore) continue;
                maxScore = currentScore;
                goodMove = move;
            }
            return goodMove;
        }

        private int _getMoveScore(Move move)
        {
            return _board.GetPiece(move.Destination).PieceValue;
        }
        private List<Move> _getValidMoves()
        {
            List<Move> validMoves = new List<Move>();
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

            myPiceces = myPiceces.OrderBy(o => _random.Next()).ToList();
            targetPlaces = targetPlaces.OrderBy(o => _random.Next()).ToList();
            for(var index = 0; index < myPiceces.Count; index++)
            {
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
                    validMoves.Add(move);
                }
            }
            return validMoves;
        }

        private void _updateGameStatus()
        {
            var numberOfKings = 0;
            var white = false;
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var piece = _board.GetPiece(new Position(i, j));
                    if (piece is King)
                    {
                        numberOfKings++;
                        white = piece.White;
                    }else if (piece is Pawn && (i == 0 || i == 7))
                    {
                        var c = _random.Next(4);
                        switch (c)
                        {
                            case 0:
                                piece = new Bishop { White = piece.White };
                                break;
                            case 1:
                                piece = new Knight { White = piece.White };
                                break;
                            case 2:
                                piece = new Queen { White = piece.White };
                                break;
                            case 3:
                                piece = new Rook { White = piece.White };
                                break;
                        }
                        _board.UpdateBoard(new Position(i, j), piece);
                    }
                }
            }

            if (numberOfKings == 1)
            {
                _gameStatus = (white ? GameStatus.BlackWin : GameStatus.WhiteWin);
                return;
            }
        }

        public void Manuel()
        {
            while (_gameStatus == GameStatus.Active)
            {
                var str = Console.ReadLine().Split(' ');
                var move = new Move(ConvertToPosition(str[0]), ConvertToPosition(str[1]));
                Play(move);
                Console.Clear();
                Console.WriteLine($"total number of moves made is:{_moves.Count}");
                Console.WriteLine($"move made is {move}");
                _board.PrintBoard();
            }
        }
    }
}