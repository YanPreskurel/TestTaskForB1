using System;                      
using System.IO;                   
using System.Text;                 
using System.Collections.Generic;  
using Microsoft.Data.SqlClient;    

namespace FirstTask_ConsoleApp.Services
{

    public static class SqlRunner
    {
        public static void ExecuteSqlScript(string connectionString, string scriptPath, Action<string>? log = null)
        {
            string sql = File.ReadAllText(scriptPath, Encoding.UTF8);
            var batches = SplitByGo(sql);

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

                if (batch.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var values = new object[reader.FieldCount];
                        reader.GetValues(values);
                        log?.Invoke(string.Join(" | ", values));
                    }
                }
                else
                {
                    cmd.ExecuteNonQuery();
                }

                log?.Invoke($"Выполнен блок {i}/{batches.Length}");
            }

            log?.Invoke("Выполнение SQL-скрипта завершено.");
        }

        private static string[] SplitByGo(string sql)
        {
            var lines = sql.Replace("\r\n", "\n").Split('\n');
            var list = new List<string>();
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            if (sb.Length > 0)
                list.Add(sb.ToString());

            return list.ToArray();
        }
    }
}
