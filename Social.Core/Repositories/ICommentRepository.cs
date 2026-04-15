using Social.Core.Entities;
using System;
using System.Collections.Generic;

namespace Social.Core.Repositories
{
    public interface ICommentRepository
    {
        void Add(Comment comment);
        List<Comment> GetByPostId(Guid postId);
    }
}
