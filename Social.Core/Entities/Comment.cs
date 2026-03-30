using System;

namespace Social.Core.Entities
{
    namespace Social.Core.Entities
    {
        public class Comment : BaseEntity
        {
            public Guid UserId { get; set; }
            public string Text { get; set; }
            public Comment(Guid userId, string text)
            {
                UserId = userId;
                Text = text;
            }
        }
    }
}
