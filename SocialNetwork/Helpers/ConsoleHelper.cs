using System;

namespace TweetingPlatform.Helpers
{
    /// <summary>
    /// Console дээр ажиллах нэмэлт туслах (helper) функцуудыг агуулсан class.
    /// 
    /// Энэ class нь:
    /// - Хэрэглэгчээс нууц үгийг аюулгүйгээр (масклан) унших боломж олгоно
    /// 
    /// Password оруулах үед тэмдэгтүүдийг '*' болгон харуулж,
    /// жинхэнэ утгыг нууцалдаг.
    /// </summary>
    public class ConsoleHelper
    {
        /// <summary>
        /// Console-оос нууц үгийг масклан (*** хэлбэрээр) уншина.
        /// 
        /// Хэрэглэгч:
        /// - Тэмдэгт оруулах үед '*' харагдана
        /// - Backspace дарж засвар хийж болно
        /// - Enter дарж оруулалтыг дуусгана
        /// </summary>
        /// <returns>Оруулсан нууц үг (string)</returns>
        public static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            return password;
        }
    }
}