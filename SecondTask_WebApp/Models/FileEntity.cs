namespace SecondTask_WebApp.Models
{
    public class FileEntity
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public List<Account> Accounts { get; set; } = new();
    }
}
