using ClosedXML.Excel;
using SecondTask_WebApp.Data.Repositories;
using SecondTask_WebApp.Models;
using SecondTask_WebApp.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SecondTask_WebApp.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly IFileRepository _fileRepo;
        private readonly IAccountClassRepository _classRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IBalanceRepository _balanceRepo;

        private static readonly string[] SummaryKeywords = new[] { "итог", "по классу", "итого", "баланс", "сумма", "всего" };

        public ExcelImportService(
            IFileRepository fileRepo,
            IAccountClassRepository classRepo,
            IAccountRepository accountRepo,
            IBalanceRepository balanceRepo)
        {
            _fileRepo = fileRepo;
            _classRepo = classRepo;
            _accountRepo = accountRepo;
            _balanceRepo = balanceRepo;
        }

        // Импорт в БД — использует общий парсер, затем сохраняет через репозитории
        public async Task<FileEntity> ImportAsync(string filePath, string fileName)
        {
            // фоновая CPU/IO-интенсивная операция
            var (fileVm, tableVm) = await Task.Run(() => ParseInternal(filePath, fileName));

            // Создаём FileEntity и сохраняем
            var fileEntity = new FileEntity
            {
                FileName = fileVm.FileName,
                BankName = fileVm.BankName ?? "",
                PeriodFrom = fileVm.PeriodFrom ?? DateTime.MinValue,
                PeriodTo = fileVm.PeriodTo ?? DateTime.MinValue
            };

            await _fileRepo.AddAsync(fileEntity);

            // Группируем строки по классу и записываем
            var classesCache = new Dictionary<string, AccountClass>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in tableVm.Rows)
            {
                var classKey = (row.ClassCode + "|" + row.ClassName).Trim();
                if (!classesCache.TryGetValue(classKey, out var accClass))
                {
                    accClass = new AccountClass
                    {
                        FileEntityId = fileEntity.Id,
                        ClassCode = row.ClassCode ?? "",
                        ClassName = row.ClassName ?? ""
                    };
                    await _classRepo.AddAsync(accClass);
                    classesCache[classKey] = accClass;
                }

                // Создаём Account
                var account = new Account
                {
                    AccountClassId = accClass.Id,
                    AccountCode = row.AccountCode,
                    AccountName = row.AccountName ?? "",
                    IsSummary = row.IsSummary
                };
                await _accountRepo.AddAsync(account);

                // Создаём Balance
                var balance = new Balance
                {
                    AccountId = account.Id,
                    OpeningDebit = row.OpeningDebit,
                    OpeningCredit = row.OpeningCredit,
                    TurnoverDebit = row.TurnoverDebit,
                    TurnoverCredit = row.TurnoverCredit,
                    ClosingDebit = row.ClosingDebit,
                    ClosingCredit = row.ClosingCredit
                };
                await _balanceRepo.AddAsync(balance);
            }

            return fileEntity;
        }

        // Парсинг без записи в БД
        public async Task<(FileViewModel FileInfo, TableViewModel Table)> ParseFileAsync(string filePath, string fileName)
        {
            return await Task.Run(() => ParseInternal(filePath, fileName));
        }

        // Общая логика парсера 
        private (FileViewModel FileInfo, TableViewModel Table) ParseInternal(string filePath, string fileName)
        {
            using var wb = new XLWorkbook(filePath);
            var ws = wb.Worksheet(1); // дотсуп к первому листу

            int headerRow = FindHeaderRow(ws); // поиск заголовка
            if (headerRow == -1)
            {
                headerRow = 7;
            }

            var headerInfo = ReadHeaderInfo(ws);
            var fileVm = new FileViewModel
            {
                FileName = fileName,
                BankName = headerInfo.BankName ?? fileName,
                PeriodFrom = headerInfo.PeriodFrom,
                PeriodTo = headerInfo.PeriodTo
            };

            // Определяем стартовую строку с данными
            int startRow = headerRow + 1;

            if (ws.Row(startRow).CellsUsed().All(c => !IsNumericLike(c.GetString()))) startRow++; // если следующая строка без чисел - пропуск

            var tableVm = new TableViewModel { FileId = 0 };
            AccountClass? currentClass = null;
            var classCache = new Dictionary<string, (string Code, string Name)>(StringComparer.OrdinalIgnoreCase);

            int r = startRow;
            int lastUsed = ws.LastRowUsed()?.RowNumber() ?? r + 1000; //для индексации

            while (r <= lastUsed)
            {
                var rowUsed = ws.Row(r).CellsUsed(); // читаем строку
                if (rowUsed.Count() == 0)
                {
                    break;
                }

                var firstCell = ws.Cell(r, 1);
                var firstText = firstCell.GetString().Trim(); // первая колонка

                // Если строка содержит "КЛАСС" — считаем это началом блока класса
                if (!string.IsNullOrEmpty(firstText) && Regex.IsMatch(firstText, @"\bКЛАСС\b", RegexOptions.IgnoreCase))
                {
                    var m = Regex.Match(firstText, @"\bКЛАСС\b\s*([0-9]+)?\s*(.*)", RegexOptions.IgnoreCase);
                    string classCode = "";
                    string className = firstText;
                    if (m.Success) // успех 
                    {
                        if (!string.IsNullOrWhiteSpace(m.Groups[1].Value))
                            classCode = m.Groups[1].Value.Trim(); // то пишем код
                        if (!string.IsNullOrWhiteSpace(m.Groups[2].Value))
                            className = m.Groups[2].Value.Trim(); // пишем имя класса
                    }

                    var key = (classCode + "|" + className).Trim(); // расширяем словарь
                    if (!classCache.ContainsKey(key)) // чтобв не перезаписать существующие данные
                        classCache[key] = (classCode, className);

                    currentClass = new AccountClass { ClassCode = classCode, ClassName = className, FileEntityId = 0 }; // создаем класс

                    r++; // итеририруемся по строке и если данный блок завершен то идем к следующему индексу строки
                    continue; 
                }

                var accountCodeRaw = ws.Cell(r, 1).GetString().Trim();
                if (string.IsNullOrEmpty(accountCodeRaw))
                {
                    var val = ws.Cell(r, 1).Value; // первая колнка у ряда после Класса
                    if (val is double || val is int || val is decimal)
                        accountCodeRaw = ws.Cell(r, 1).GetValue<double>().ToString(CultureInfo.InvariantCulture); //парсим код счета
                }
                var accountCode = NormalizeAccountCode(accountCodeRaw); // превращаем в удобную строку типа 10.0 к 10
                var accountName = ""; 

                // определяем IsSummary
                bool isSummary = false;
                var lowerName = (accountName ?? "").ToLowerInvariant(); // если наименование заменило код в колонке
                if (SummaryKeywords.Any(k => lowerName.Contains(k))) isSummary = true;
                try
                {
                    if (ws.Cell(r, 1).Style.Font.Bold) isSummary = true; // если жирным шрифт пометим это
                }
                catch 
                {
                    throw;
                }

                // читаем числовые колонки 
                decimal? openingDebit = SafeGetDecimal(ws.Cell(r, 2));
                decimal? openingCredit = SafeGetDecimal(ws.Cell(r, 3));
                decimal? turnoverDebit = SafeGetDecimal(ws.Cell(r, 4));
                decimal? turnoverCredit = SafeGetDecimal(ws.Cell(r, 5));
                decimal? closingDebit = SafeGetDecimal(ws.Cell(r, 6));
                decimal? closingCredit = SafeGetDecimal(ws.Cell(r, 7));

                string classCodeForRow = currentClass?.ClassCode ?? "";
                string classNameForRow = currentClass?.ClassName ?? "";

                tableVm.Rows.Add(new TableRowViewModel
                {
                    ClassCode = classCodeForRow,
                    ClassName = classNameForRow,
                    AccountCode = accountCode,
                    AccountName = accountName,
                    OpeningDebit = openingDebit,
                    OpeningCredit = openingCredit,
                    TurnoverDebit = turnoverDebit,
                    TurnoverCredit = turnoverCredit,
                    ClosingDebit = closingDebit,
                    ClosingCredit = closingCredit,
                    IsSummary = isSummary
                });

                r++; // итерируемся
            }

            return (fileVm, tableVm); // дальше возвратим модели для вывода 
        }

        private int FindHeaderRow(IXLWorksheet ws)
        {
            int maxRow = Math.Min(50, ws.LastRowUsed()?.RowNumber() ?? 50); // проходит первые 50 строк и 10 колонок
            for (int r = 1; r <= maxRow; r++)
            {
                for (int c = 1; c <= 10; c++)
                {
                    var s = ws.Cell(r, c).GetString();
                    if (string.IsNullOrWhiteSpace(s)) continue;

                    var upper = s.Trim().ToUpperInvariant();
                    if (upper.Contains("Б/СЧ")) return r;
                }
            }
            return -1;
        }

        private (string? BankName, DateTime? PeriodFrom, DateTime? PeriodTo) ReadHeaderInfo(IXLWorksheet ws) // первые 20 срок ищет период
        {
            string? bank = null;
            DateTime? from = null;
            DateTime? to = null;
            int maxRow = Math.Min(20, ws.LastRowUsed()?.RowNumber() ?? 20);
            var periodRegex = new Regex(@"за\s*период\s*с\s*(\d{1,2}\.\d{1,2}\.\d{4})\s*по\s*(\d{1,2}\.\d{1,2}\.\d{4})", RegexOptions.IgnoreCase); // регулряка дял поиска по шаблону

            for (int r = 1; r <= maxRow; r++) // идем по срокам
            {
                for (int c = 1; c <= 8; c++) // по колонкам
                {
                    var s = ws.Cell(r, c).GetString();
                    if (string.IsNullOrWhiteSpace(s)) continue; // проауск пустых строк

                    var m = periodRegex.Match(s); // если сопадение то заходим и парсим нужное
                    if (m.Success)
                    {
                        if (DateTime.TryParseExact(m.Groups[1].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1))
                            from = d1;
                        if (DateTime.TryParseExact(m.Groups[2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2))
                            to = d2;
                    }

                    var low = s.ToLowerInvariant();
                    if (low.Contains("банк") || low.Contains("bank") || low.Contains("Название банка")) // наименование банка форма расплывчата
                    {
                        var cand = ws.Cell(r, c).GetString().Trim();
                        if (!string.IsNullOrWhiteSpace(cand))
                            bank = cand;
                    }
                }
            }

            if (from == null || to == null) // если не спарсили то 
            {
                var dateRegex = new Regex(@"\d{1,2}\.\d{1,2}\.\d{4}");
                for (int r = 1; r <= maxRow; r++)
                {
                    for (int c = 1; c <= 8; c++)
                    {
                        var s = ws.Cell(r, c).GetString();
                        if (string.IsNullOrWhiteSpace(s)) continue;
                        var ms = dateRegex.Matches(s);
                        if (ms.Count >= 2)
                        {
                            if (DateTime.TryParseExact(ms[0].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1))
                                from = d1;
                            if (DateTime.TryParseExact(ms[1].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2))
                                to = d2;
                        }
                    }
                }
            }

            return (bank, from, to); // кортеж с названием и периодами
        }

        private decimal? SafeGetDecimal(IXLCell cell) // попытка безопасно прочесть число
        {
            if (cell == null || cell.IsEmpty()) 
                return null;

            try
            {
                if (cell.DataType == XLDataType.Number) // если число
                {
                    var d = cell.GetValue<double>();
                    return Convert.ToDecimal(d);
                }

                var s = cell.GetString().Trim(); // если строка
                if (string.IsNullOrEmpty(s)) 
                    return null;

                s = s.Replace(" ", "").Replace("\u00A0", "").Replace(",", ".");

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var res))
                    return res;

                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var dd))
                    return Convert.ToDecimal(dd);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении ячейки {cell.Address}: {ex.Message}");
                return null;
            }
            return null;
        }

        private static bool IsNumericLike(string s) // проверка, что похоже на число
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            s = s.Trim();

            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        }

        private string NormalizeAccountCode(string raw) // общий метод убирает лишние пробелы, есл чсило то приводит к целому, если целый, если не число убирает внутринние пробелы
        {
            if (string.IsNullOrWhiteSpace(raw)) 
                return "";
            raw = raw.Trim();

            if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            {
                if (Math.Abs(d - Math.Round(d)) < 1e-9)
                    return ((long)Math.Round(d)).ToString();
                else
                    return d.ToString(CultureInfo.InvariantCulture);
            }

            return Regex.Replace(raw, @"\s+", "");
        }
    }
}
