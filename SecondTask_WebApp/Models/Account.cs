namespace SecondTask_WebApp.Models
{
    public class Account
    {
        public int Id { get; set; }

        public int AccountClassId { get; set; }
        public AccountClass AccountClass { get; set; } = null!;

        public string? AccountCode { get; set; }
        public string AccountName { get; set; } = null!;

        public bool IsSummary { get; set; }
        public Balance Balance { get; set; } = null!;
    }
}
