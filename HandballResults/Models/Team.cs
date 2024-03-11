namespace HandballResults.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = "";
        public int LeagueId { get; set; }
        public string GroupText { get; set; } = "";
    }
}