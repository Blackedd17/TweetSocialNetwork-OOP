using Microsoft.Data.Sqlite;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;
using Social.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Social.Infrastructure.Repositories
{
    /// <summary>
    /// Post (нийтлэл)-ыг SQLite өгөгдлийн санд хадгалах repository.
    /// 
    /// Энэ class нь:
    /// - Post нэмэх (Add)
    /// - Бүх постуудыг авах (GetAll)
    /// - Нэг постыг ID-р авах (GetById)
    /// - Пост устгах (Delete)
    /// үйлдлүүдийг гүйцэтгэнэ.
    /// 
    /// SQLite database-тай холбогдож SQL query ашиглан
    /// Post entity-г хадгалах, унших үүрэгтэй.
    /// </summary>
    public class SQLitePostRepository : IPostRepository
    {
        private readonly SqliteDbContext context;

        public SQLitePostRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        public void Add(Post post)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Posts (Id, AuthorId, Content, CreatedAt)
                VALUES ($id, $authorId, $content, $createdAt)";

                cmd.Parameters.AddWithValue("$id", post.Id.ToString());
                cmd.Parameters.AddWithValue("$authorId", post.AuthorId.ToString());
                cmd.Parameters.AddWithValue("$content", post.Content);
                cmd.Parameters.AddWithValue("$createdAt", post.CreatedAt.ToString());

                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(Guid id)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Posts WHERE Id = $id";

                cmd.Parameters.AddWithValue("$id", id.ToString());

                cmd.ExecuteNonQuery();
            }
        }

        public List<Post> GetAll()
        {
            var list = new List<Post>();

            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Posts";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var post = new TextPost(
                            Guid.Parse(reader["AuthorId"].ToString()),
                            reader["Content"].ToString()
                        );

                        list.Add(post);
                    }
                }
            }

            return list;
        }

        public Post GetById(Guid id)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Posts WHERE Id = $id";
                cmd.Parameters.AddWithValue("$id", id.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TextPost(
                            Guid.Parse(reader["AuthorId"].ToString()),
                            reader["Content"].ToString()
                        );
                    }
                }
            }

            return null;
        }
    }
}