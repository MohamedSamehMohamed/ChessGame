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
        private Stack<Move> _moves;
        private GameStatus _gameStatus;
        private Random _random;

        public ChessGame()
        {
            _board = new ChessBoard();
            _player1 = new Player();
            _player2 = new Player();
            _moves = new Stack<Move>();
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
            if (move == null) return false;
            var sourcePiece = _board.GetPiece(move.Source);
            var myMoves = sourcePiece.GetMyMoves(_board, move.Source);
            if (myMoves.Contains(move) == false)
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
                sourcePiece.FirstMove = false;
            }
            else
            {
                move.CapturedPiece = _board.GetPiece(move.Destination);
                move.IsSourceFirstMove = sourcePiece.FirstMove;
                _board.UpdateBoard(move.Source, new Empty());
                _board.UpdateBoard(move.Destination, sourcePiece);
                sourcePiece.FirstMove = false;
            }

            _player1Turn ^= true;
            _moves.Push(move);
            _updateGameStatus();
            return true;
        }

        public void UnDoMove()
        {
            var move = _moves.Pop();
            var source = _board.GetPiece(move.Destination);
            source.FirstMove = move.IsSourceFirstMove;
            _board.UpdateBoard(move.Source, source);
            _board.UpdateBoard(move.Destination, move.CapturedPiece);
            
            _player1Turn ^= true;
            _updateGameStatus();
        }
        public void Simulate()
        {
            while (_gameStatus == GameStatus.Active)
            {
                var move = _getAGoodSecondMove(2);
                Play(move.Item1);
                Console.Clear();
                Console.WriteLine($"total number of moves made is:{_moves.Count}");
                Console.WriteLine($"move made is {move}");
                _board.PrintBoard();
                Thread.Sleep(1000);
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

        public int GetAGoodMoveScore()
        {
            var moves = _getValidMoves();
            if (moves.Count == 0)
                return 0;
            var maxScore = 0;
            foreach (var move in moves)
            {
                var currentScore = GetMoveScore(move);
                if (currentScore <= maxScore) continue;
                maxScore = currentScore;
            }
            return maxScore;
        }
        public (Move, int) _getAGoodSecondMove(int dep = 1)
        {
            var moves = _getValidMoves();
            /*
            Console.WriteLine($"{(_player1Turn? "white": "black")} turn dep {dep}");
            foreach (var move in moves)
            {
                Console.WriteLine(move);
            }
            Console.WriteLine();
            */
            if (moves.Count == 0) return (null,0);
            var maxScore = _player1Turn? (int)-1e9: (int)1e9;
            Move goodMove = null;
            foreach (var move in moves)
            {
                var currentScore = GetMoveScore(move) * (_player1Turn? 1: -1);
                if (dep > 0)
                {
                    Play(move);
                    var currentMove = _getAGoodSecondMove(dep - 1);
                    var score = 0;
                    if (currentMove.Item1 == null)
                    {
                        if (_player1Turn)
                        {
                            score = 200;
                        }
                        else
                        {
                            score = -200;
                        }
                    }
                    else
                    {
                        score = currentMove.Item2;
                        if (!_player1Turn)
                            score *= -1;
                    }
                    currentScore -= score;
                    UnDoMove();
                }

                var change = false;
                if (_player1Turn)
                {
                    if (currentScore > maxScore)
                        change = true;
                }
                else
                {
                    if (currentScore < maxScore)
                        change = true;
                }

                if (!change) continue;
                maxScore = currentScore;
                goodMove = move;
            }

            return (goodMove, maxScore);
        }
        public Move GetGoodMove()
        {
            var moves = _getValidMoves();
            return moves.Count == 0 ? null : moves.OrderByDescending(GetMoveScore).FirstOrDefault();
        }
        public int GetMoveScore(Move move)
        {
            return _board.GetPiece(move.Destination).PieceValue;
        }
        private List<Move> _getValidMoves()
        {
            var validMoves = new List<Move>();
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var currentPiece = _board.GetPiece(new Position(i, j));
                    if (currentPiece is Empty || currentPiece.White != _player1Turn)
                    {
                        continue;
                    }
                    validMoves.AddRange(currentPiece.GetMyMoves(_board, new Position(i, j)));
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
        public void AgainstComputer()
        {
            while (_gameStatus == GameStatus.Active)
            {
                Move move;
                if (_player1Turn)
                {
                    var str = Console.ReadLine().Split(' ');
                    move = new Move(ConvertToPosition(str[0]), ConvertToPosition(str[1]));
                }
                else
                {
                    move = _getAGoodSecondMove(2).Item1;
                    Console.WriteLine("To play");
                    Console.WriteLine(move);
                }

                if (!MakeAMove(move))
                {
                    Console.WriteLine("Can't make the move");
                    _gameStatus = _player1Turn ? GameStatus.BlackWin : GameStatus.WhiteWin;
                }
                // Console.Clear();
                Console.WriteLine($"total number of moves made is:{_moves.Count}");
                Console.WriteLine($"move made is {move}");
                _board.PrintBoard();
            }
        }
    }
}