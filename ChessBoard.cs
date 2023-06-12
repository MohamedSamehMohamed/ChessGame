using System;
using System.Collections.Generic;

namespace ChessGame
{
    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position(int row, int column)
        {
            if (row < 0 || row >= 8) throw new IndexOutOfRangeException();
            if (column < 0 || column >= 8) throw new IndexOutOfRangeException();
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Convert.ToChar('a' + Column)}{8 - Row}";
        }
    }
    public class Box
    {
        public Position Position { get; set; }
        public Piece Piece { get; set; }

        public Box()
        {
            Piece = new Empty();
        }

        public Box(Piece piece, Position position)
        {
            Piece = piece;
            Position = position;
        }
        
    }
    public class ChessBoard
    {
        private Box[][] boxes = new Box[8][];

        public ChessBoard()
        {
            for (var i = 0; i < 8; i++)
            {
                boxes[i] = new Box[8];
            }
            BuildBoard();
        }

        public void ResetBoard()
        {
            for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                {
                    boxes[i][j] = new Box();
                }
        }

        public void BuildBoard()
        {
            ResetBoard();
            for (var i = 0; i < 8; i++)
            {
                boxes[1][i] = new Box(new Pawn(), new Position(1, i));
                boxes[6][i] = new Box(new Pawn{White = true}, new Position(6, i));
            }
            boxes[0][0] = new Box(new Rook(), new Position(0, 0));
            boxes[0][7] = new Box(new Rook (), new Position(0, 7));

            boxes[0][1] = new Box(new Knight (), new Position(0, 1));
            boxes[0][6] = new Box(new Knight (), new Position(0, 6));

            boxes[0][2] = new Box(new Bishop (), new Position(0, 2));
            boxes[0][5] = new Box(new Bishop (), new Position(0, 5));

            boxes[0][4] = new Box(new King (), new Position(0, 4));
            boxes[0][3] = new Box(new Queen (), new Position(0, 3));
            
            
            boxes[7][0] = new Box(new Rook { White = true }, new Position(0, 0));
            boxes[7][7] = new Box(new Rook { White = true }, new Position(0, 7));

            boxes[7][1] = new Box(new Knight { White = true }, new Position(0, 1));
            boxes[7][6] = new Box(new Knight { White = true }, new Position(0, 6));

            boxes[7][2] = new Box(new Bishop { White = true }, new Position(0, 2));
            boxes[7][5] = new Box(new Bishop { White = true }, new Position(0, 5));

            boxes[7][4] = new Box(new King { White = true }, new Position(0, 4));
            boxes[7][3] = new Box(new Queen { White = true }, new Position(0, 3));
        }
        public void PrintBoard()
        {
            for (var i = 0; i < 8; i++)
            {
                Console.Write(8 - i);
                Console.Write(' ');
                for (var j = 0; j < 8; j++)
                {
                    Console.Write(boxes[i][j].Piece);
                    Console.Write(j + 1 < 8? ' ': '\n');
                }
            }
            Console.Write("# ");
            for (var i = 0; i < 8; i++)
            {
                Console.Write( Convert.ToChar('a' + i) );
                Console.Write(i + 1 < 8 ? ' ' : '\n');
            }
        }

        public void UpdateBoard(Position position, Piece piece)
        {
            boxes[position.Row][position.Column] = new Box(piece, position);
        }

        public Piece GetPiece(Position position)
        {
            return boxes[position.Row][position.Column].Piece;
        }
    }
}