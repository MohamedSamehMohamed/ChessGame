using System;

namespace ChessGame
{
    public static class CommonMethods{
        public static bool IsNeigbour(Move move)
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
        public static bool IsSquareAttacked(ChessBoard board, Position squarePosition, bool whiteCanAttack)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var currentPiece = board.GetPiece(new Position(i, j));
                    if (currentPiece is Empty || currentPiece.White != whiteCanAttack)
                        continue;
                    if (currentPiece is King)
                    {
                        if (IsNeigbour(new Move(new Position(i, j), squarePosition)))
                            return true;
                    }
                    else
                    {
                        if (currentPiece.CanMove(board, new Move(new Position(i, j), squarePosition)))
                            return true;
                    }
                }
            }
            return false;
        }
        public static bool _canWalk(ChessBoard board, Move move, int[] dx, int[] dy, bool walkWithNocheck = false)
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
                            if (walkWithNocheck)
                            {
                               // check if there is any piece can attack this position 
                               if (IsSquareAttacked(board, new Position(newRow, newColumn),
                                       !board.GetPiece(move.Source).White))
                                   return false;
                            }
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
        public bool FirstMove { get; set; } = true;
    
        public virtual bool CanMoveLogical(ChessBoard board, Move move)
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
        public override string ToString()
        {
            return White? "K": "k";
        }
        public override bool CanMoveLogical(ChessBoard board, Move move)
        {
            // base CanMoveLogical will return false in case of castle because src, des are of same color  
            var sourcePiece = board.GetPiece(move.Source);
            return !(sourcePiece is Empty);
        }
        public bool IsCastleMove(ChessBoard board, Move move)
        {
            var source = board.GetPiece(move.Source);
            var destination = board.GetPiece(move.Destination);
            if (!(destination is Rook))
                return false;
            if (source.FirstMove == false || destination.FirstMove == false)
                return false;
            var dx = new[] {0, 0};
            var dy = new[] {-1, 1};
            return CommonMethods._canWalk(board, move, dx, dy, true);
        }
        public override bool CanMove(ChessBoard board, Move move)
        {
            if (IsCastleMove(board, move))
                return true;
            var can = CommonMethods.IsNeigbour(move);
            // king can't reach this square 
            if (!can) return false;
            
            // check if the destination square is attacked
            var myPiece = board.GetPiece(move.Source);
            return !CommonMethods.IsSquareAttacked(board, move.Destination, myPiece.White);
        }
    }
    
    public class Queen : Piece {
        public override string ToString()
        {
            return White? "Q": "q";
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
            return White? "N": "n";
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
            return White? "B": "b";
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
            return White? "R": "r";
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
            return White? "P": "p";
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
                    if (myPiece.FirstMove == false)
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