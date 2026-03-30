using System;

namespace Social.Core.Entities
{
    public class TextPost : Post
    {
        public TextPost(Guid authorId, string content) : base(authorId, content)
        {
        }
    }
}
