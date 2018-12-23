using System;

namespace HandballResults.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public int GameNr { get; set; }
        public DateTime GameDateTime { get; set; }
        public string GameTypeLong { get; set; }
        public string GameTypeShort { get; set; }
        public string TeamAName { get; set; }
        public string TeamBName { get; set; }
        public string LeagueLong { get; set; }
        public string LeagueShort { get; set; }
        public string Round { get; set; }
        public string GameStatus { get; set; }
        public int TeamAScoreHt { get; set; }
        public int TeamBScoreHt { get; set; }
        public int TeamAScoreFt { get; set; }
        public int TeamBScoreFt { get; set; }
        public string Venue { get; set; }
        public string VenueAddress { get; set; }
        public int VenueZip { get; set; }
        public string VenueCity { get; set; }
        public int Spectators { get; set; }
        public int RoundNr { get; set; }

    }
}