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
            var files = Directory.GetFiles(folder, "*.txt")
                                 .OrderBy(x => x)
                                 .ToArray();

            long removedCount = 0;
            long writtenCount = 0;

            using var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20);

            using var writer = new StreamWriter(fs, Encoding.UTF8);

            int inx = 0;

            foreach (var file in files)
            {
                inx++;

                foreach (var line in File.ReadLines(file))
                {
                    if(!string.IsNullOrEmpty(filter) && line.Contains(filter))
                    {
                        removedCount++;
                        continue;
                    }

                    writer.WriteLine(line);

                    writtenCount++;
                }
            }
             
            return removedCount;
        }
    }
}
