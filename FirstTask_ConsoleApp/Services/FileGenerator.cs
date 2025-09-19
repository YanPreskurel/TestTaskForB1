using FirstTask_ConsoleApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace FirstTask_ConsoleApp.Services
{
    public static class FileGenerator
    {
        public static void GenerateFiles(string folder, int fileCount, int rowsPerFile)
        {
            Directory.CreateDirectory(folder); // создание папки при ее отсутствии

            for (int fileIndex = 1; fileIndex <= fileCount; fileIndex++)
            {
                string path = Path.Combine(folder, $"file_{fileIndex:D3}.txt"); // шаблон создания файлов
                // размер буфера 1 МБ для того чтобы не делать много записей на диск, а через него сбрасываем на диск тем самым уменьшаем кол-во обращений
                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20); // создание потока записи, пересоздает фалй, если был, только для записи, файл не может быть открыт другими потокамм

                using var writer = new StreamWriter(fs, Encoding.UTF8); // удобная обертка пишет текст в формате для кириллицы

                for(int row = 0; row < rowsPerFile; row++)
                {
                    var date = RandomDataGenerator.RandomDateLast5Years();
                    var latin = RandomDataGenerator.RandomLatinString(10);
                    var cyr = RandomDataGenerator.RandomCyrillicString(10);
                    var even = RandomDataGenerator.RandomEvenInt();
                    var dbl = RandomDataGenerator.RandomDouble();


                    string line =
                        $"{date:dd.MM.yyyy}||" +
                        $"{latin}||" +
                        $"{cyr}||" +
                        $"{even}||" +
                        $"{RandomDataGenerator.DoubleToStringInvariant(dbl)}||";

                    writer.WriteLine(line);
                }
            }
        }
    }
}
