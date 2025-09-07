using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTask_ConsoleApp.Utils
{
    public static class RandomDataGenerator
    {
        private static readonly Random random = new Random();
        private const string LatinChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string CyrillicChars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        public static DateTime RandomDateLast5Years() // случайная дата за 5 лет
        {
            int daysBack = random.Next(0, 365 * 5 + 1);

            return DateTime.Today.AddDays(-daysBack);
        }

        public static string RandomLatinString(int length = 10) => RandomString(length, LatinChars);
        public static string RandomCyrillicString(int length = 10) => RandomString(length, CyrillicChars);

        private static string RandomString(int length, string chars) // случайная строка
        {
            var sb = new StringBuilder(length);

            int n = chars.Length;

            for(int i = 0; i < length; i++) 
                sb.Append(chars[random.Next(n)]);

            return sb.ToString();
        }

        public static int RandomEvenInt() // случайное четное целое число от 2 до 100М
        {
            int half = random.Next(1, 50_000_001);

            return half * 2;
        }

        public static double RandomDouble() // случайное десятичное число от 1 до 20
        {
            double d = random.NextDouble() * 19.0 + 1.0;

            return Math.Round(d, 8);
        }
        public static string DoubleToStringInvariant(double d)
            => d.ToString("F8", CultureInfo.InvariantCulture);
    }
}
