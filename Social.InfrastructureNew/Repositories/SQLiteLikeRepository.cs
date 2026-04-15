using Microsoft.Data.Sqlite;
using Social.Core.Repositories;
using Social.Infrastructure.Data;
using System;

/// <summary>
/// Like (таалагдсан) мэдээллийг SQLite өгөгдлийн санд удирдах repository.
/// 
/// Энэ class нь:
/// - Постод like нэмэх (AddLike)
/// - Like устгах (RemoveLike)
/// - Постын нийт like тоог авах (GetLikeCount)
/// - Хэрэглэгч тухайн постод like дарсан эсэхийг шалгах (HasLiked)
/// үйлдлүүдийг гүйцэтгэнэ.
/// 
/// Likes table дээр SQL query ашиглан like мэдээллийг хадгалж,
/// COUNT функцээр like-ийн тоо болон төлөвийг тодорхойлно.
/// </summary>

namespace Social.Infrastructure.Repositories
{
    public class SQLiteLikeRepository : ILikeRepository
    {
        private readonly SqliteDbContext context;

        public SQLiteLikeRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        public void AddLike(Guid postId, Guid userId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Likes (Id, PostId, UserId)
                VALUES ($id, $postId, $userId)";

                cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("$postId", postId.ToString());
                cmd.Parameters.AddWithValue("$userId", userId.ToString());

                cmd.ExecuteNonQuery();
            }
        }

        public void RemoveLike(Guid postId, Guid userId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                DELETE FROM Likes 
                WHERE PostId = $postId AND UserId = $userId";

                cmd.Parameters.AddWithValue("$postId", postId.ToString());
                cmd.Parameters.AddWithValue("$userId", userId.ToString());

                cmd.ExecuteNonQuery();
            }
        }

        public int GetLikeCount(Guid postId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM Likes WHERE PostId = $postId";
                cmd.Parameters.AddWithValue("$postId", postId.ToString());

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool HasLiked(Guid postId, Guid userId)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                SELECT COUNT(*) FROM Likes 
                WHERE PostId = $postId AND UserId = $userId";

                cmd.Parameters.AddWithValue("$postId", postId.ToString());
                cmd.Parameters.AddWithValue("$userId", userId.ToString());

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}