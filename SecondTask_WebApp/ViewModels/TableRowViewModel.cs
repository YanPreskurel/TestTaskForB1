namespace SecondTask_WebApp.ViewModels
{
    public class TableRowViewModel
    {
        public string ClassCode { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";

        public decimal? OpeningDebit { get; set; }
        public decimal? OpeningCredit { get; set; }
        public decimal? TurnoverDebit { get; set; }
        public decimal? TurnoverCredit { get; set; }
        public decimal? ClosingDebit { get; set; }
        public decimal? ClosingCredit { get; set; }

        public bool IsSummary { get; set; }
    }
}
