using Microsoft.Data.Sqlite;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Social.Infrastructure.Repositories
{
    /// <summary>
    /// Post (нийтлэл)-ыг SQLite өгөгдлийн санд хадгалах repository.
    ///
    /// Энэ class нь:
    /// - Post нэмэх
    /// - Бүх post-уудыг авах
    /// - ID-р post авах
    /// - Post устгах
    ///
    /// Database-аас post унших үед Id болон CreatedAt утгуудыг
    /// яг хэвээр нь entity дээр сэргээж өгдөг.
    /// Ингэснээр comment, reaction зэрэг холбоотой өгөгдлүүд
    /// зөв post дээрээ харагдана.
    /// </summary>
    public class SQLitePostRepository : IPostRepository
    {
        /// <summary>
        /// SQLite өгөгдлийн сангийн context.
        /// </summary>
        private readonly SqliteDbContext context;

        /// <summary>
        /// Repository-г context-тэй нь үүсгэнэ.
        /// </summary>
        /// <param name="context">SQLite DbContext</param>
        public SQLitePostRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Шинэ post-ыг database-д хадгална.
        /// </summary>
        /// <param name="post">Хадгалах post</param>
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
                cmd.Parameters.AddWithValue("$createdAt", post.CreatedAt.ToString("o"));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// ID-р post устгана.
        /// </summary>
        /// <param name="id">Устгах post-ийн ID</param>
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

        /// <summary>
        /// Database дахь бүх post-уудыг буцаана.
        /// </summary>
        /// <returns>Post жагсаалт</returns>
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

                        /// <summary>
                        /// Database-аас уншсан анхны Id-г сэргээнэ.
                        /// </summary>
                        post.Id = Guid.Parse(reader["Id"].ToString());

                        /// <summary>
                        /// Database-аас уншсан CreatedAt-г сэргээнэ.
                        /// </summary>
                        post.CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString());

                        list.Add(post);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// ID-р нэг post олж буцаана.
        /// </summary>
        /// <param name="id">Хайх post-ийн ID</param>
        /// <returns>Олдвол post, үгүй бол null</returns>
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
                        var post = new TextPost(
                            Guid.Parse(reader["AuthorId"].ToString()),
                            reader["Content"].ToString()
                        );

                        /// <summary>
                        /// Database-аас уншсан анхны Id-г сэргээнэ.
                        /// </summary>
                        post.Id = Guid.Parse(reader["Id"].ToString());

                        /// <summary>
                        /// Database-аас уншсан CreatedAt-г сэргээнэ.
                        /// </summary>
                        post.CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString());

                        return post;
                    }
                }
            }

            return null;
        }
    }
}