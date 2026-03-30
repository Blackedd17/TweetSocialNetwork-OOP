using System;

namespace Social.Core.Entities
{
    /// <summary>
    /// Story entity class.
    /// Social platform дээрх 24 цагийн story өгөгдлийг төлөөлнө.
    /// </summary>
    public class Story
    {
        /// <summary>
        /// Story-ийн дахин давтагдашгүй ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Story-г үүсгэсэн хэрэглэгчийн ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Story эзэмшигчийн username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Story-ийн текстэн агуулга.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Story-г үзсэн эсэх төлөв.
        /// true бол үзсэн, false бол үзээгүй.
        /// </summary>
        public bool IsViewed { get; set; }

        /// <summary>
        /// Story үүссэн огноо, цаг.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Story объект үүсгэх constructor.
        /// </summary>
        /// <param name="userId">Story эзэмшигчийн ID</param>
        /// <param name="username">Story эзэмшигчийн username</param>
        /// <param name="content">Story агуулга</param>
        public Story(Guid userId, string username, string content)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Username = username;
            Content = content;
            IsViewed = false;
            CreatedAt = DateTime.Now;
        }
    }
}