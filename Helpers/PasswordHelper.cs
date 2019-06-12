using System;
using System.Security;

namespace FunctionalAreaCoverage.Helpers
{
    internal static class PasswordHelper
    {
        public static void WriteLine(string value = "")
        {
            Console.WriteLine(value);
        }

        public static void WriteLine(string format, object[] arg)
        {
            Console.WriteLine(format, arg);
        }

        public static string ReadUsername(string caption = "Username: ")
        {
            Console.Write(caption);
            return Console.ReadLine();
        }

        public static SecureString ReadPassword(string caption = "Password: ")
        {
            Console.Write(caption);

            var password = new SecureString();

            var nextKey = Console.ReadKey(true);

            while (nextKey.Key != ConsoleKey.Enter)
            {
                if (nextKey.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);
                        Console.Write(nextKey.KeyChar);
                        Console.Write(" ");
                        Console.Write(nextKey.KeyChar);
                    }
                }
                else
                {
                    password.AppendChar(nextKey.KeyChar);
                    Console.Write("*");
                }
                nextKey = Console.ReadKey(true);
            }
            password.MakeReadOnly();

            Console.WriteLine();

            return password;
        }
    }
}
