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
            Directory.CreateDirectory(folder);

            for (int fileIndex = 1; fileIndex <= fileCount; fileIndex++)
            {
                string path = Path.Combine(folder, $"file_{fileIndex:D3}.txt");

                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20);

                using var writer = new StreamWriter(fs, Encoding.UTF8);

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
