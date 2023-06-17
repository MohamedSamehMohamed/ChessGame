namespace ChessGame
{
    public class Move
    {
        public Position Source { get; set; }
        public Position Destination { get; set; }
        public Piece CapturedPiece { get; set; } = new Empty();
        public bool IsSourceFirstMove { get; set; } = true;

        public Move(Position source, Position destination)
        {
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"from {Source} to {Destination} and capture {CapturedPiece}";
        }
        
    }
}