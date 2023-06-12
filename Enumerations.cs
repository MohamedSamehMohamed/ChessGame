using System;

namespace ChessGame
{
    enum GameStatus
    {
        Active,
        BlackWin,
        WhiteWin,
        Forfeit,
        Stalemate,
        Resignation
    }
    enum AccountStatus {
        Active, 
        Closed, 
        Canceled, 
        Blacklisted, 
        None
    }
    public class Person {
        private String name;
        private String streetAddress;
        private String city;
        private String state;
        private int zipCode;
        private String country;
    }
}