using System;

namespace ChessGame
{
    public static class CommonMethods{
        public static bool _canWalk(ChessBoard board, Move move, int[] dx, int[] dy)
        {
            var index = -1;
            for (var i = 0; index == -1 && i < dx.Length; i++)
            {
                try
                {
                    var newRow = move.Source.Row + dx[i];
                    var newColumn = move.Source.Column + dy[i];
                    while (newRow >= 0 && newRow < 8 && newColumn >= 0 && newColumn < 8)
                    {
                        if (newRow == move.Destination.Row && newColumn == move.Destination.Column)
                        {
                            index = i;
                            break;
                        }

                        var inPathPiece = board.GetPiece(new Position(newRow, newColumn));
                        if (inPathPiece is Empty)
                        {
                            // ok cool 
                        }
                        else
                        {
                            // can't cross a piece 
                            break;
                        }

                        newRow += dx[i];
                        newColumn += dy[i];
                    }
                }
                catch
                {
                    continue;
                }
            }
            return index != -1;
        }
    }
    public abstract class Piece
    {
        public bool Killed { get; set; } = false;
        public bool White { get; set; } = false;
    
        public bool CanMoveLogical(ChessBoard board, Move move)
        {
            var sourcePiece = board.GetPiece(move.Source);
            var destinationPiece = board.GetPiece(move.Destination);
            if (sourcePiece is Empty) return false;
            if (destinationPiece is Empty) return true;
            return sourcePiece.White != destinationPiece.White;
        }
        public abstract bool CanMove(ChessBoard board, Move move);
    }
    
    public class Empty : Piece
    {
        public override string ToString()
        {
            return "-";
        }
        public override bool CanMove(ChessBoard board, Move move)
        {
            throw new System.NotImplementedException();
        }
    }
    public class King : Piece 
    {
        private bool castlingDone = false;

        public override string ToString()
        {
            return "K";
        }

        
        public override bool CanMove(ChessBoard board, Move move)
        {
            var can = IsNeigbour(move);
            // king can't reach this square 
            if (!can) return false;
            
            // check if the destination square is attacked
            var myPiece = board.GetPiece(move.Source);
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var currentPiece = board.GetPiece(new Position(i, j));
                    if (currentPiece is Empty || currentPiece.White == myPiece.White)
                        continue;
                    if (currentPiece is King)
                    {
                        if (IsNeigbour(new Move(new Position(i, j), move.Source)))
                            return false;
                    }
                    if (currentPiece.CanMove(board, new Move(new Position(i, j), move.Source)))
                        return false;
                }
            }

            return true;
        }

        private static bool IsNeigbour(Move move)
        {
            var dx = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
            var dy = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };
            bool can = false;
            for (var i = 0; i < 8; i++)
            {
                var newRow = move.Source.Row + dx[i];
                var newColumn = move.Source.Column + dy[i];
                if (newRow != move.Destination.Row || newColumn != move.Destination.Column)
                    continue;
                can = true;
                break;
            }

            return can;
        }
    }
    
    public class Queen : Piece {
        public override string ToString()
        {
            return "Q";
        }

        public override bool CanMove(ChessBoard board, Move move)
        {
            var dx = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
            var dy = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };
            return CommonMethods._canWalk(board, move, dx, dy);
        }
    }

    public class Knight : Piece {
        public override string ToString()
        {
            return "N";
        }

        public override bool CanMove(ChessBoard board, Move move)
        {
            var rowDiff = Math.Abs(move.Destination.Row - move.Source.Row);
            var columnDiff = Math.Abs(move.Destination.Column - move.Source.Column);
            return rowDiff + columnDiff == 3 && Math.Max(rowDiff, columnDiff) == 2;
        }
    }

    public class Bishop : Piece {
        public override string ToString()
        {
            return "B";
        }

        public override bool CanMove(ChessBoard board, Move move)
        {
            var dx = new[] { -1, -1, 1, 1 };
            var dy = new[] { -1, 1, -1, 1 };
            return CommonMethods._canWalk(board, move, dx, dy);
        }
    }

    public class Rook : Piece {
        public override string ToString()
        {
            return "R";
        }

        public override bool CanMove(ChessBoard board, Move move)
        {
            var dx = new[] {-1, 0, 0,1};
            var dy = new[] {0, -1, 1, 0};
            return CommonMethods._canWalk(board, move, dx, dy);
        }
    }

    public class Pawn : Piece
    {
        public override string ToString()
        {
            return "P";
        }

        public override bool CanMove(ChessBoard board, Move move)
        {
            var myPiece = board.GetPiece(move.Source);
            var ourTarget = board.GetPiece(move.Destination);
            var can = false;
            for (var i = -1; i <= 1; i++)
            {
                var newColumn = i + move.Source.Column;
                var newRow = move.Source.Row + (myPiece.White? -1: 1);
                var valid = true;
                if (move.Destination.Row != newRow || move.Destination.Column != newColumn)
                {
                    if (i != 0)
                        continue;
                    valid = false;
                }

                if (!valid)
                {
                    newRow += (myPiece.White ? -1 : 1);
                    if (move.Destination.Row != newRow || move.Destination.Column != newColumn)
                        continue;
                }
                if (i == 0)
                {
                    if (ourTarget is Empty)
                    {
                        can = true;
                    }
                    break;
                }

                if (ourTarget is Empty)
                    break;
                if (ourTarget.White == myPiece.White)
                    break;
                can = true;
                break;
            }
            return can;
        }
    }
}