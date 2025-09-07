using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTask_ConsoleApp.Services
{
    public static class DatabaseImporter
    {
        private const int BatchSize = 10_000;

        public static void ImportToSqlServer(string connectionString, string tableName, string inputFile, Action<string>? log = null)
        {
            EnsureTableExists(connectionString, tableName, log);

            int processed = 0;
            int total = -1;

            try 
            { 
                total = File.ReadLines(inputFile).Count();
            } 
            catch
            { 
                total = -1;
            }

            var table = CreateDataTable();

            foreach (var line in File.ReadLines(inputFile))
            {
                processed++;
                var parts = line.Split(new string[] { "||" }, StringSplitOptions.None);
                if (parts.Length < 5) continue;

                if (!TryParseRow(parts, out var parsed))
                    continue;

                table.Rows.Add(parsed.Date, parsed.Latin, parsed.Cyrillic, parsed.IntValue, parsed.FloatValue);

                // Прогресс по строкам
                if (processed % 5000 == 0)
                {
                    log?.Invoke($"Обработано {processed}{(total > 0 ? $" из {total}" : "")} строк...");
                }

                // Запись батчами
                if (table.Rows.Count >= BatchSize)
                {
                    BulkWrite(connectionString, tableName, table, log);
                    table.Clear();
                    log?.Invoke($"В БД добавлен батч (всего {processed}{(total > 0 ? $" из {total}" : "")})");
                }
            }


            if (table.Rows.Count > 0)
            {
                BulkWrite(connectionString, tableName, table, log);
                log?.Invoke($"Импортировано {processed} строк{(total > 0 ? $" из {total}" : "")}");
            }

            log?.Invoke("Импорт в БД завершён.");
        }

        private static DataTable CreateDataTable()
        {
            var t = new DataTable();

            t.Columns.Add("Date", typeof(DateTime));
            t.Columns.Add("Latin", typeof(string));
            t.Columns.Add("Cyrillic", typeof(string));
            t.Columns.Add("IntValue", typeof(int));
            t.Columns.Add("FloatValue", typeof(decimal)); // будем копировать в DECIMAL(18,8)

            return t;
        }

        private static void BulkWrite(string connectionString, string tableName, DataTable table, Action<string>? log)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var bulk = new SqlBulkCopy(conn)
            {
                DestinationTableName = tableName,
                BatchSize = table.Rows.Count,
                BulkCopyTimeout = 600
            };

            bulk.ColumnMappings.Add("Date", "Date");
            bulk.ColumnMappings.Add("Latin", "Latin");
            bulk.ColumnMappings.Add("Cyrillic", "Cyrillic");
            bulk.ColumnMappings.Add("IntValue", "IntValue");
            bulk.ColumnMappings.Add("FloatValue", "FloatValue");

            bulk.WriteToServer(table);

            log?.Invoke($"Батч ({table.Rows.Count}) записан в таблицу {tableName}");
        }

        private static bool TryParseRow(string[] parts, out (DateTime Date, string Latin, string Cyrillic, int IntValue, decimal FloatValue) result)
        {
            result = default;

            string dateStr = parts[0];
            string latin = parts[1];
            string cyr = parts[2];
            string intStr = parts[3];
            string dblStr = parts[4];

            if (!DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return false;

            if (!int.TryParse(intStr, out var iv))
                return false;

            dblStr = dblStr.Replace(',', '.');
            if (!decimal.TryParse(dblStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var dv))
                return false;

            result = (date, latin, cyr, iv, dv);

            return true;
        }

        private static void EnsureTableExists(string connectionString, string tableName, Action<string>? log)
        {
            var sql = $@"
                         IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{tableName}]') AND type in (N'U'))
                            BEGIN
                                CREATE TABLE [dbo].[{tableName}](
                                    [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                    [Date] DATE NOT NULL,
                                    [Latin] NVARCHAR(50) NULL,
                                    [Cyrillic] NVARCHAR(50) NULL,
                                    [IntValue] INT NULL,
                                    [FloatValue] DECIMAL(18,8) NULL
                                );
                            END
            ";
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            log?.Invoke($"Таблица {tableName} создана.");
        }
    }
}
