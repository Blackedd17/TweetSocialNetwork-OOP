using Social.Core.Entities;
using System;
using System.Collections.Generic;

namespace Social.Core.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        List<Post> GetAll();
        Post GetById(Guid id);
        void Delete(Guid id);
    }
}
