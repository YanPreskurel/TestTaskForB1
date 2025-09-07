using FirstTask_ConsoleApp.Services;
class Program
{
    // строка подключения к SQL Server — поменяй под свою БД
    private static string connectionString =
        "Server=localhost\\SQLEXPRESS;Database=TestDB;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string tableName = "MyTable";   // имя таблицы в базе
    private const string dataFolder = "Data";     // папка для файлов
    private const string mergedFile = "Merged.txt"; // итоговый файл после объединения

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Сгенерировать файлы");
            Console.WriteLine("2. Объединить файлы + фильтр");
            Console.WriteLine("3. Импортировать в БД");
            Console.WriteLine("4. SQL-скрипты (сумма / медиана)");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Генерация файлов...");
                    FileGenerator.GenerateFiles(dataFolder, 100, 100_000);
                    break;

                case "2":
                    Console.Write("Введите фильтр (например 'abc', пусто = без фильтра): ");
                    string? filter = Console.ReadLine();
                    FileMerger.MergeFiles(dataFolder, mergedFile, filter ?? "");
                    break;

                case "3":
                    Console.WriteLine("Импорт данных в БД...");
                    DatabaseImporter.ImportToSqlServer(connectionString, tableName, mergedFile, Console.WriteLine);
                    break;

                case "4":
                    Console.WriteLine("\nВыберите SQL-запрос:");
                    Console.WriteLine("1. Сумма целых чисел");
                    Console.WriteLine("2. Медиана дробных чисел");
                    Console.Write("Ваш выбор: ");
                    string? sqlChoice = Console.ReadLine();

                    string scriptDir = Path.Combine(AppContext.BaseDirectory, "SqlScripts");

                    if (sqlChoice == "1")
                    {
                        string scriptPath = Path.Combine(scriptDir, "SumIntegers.sql");
                        SqlRunner.ExecuteSqlScript(connectionString, scriptPath, Console.WriteLine);
                    }
                    else if (sqlChoice == "2")
                    {
                        string scriptPath = Path.Combine(scriptDir, "MedianFloats.sql");
                        SqlRunner.ExecuteSqlScript(connectionString, scriptPath, Console.WriteLine);
                    }
                    break;


                case "0":
                    return;

                default:
                    Console.WriteLine("Неверный выбор, попробуйте снова.");
                    break;
            }
        }
    }
}
