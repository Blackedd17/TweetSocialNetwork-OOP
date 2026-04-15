using Social.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Social.Core.Entities
{
    public abstract class Post : BaseEntity, ILikeable, ICommentable, IShareable
    {
        public Guid AuthorId { get; set; }
        public string Content { get; set; }
        // Like хийсэн хэрэглэгчдийн Id-г хадгална (давхар like хийхээс хамгаална)
        private readonly HashSet<Guid> _likedUserIds = new HashSet<Guid>();
        public int LikeCount => _likedUserIds.Count;
        public List<Comment> Comments { get; } = new List<Comment>();
        public Post(Guid authorId, string content)
        {
            AuthorId = authorId;
            Content = content;
        }

        public void Like(Guid userId)
        {
            _likedUserIds.Add(userId); // HashSet учраас давхар орохгүй
        }

        public void AddComment(Guid userId, string text)
        {
            Comments.Add(new Comment(this.Id, userId, text));
        }

        public void Share(Guid userId)
        {
            
        }
    }
}
