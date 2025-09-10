namespace SecondTask_WebApp.Models
{
    public class Account
    {
        public int Id { get; set; }

        public int FileEntityId { get; set; }
        public FileEntity FileEntity { get; set; } = null!;

        public string? AccountCode { get; set; }
        public string AccountName { get; set; } = null!;

        public Balance Balance { get; set; } = null!;
    }
}
