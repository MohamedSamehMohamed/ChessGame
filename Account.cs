namespace ChessGame
{
    public class Account
    {
        private int id;
        private string password;
        private AccountStatus status;

        public bool resetPassword()
        {
            return true;
        }
    }
    public class Player : Account {
        private Person person;
        private int _totalGamesPlayed = 0;
    }

    public class Admin : Account {
        public bool blockUser()
        {
            return true;
        }
    }
}