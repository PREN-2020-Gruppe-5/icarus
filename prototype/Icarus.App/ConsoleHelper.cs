using System;
using System.Collections.Generic;
using System.Linq;

namespace Icarus.App
{
    public class ConsoleHelper
    {
        private static readonly Dictionary<string, int> Rows = new Dictionary<string, int>();

        public static void WriteDouble(string title, double value)
        {
            WriteString(title, $"{value:F}");
        }

        public static void WriteString(string title, string value)
        {
            lock (Rows)
            {
                Console.CursorVisible = false;

                if (Rows.ContainsKey(title))
                {
                    var currentTop = Console.CursorTop;
                    Console.SetCursorPosition(title.Length + 2, Rows[title]);
                    Console.Write(string.Join(string.Empty, Enumerable.Repeat(" ", Console.WindowWidth - (title.Length + 2))));
                    Console.SetCursorPosition(title.Length + 2, Rows[title]);
                    Console.Write(value);
                    Console.CursorTop = currentTop;
                    Console.CursorLeft = 0;
                }
                else
                {
                    Rows.Add(title, Console.CursorTop);
                    Console.WriteLine($"{title}: {value}");
                }
            }
        }
    }
}