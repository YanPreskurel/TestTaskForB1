using SecondTask_WebApp.Models;

public class FileEntity
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public DateTime? PeriodFrom { get; set; }   // <- nullable
    public DateTime? PeriodTo { get; set; }     // <- nullable
    public List<AccountClass> Classes { get; set; } = new();
}
