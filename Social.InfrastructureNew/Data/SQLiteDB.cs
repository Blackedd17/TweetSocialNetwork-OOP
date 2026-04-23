using Microsoft.Data.Sqlite;

namespace Social.Infrastructure.Data
{
    /// <summary>
    /// SQLite өгөгдлийн сантай холбогдох болон database үүсгэх class.
    /// 
    /// Энэ class нь:
    /// - SQLite connection үүсгэх (GetConnection)
    /// - Шаардлагатай table-уудыг үүсгэх (Initialize)
    /// 
    /// social.db нэртэй файл дээр өгөгдлийг хадгална.
    /// </summary>
    public class SqliteDbContext
    {
        /// <summary>
        /// SQLite өгөгдлийн сантай холбогдох connection string.
        /// </summary>
        private string connectionString = "Data Source=social.db";

        /// <summary>
        /// Шинэ SQLite connection үүсгэж буцаана.
        /// </summary>
        /// <returns>SqliteConnection объект</returns>
        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        /// <summary>
        /// Database-ийг анх удаа үүсгэх болон шаардлагатай table-уудыг бий болгоно.
        /// 
        /// Үүнд:
        /// - Users (хэрэглэгчид)
        /// - Posts (нийтлэлүүд)
        /// - Likes (таалагдсан мэдээлэл)
        /// - Comments (сэтгэгдлүүд)
        /// 
        /// Хэрвээ table аль хэдийн байвал дахин үүсгэхгүй.
        /// </summary>
        public void Initialize()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();

                cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id TEXT PRIMARY KEY,
                Username TEXT UNIQUE,
                DisplayName TEXT,
                Age INTEGER,
                PasswordHash TEXT
            );

            CREATE TABLE IF NOT EXISTS Posts (
                Id TEXT PRIMARY KEY,
                AuthorId TEXT,
                Content TEXT,
                CreatedAt TEXT
            );

            CREATE TABLE IF NOT EXISTS Reactions (
                Id TEXT PRIMARY KEY,
                PostId TEXT,
                UserId TEXT,
                Type TEXT
            );

            CREATE TABLE IF NOT EXISTS Comments (
                Id TEXT PRIMARY KEY,
                PostId TEXT,
                UserId TEXT,
                Text TEXT
            );
            ";

                cmd.ExecuteNonQuery();
            }
        }
    }
}