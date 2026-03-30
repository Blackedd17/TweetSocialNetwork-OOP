using System;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Програмын эхлэх цэг (Entry Point).
    /// Энэ class нь WinForms application-ийг эхлүүлнэ.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method нь програм ажиллаж эхлэх үндсэн цэг юм.
        /// Эндээс Form1 нээгдэж social platform UI ажиллана.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Үндсэн form-ийг ажиллуулж байна
            Application.Run(new Form1());
        }
    }
}