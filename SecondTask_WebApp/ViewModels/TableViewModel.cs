namespace SecondTask_WebApp.ViewModels
{
    public class TableViewModel
    {
        public int FileId { get; set; }
        public List<TableRowViewModel> Rows { get; set; } = new();
    }
}
