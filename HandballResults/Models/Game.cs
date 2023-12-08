using System;

namespace HandballResults.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public int GameNr { get; set; }
        public DateTime GameDateTime { get; set; }
        public string GameTypeLong { get; set; } = string.Empty;
        public string GameTypeShort { get; set; } = string.Empty;
        public int TeamAId { get; set; }
        public string TeamAName { get; set; } = string.Empty;
        public int TeamBId { get; set; }
        public string TeamBName { get; set; } = string.Empty;
        public string LeagueLong { get; set; } = string.Empty;
        public string LeagueShort { get; set; } = string.Empty;
        public string Round { get; set; } = string.Empty;
        public int GameStatusId { get; set; }
        public string GameStatus { get; set; } = string.Empty;
        public int? TeamAScoreHt { get; set; }
        public int? TeamBScoreHt { get; set; }
        public int? TeamAScoreFt { get; set; }
        public int? TeamBScoreFt { get; set; }
        public string Venue { get; set; } = string.Empty;
        public string VenueAddress { get; set; } = string.Empty;
        public int? VenueZip { get; set; }
        public string VenueCity { get; set; } = string.Empty;
        public int? Spectators { get; set; }
        public int? RoundNr { get; set; }

    }
}