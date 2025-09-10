namespace SecondTask_WebApp.ViewModels
{
    public class FileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public DateTime? PeriodFrom { get; set; }    // <- nullable
        public DateTime? PeriodTo { get; set; }      // <- nullable
    }
}
