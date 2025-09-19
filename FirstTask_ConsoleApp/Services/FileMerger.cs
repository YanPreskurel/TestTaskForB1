using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTask_ConsoleApp.Services
{
    public static class FileMerger
    {
        public static long MergeFiles(string folder, string outputFile, string filter)
        {
            var files = Directory.GetFiles(folder, "*.txt") // берем все файлы сортируем и превращаем в массив
                                 .OrderBy(x => x)
                                 .ToArray();

            long removedCount = 0; // сроки, которые удалили фильтром
            long writtenCount = 0; // сколько записали

            using var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20);

            using var writer = new StreamWriter(fs, Encoding.UTF8);

            foreach (var file in files) // цикл по файлам
            {
                foreach (var line in File.ReadLines(file)) // по строкам
                {
                    if(!string.IsNullOrEmpty(filter) && line.Contains(filter)) // если фильтр не пустой и строка содержит фильтр
                    {
                        removedCount++;
                        continue; // пропускаем запись
                    }

                    writer.WriteLine(line); // пишем в итоговый файл

                    writtenCount++;
                }
            }
             
            return removedCount;
        }
    }
}
