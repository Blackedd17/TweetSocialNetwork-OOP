using Microsoft.Data.Sqlite;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Social.Infrastructure.Repositories
{
    /// <summary>
    /// Comment (сэтгэгдэл)-ийг SQLite өгөгдлийн санд хадгалах repository.
    ///
    /// Энэ class нь:
    /// - Comment нэмэх
    /// - Тухайн post-ийн comment-уудыг авах
    ///
    /// Database-аас comment унших үед Id утгыг сэргээж өгнө.
    /// </summary>
    public class SQLiteCommentRepository : ICommentRepository
    {
        /// <summary>
        /// SQLite өгөгдлийн сангийн context.
        /// </summary>
        private readonly SqliteDbContext context;

        /// <summary>
        /// Repository-г context-тэй нь үүсгэнэ.
        /// </summary>
        /// <param name="context">SQLite DbContext</param>
        public SQLiteCommentRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Шинэ comment нэмнэ.
        /// </summary>
        /// <param name="comment">Нэмэх comment</param>
        public void Add(Comment comment)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Comments (Id, PostId, UserId, Text)
                VALUES ($id, $postId, $userId, $text)";

                cmd.Parameters.AddWithValue("$id", comment.Id.ToString());
                cmd.Parameters.AddWithValue("$postId", comment.PostId.ToString());
                cmd.Parameters.AddWithValue("$userId", comment.UserId.ToString());
                cmd.Parameters.AddWithValue("$text", comment.Text);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Тухайн post-ийн бүх comment-уудыг буцаана.
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Comment жагсаалт</returns>
        public List<Comment> GetByPostId(Guid postId)
        {
            var list = new List<Comment>();

            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Comments WHERE PostId = $postId";
                cmd.Parameters.AddWithValue("$postId", postId.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var comment = new Comment(
                            Guid.Parse(reader["PostId"].ToString()),
                            Guid.Parse(reader["UserId"].ToString()),
                            reader["Text"].ToString()
                        );

                        comment.Id = Guid.Parse(reader["Id"].ToString());

                        list.Add(comment);
                    }
                }
            }

            return list;
        }
    }
}