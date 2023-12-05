namespace HandballResults.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupText { get; set; } = string.Empty;
        public int TotalTeams { get; set; }
        public int PromotionCandidate { get; set; }
        public int RelegationCandidate { get; set; }
        public int DirectPromotion { get; set; }
        public int DirectRelegation { get; set; }
        public string LeagueLong { get; set; } = string.Empty;
        public string LeagueShort { get; set; } = string.Empty;
        public int LeagueId { get; set; }
        public string Modus { get; set; } = string.Empty;
        public int TotalRounds { get; set; }
        public int GamesPerRound { get; set; }
        public string ModusHtml { get; set; } = string.Empty;
        public List<Ranking> Ranking { get; set; } = new List<Ranking>();
    }
}