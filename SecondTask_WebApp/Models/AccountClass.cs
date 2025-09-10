namespace SecondTask_WebApp.Models
{
    public class AccountClass
    {
        public int Id { get; set; }
        public int FileEntityId { get; set; }
        public FileEntity FileEntity { get; set; } = null!;

        public string ClassCode { get; set; } = null!; 
        public string ClassName { get; set; } = null!; 

        public List<Account> Accounts { get; set; } = new();
    }
}
