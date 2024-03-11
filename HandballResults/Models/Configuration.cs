namespace HandballResults.Models
{
    public class Configuration
    {
        public string ShvApiKey { get; set; } = "";
        public List<int> TeamIdWhiteList { get; set; } = new List<int>();
    }
}
