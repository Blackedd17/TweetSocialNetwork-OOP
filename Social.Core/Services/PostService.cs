using System;
using System.Collections.Generic;
using System.Linq;
using Social.Core.Entities;
using Social.Core.Repositories;

namespace Social.Core.Services
{
    public class PostService
    {
        private readonly IRepository<Post> postRepository;
        public PostService(IRepository<Post> postRepository)
        {
            this.postRepository = postRepository;
        }
        public void CreatePost(Post post)
        {
            postRepository.Add(post);
        }
        public List<Post> GetAllPosts()
        {
            return postRepository.GetAll();
        }
        public void DeletePost(Post post)
        {
            postRepository.Remove(post);
        }
        public List<Post> GetPostsByAuthor(Guid authorId)
        {
            return postRepository.GetAll()
                .Where(t => t.AuthorId == authorId)
                .ToList();
        }
        public bool IsFollowing(User follower, User target)
        {
            if (follower == null || target == null) return false;
            return follower.Following.Contains(target.Id);
        }
    }
}
