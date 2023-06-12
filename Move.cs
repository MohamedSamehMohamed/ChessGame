namespace ChessGame
{
    public class Move
    {
        public Position Source { get; set; }
        public Position Destination { get; set; }

        public Move(Position source, Position destination)
        {
            Source = source;
            Destination = destination;
        }

        public override string ToString()
        {
            return $"from {Source} to {Destination}";
        }
    }
}