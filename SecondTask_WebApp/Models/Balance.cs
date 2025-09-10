namespace SecondTask_WebApp.Models
{
    public class Balance
    {
        public int Id { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public decimal? OpeningDebit { get; set; }
        public decimal? OpeningCredit { get; set; }

        public decimal? TurnoverDebit { get; set; }
        public decimal? TurnoverCredit { get; set; }

        public decimal? ClosingDebit { get; set; }
        public decimal? ClosingCredit { get; set; }
    }
}
