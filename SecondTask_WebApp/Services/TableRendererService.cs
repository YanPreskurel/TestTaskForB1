using SecondTask_WebApp.Data.Repositories;
using SecondTask_WebApp.ViewModels;

namespace SecondTask_WebApp.Services
{
    public class TableRendererService : ITableRenderer
    {
        private readonly IFileRepository _fileRepo;

        public TableRendererService(IFileRepository fileRepo)
        {
            _fileRepo = fileRepo;
        }

        public async Task<TableViewModel> RenderTableAsync(int fileId)
        {
            var file = await _fileRepo.GetFileWithClassesAsync(fileId);

            if (file == null)
                throw new Exception("Файл не найден");

            var table = new TableViewModel
            {
            };

            foreach (var cls in file.Classes)
            {
                foreach (var acc in cls.Accounts)
                {
                    var bal = acc.Balance;

                    table.Rows.Add(new TableRowViewModel
                    {
                        ClassCode = cls.ClassCode,
                        ClassName = cls.ClassName,
                        AccountCode = acc.AccountCode ?? "",
                        AccountName = acc.AccountName,
                        OpeningDebit = bal?.OpeningDebit,
                        OpeningCredit = bal?.OpeningCredit,
                        TurnoverDebit = bal?.TurnoverDebit,
                        TurnoverCredit = bal?.TurnoverCredit,
                        ClosingDebit = bal?.ClosingDebit,
                        ClosingCredit = bal?.ClosingCredit,
                        IsSummary = acc.IsSummary
                    });
                }
            }

            return table;
        }
    }
}
