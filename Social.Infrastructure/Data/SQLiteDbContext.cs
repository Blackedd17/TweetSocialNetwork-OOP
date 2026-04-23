using Microsoft.Data.Sqlite;

namespace Social.Infrastructure.Data
{
    /// <summary>
    /// SQLite өгөгдлийн сантай холбогдох болон database schema-г удирдах class.
    /// 
    /// Энэ class нь:
    /// - SQLite connection үүсгэх
    /// - Шаардлагатай table-уудыг үүсгэх
    /// - Database-ийг бүрэн reset хийх
    /// 
    /// social.db нэртэй файл дээр өгөгдлийг хадгална.
    /// </summary>
    public class SqliteDbContext
    {
        /// <summary>
        /// SQLite өгөгдлийн сантай холбогдох connection string.
        /// </summary>
        private readonly string connectionString = "Data Source=social.db";

        /// <summary>
        /// Шинэ SQLite connection үүсгэж буцаана.
        /// </summary>
        /// <returns>SqliteConnection объект</returns>
        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        /// <summary>
        /// Database-д шаардлагатай table-уудыг үүсгэнэ.
        /// Хэрэв table аль хэдийн байвал дахин үүсгэхгүй.
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

        /// <summary>
        /// Database дахь бүх table-уудыг устгаад дахин шинээр үүсгэнэ.
        /// 
        /// Энэ method нь:
        /// - Туршилтын үеийн хуучин өгөгдлийг цэвэрлэх
        /// - Schema өөрчлөгдсөний дараа database шинэчлэх
        /// үед хэрэглэгдэнэ.
        /// </summary>
        public void ResetDatabase()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                DROP TABLE IF EXISTS Reactions;
                DROP TABLE IF EXISTS Comments;
                DROP TABLE IF EXISTS Posts;
                DROP TABLE IF EXISTS Users;
                ";

                cmd.ExecuteNonQuery();
            }

            Initialize();
        }
    }
}