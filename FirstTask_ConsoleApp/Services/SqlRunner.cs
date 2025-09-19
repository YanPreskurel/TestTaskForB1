using System;                      
using System.IO;                   
using System.Text;                 
using System.Collections.Generic;  
using Microsoft.Data.SqlClient;    

namespace FirstTask_ConsoleApp.Services
{

    public static class SqlRunner
    {
        public static void ExecuteSqlScript(string connectionString, string scriptPath, Action<string>? log = null) // выполняет скрипт по блокам
        {
            string sql = File.ReadAllText(scriptPath, Encoding.UTF8); // читает файл в строку
            var batches = SplitByGo(sql); // разбивает скрипт на блоки по GO

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            int i = 0;

            foreach (var batch in batches)
            {
                i++;

                if (string.IsNullOrWhiteSpace(batch))
                    continue;

                using var cmd = conn.CreateCommand();
                cmd.CommandText = batch;
                cmd.CommandTimeout = 600;

                using var reader = cmd.ExecuteReader();

                // Если есть строки — выводим их
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        log?.Invoke(string.Join(" | ", values));
                    }
                }
                else
                {
                    // Если SELECT не вернул строки — просто выполняем как NonQuery
                    int affected = reader.RecordsAffected;
                    if (affected >= 0)
                        log?.Invoke($"Выполнено {affected} строк");
                }


                log?.Invoke($"Выполнен блок {i}/{batches.Length}");
            }

            log?.Invoke("Выполнение SQL-скрипта завершено.");
        }

        private static string[] SplitByGo(string sql) // передаем текст скрипта 
        {
            var lines = sql.Replace("\r\n", "\n").Split('\n'); // разбиваем на массив строк
            var list = new List<string>(); // хранит готовые блоки скрипта
            var sb = new StringBuilder(); // собирает строки временно в один блок до GO

            foreach (var line in lines)
            {
                if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase)) //  сранвение без учета регистра
                {
                    list.Add(sb.ToString()); // добавляем блок если стретили GO без учета регистра
                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(line); // собираем строку дальше
                }
            }

            if (sb.Length > 0) // добавление последнего блока
                list.Add(sb.ToString());

            return list.ToArray();
        }
    }
}
