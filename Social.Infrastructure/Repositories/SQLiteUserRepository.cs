using Microsoft.Data.Sqlite;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace Social.Infrastructure.Repositories
{
    public class SQLiteUserRepository : IRepository<User>
    {
        private readonly SqliteDbContext context;

        public SQLiteUserRepository(SqliteDbContext context)
        {
            this.context = context;
        }

        public void Add(User user)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Users (Id, Username, DisplayName, Age, PasswordHash)
                VALUES ($id, $username, $displayName, $age, $password)";

                cmd.Parameters.AddWithValue("$id", user.Id.ToString());
                cmd.Parameters.AddWithValue("$username", user.Username);
                cmd.Parameters.AddWithValue("$displayName", user.DisplayName);
                cmd.Parameters.AddWithValue("$age", user.Age);
                cmd.Parameters.AddWithValue("$password", user.PasswordHash);

                cmd.ExecuteNonQuery();
            }
        }

        public List<User> GetAll()
        {
            var list = new List<User>();

            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User(
                            reader["Username"].ToString(),
                            reader["DisplayName"].ToString(),
                            Convert.ToByte(reader["Age"]),
                            reader["PasswordHash"].ToString()
                        );

                        user.Id = Guid.Parse(reader["Id"].ToString());
                        list.Add(user);
                    }
                }
            }

            return list;
        }

        public User GetById(Guid id)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Users WHERE Id=$id";
                cmd.Parameters.AddWithValue("$id", id.ToString());

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new User(
                            reader["Username"].ToString(),
                            reader["DisplayName"].ToString(),
                            Convert.ToByte(reader["Age"]),
                            reader["PasswordHash"].ToString()
                        );

                        user.Id = Guid.Parse(reader["Id"].ToString());
                        return user;
                    }
                }
            }

            return null;
        }

        public void Delete(Guid id)
        {
            using (var conn = context.GetConnection())
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Users WHERE Id=$id";
                cmd.Parameters.AddWithValue("$id", id.ToString());

                cmd.ExecuteNonQuery();
            }
        }
    }
}