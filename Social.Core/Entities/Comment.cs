using System;

namespace Social.Core.Entities
{
        /// <summary>
        /// Пост дээр бичсэн сэтгэгдлийг илэрхийлэх class
        /// </summary>
        public class Comment
        {
            public Guid Id { get; set; }
            public Guid PostId { get; set; }
            public Guid UserId { get; set; }
            public string Text { get; set; }

            public Comment(Guid postId, Guid userId, string text)
            {
                Id = Guid.NewGuid();
                PostId = postId;
                UserId = userId;
                Text = text;
            }
        }
}
