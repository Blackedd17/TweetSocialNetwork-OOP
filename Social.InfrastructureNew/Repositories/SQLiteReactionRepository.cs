using Social.Core.Repositories;
using Social.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using System;

namespace Social.Infrastructure.Repositories
{
    public class SQLiteReactionRepository : IReactionRepository
    {
        private readonly SqliteDbContext context;

        public SQLiteReactionRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        public void React(Guid postId, Guid userId, string type)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                // өмнөх reaction устгана
                var delete = conn.CreateCommand();
                delete.CommandText = "DELETE FROM Reactions WHERE PostId=$p AND UserId=$u";
                delete.Parameters.AddWithValue("$p", postId.ToString());
                delete.Parameters.AddWithValue("$u", userId.ToString());
                delete.ExecuteNonQuery();

                // шинэ reaction нэмнэ
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Reactions (Id, PostId, UserId, Type)
                VALUES ($id, $p, $u, $t)";

                cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("$p", postId.ToString());
                cmd.Parameters.AddWithValue("$u", userId.ToString());
                cmd.Parameters.AddWithValue("$t", type);

                cmd.ExecuteNonQuery();
            }
        }

        public int GetCount(Guid postId, string type)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT COUNT(*) FROM Reactions 
            WHERE PostId=$p AND Type=$t";

                cmd.Parameters.AddWithValue("$p", postId.ToString());
                cmd.Parameters.AddWithValue("$t", type);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public string GetUserReaction(Guid postId, Guid userId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT Type FROM Reactions 
            WHERE PostId=$p AND UserId=$u";

                cmd.Parameters.AddWithValue("$p", postId.ToString());
                cmd.Parameters.AddWithValue("$u", userId.ToString());

                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
}