namespace HandballResults.Models
{
    public class Ranking
    {
        public int Rank { get; set; }
        public string TeamName { get; set; }
        public int TotalPoints { get; set; }
        public int TotalWins { get; set; }
        public int TotalLoss { get; set; }
        public int TotalDraws { get; set; }
        public int TotalScoresPlus { get; set; }
        public int TotalScoresMinus { get; set; }
        public int TotalGames { get; set; }
        public int TotalScoresDiff { get; set; }
    }
}