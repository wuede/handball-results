using System.Collections.Generic;

namespace HandballResults.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupText { get; set; }
        public int TotalTeams { get; set; }
        public int PromotionCandidate { get; set; }
        public int RelegationCandidate { get; set; }
        public int DirectPromotion { get; set; }
        public int DirectRelegation { get; set; }
        public string LeagueLong { get; set; }
        public string LeagueShort { get; set; }
        public int LeagueId { get; set; }
        public string Modus { get; set; }
        public int TotalRounds { get; set; }
        public int GamesPerRound { get; set; }
        public string ModusHtml { get; set; }
        public List<Ranking> Ranking { get; set; }
    }
}