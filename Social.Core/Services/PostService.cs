using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<Post> GetPostsByAuthor(Guid authorId)
        {
            return postRepository.GetAll()
                .Where(t => t.AuthorId == authorId)
                .ToList();
        }
    }
}
