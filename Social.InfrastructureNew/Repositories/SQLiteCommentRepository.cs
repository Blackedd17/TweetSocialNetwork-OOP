using Microsoft.Data.Sqlite;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Infrastructure.Data;
using System;
using System.Collections.Generic;

/// <summary>
/// Comment (сэтгэгдэл)-ийг SQLite өгөгдлийн санд хадгалах repository.
/// 
/// Энэ class нь:
/// - Comment нэмэх (Add)
/// - Тухайн постын comment-уудыг авах (GetByPostId)
/// үйлдлүүдийг гүйцэтгэнэ.
/// 
/// Comments table-оос PostId ашиглан comment-уудыг шүүж,
/// Comment entity болгон хөрвүүлж буцаана.
/// </summary>

namespace Social.Infrastructure.Repositories
{
    public class SQLiteCommentRepository : ICommentRepository
    {
        private readonly SqliteDbContext context;

        public SQLiteCommentRepository(SqliteDbContext context)
        {
            this.context = context;
        }

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

                        list.Add(comment);
                    }
                }
            }

            return list;
        }
    }
}