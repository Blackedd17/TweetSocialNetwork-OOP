using Microsoft.Data.Sqlite;
using Social.Core.Repositories;
using Social.Infrastructure.Data;
using System;

namespace Social.Infrastructure.Repositories
{
    /// <summary>
    /// Reaction (Like, Love, Haha, Sad, Angry, Care)-ийг
    /// SQLite өгөгдлийн санд хадгалах repository.
    /// 
    /// Энэ class нь:
    /// - Reaction нэмэх
    /// - Reaction солих
    /// - Reaction устгах
    /// - Reaction count авах
    /// - Хэрэглэгчийн тухайн post дээр өгсөн reaction-г унших
    /// - Toggle reaction хийх
    /// үйлдлүүдийг гүйцэтгэнэ.
    /// </summary>
    public class SQLiteReactionRepository : IReactionRepository
    {
        /// <summary>
        /// SQLite өгөгдлийн сангийн context.
        /// </summary>
        private readonly SqliteDbContext context;

        /// <summary>
        /// Repository-г context-тэй нь үүсгэнэ.
        /// </summary>
        /// <param name="context">SQLite DbContext</param>
        public SQLiteReactionRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Тухайн post дээр хэрэглэгчийн reaction-ийг нэмэх эсвэл шинэчилнэ.
        /// Хуучин reaction байвал устгаад шинэ reaction оруулна.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="type">Reaction төрөл</param>
        public void React(Guid postId, Guid userId, string type)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    var delete = conn.CreateCommand();
                    delete.CommandText = "DELETE FROM Reactions WHERE PostId = $p AND UserId = $u";
                    delete.Parameters.AddWithValue("$p", postId.ToString());
                    delete.Parameters.AddWithValue("$u", userId.ToString());
                    delete.ExecuteNonQuery();

                    var insert = conn.CreateCommand();
                    insert.CommandText = @"
                    INSERT INTO Reactions (Id, PostId, UserId, Type)
                    VALUES ($id, $p, $u, $t)";
                    insert.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
                    insert.Parameters.AddWithValue("$p", postId.ToString());
                    insert.Parameters.AddWithValue("$u", userId.ToString());
                    insert.Parameters.AddWithValue("$t", type);
                    insert.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Тухайн хэрэглэгчийн тухайн post дээр өгсөн reaction-ийг устгана.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        public void RemoveReaction(Guid postId, Guid userId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Reactions WHERE PostId = $p AND UserId = $u";
                cmd.Parameters.AddWithValue("$p", postId.ToString());
                cmd.Parameters.AddWithValue("$u", userId.ToString());

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Тухайн post дээрх тодорхой reaction төрлийн нийт тоог буцаана.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="type">Reaction төрөл</param>
        /// <returns>Reaction count</returns>
        public int GetCount(Guid postId, string type)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                SELECT COUNT(*) 
                FROM Reactions 
                WHERE PostId = $p AND Type = $t";
                cmd.Parameters.AddWithValue("$p", postId.ToString());
                cmd.Parameters.AddWithValue("$t", type);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Тухайн хэрэглэгчийн тухайн post дээр өгсөн reaction төрлийг буцаана.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>Reaction төрөл, байхгүй бол null</returns>
        public string GetUserReaction(Guid postId, Guid userId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                SELECT Type 
                FROM Reactions 
                WHERE PostId = $p AND UserId = $u";
                cmd.Parameters.AddWithValue("$p", postId.ToString());
                cmd.Parameters.AddWithValue("$u", userId.ToString());

                var result = cmd.ExecuteScalar();
                return result == null ? null : result.ToString();
            }
        }

        /// <summary>
        /// Toggle reaction хийнэ.
        /// 
        /// Хэрэв хэрэглэгч ижил reaction дахин дарвал:
        /// - reaction устгана
        /// 
        /// Хэрэв өөр reaction дарвал:
        /// - шинэ reaction болгон солино
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="type">Reaction төрөл</param>
        public void ToggleReaction(Guid postId, Guid userId, string type)
        {
            string currentReaction = GetUserReaction(postId, userId);

            if (!string.IsNullOrWhiteSpace(currentReaction) &&
                string.Equals(currentReaction, type, StringComparison.OrdinalIgnoreCase))
            {
                RemoveReaction(postId, userId);
                return;
            }

            React(postId, userId, type);
        }
    }
}