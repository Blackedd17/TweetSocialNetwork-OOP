using System;
using System.Collections.Generic;
using System.Linq;
using Social.Core.Entities;
using Social.Core.Repositories;

namespace Social.Core.Services
{
    /// <summary>
    /// Post (нийтлэл)-той холбоотой бизнес логикийг удирдах service class.
    /// 
    /// Энэ class нь:
    /// - Пост үүсгэх (CreatePost)
    /// - Бүх постуудыг авах (GetAllPosts)
    /// - Пост устгах (DeletePost)
    /// - ID-р пост авах (GetById)
    /// - Хэрэглэгчийн постуудыг авах (GetPostsByAuthor)
    /// - Хэрэглэгч дагаж байгаа эсэхийг шалгах (IsFollowing)
    /// 
    /// Repository ашиглан өгөгдөлтэй харьцана.
    /// </summary>
    public class PostService
    {
        private readonly IPostRepository postRepository;

        /// <summary>
        /// PostService-ийг repository-тэй холбож үүсгэнэ.
        /// </summary>
        /// <param name="postRepository">Post хадгалах repository</param>
        public PostService(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        /// <summary>
        /// Шинэ пост үүсгэн хадгална.
        /// </summary>
        /// <param name="post">Үүсгэх пост</param>
        public void CreatePost(Post post)
        {
            postRepository.Add(post);
        }

        /// <summary>
        /// Бүх постуудыг буцаана.
        /// </summary>
        /// <returns>Post жагсаалт</returns>
        public List<Post> GetAllPosts()
        {
            return postRepository.GetAll();
        }

        /// <summary>
        /// Пост устгана.
        /// </summary>
        /// <param name="post">Устгах пост</param>
        public void DeletePost(Post post)
        {
            postRepository.Delete(post.Id);
        }

        /// <summary>
        /// ID-р пост хайж олно.
        /// </summary>
        /// <param name="id">Post ID</param>
        /// <returns>Олдвол Post, эс бөгөөс null</returns>
        public Post GetById(Guid id)
        {
            return postRepository.GetById(id);
        }

        /// <summary>
        /// Тухайн хэрэглэгчийн бүх постуудыг буцаана.
        /// </summary>
        /// <param name="authorId">Хэрэглэгчийн ID</param>
        /// <returns>Post жагсаалт</returns>
        public List<Post> GetPostsByAuthor(Guid authorId)
        {
            return postRepository.GetAll()
                .Where(p => p.AuthorId == authorId)
                .ToList();
        }

        /// <summary>
        /// Нэг хэрэглэгч нөгөөг дагаж байгаа эсэхийг шалгана.
        /// </summary>
        /// <param name="follower">Дагаж буй хэрэглэгч</param>
        /// <param name="target">Дагагдаж буй хэрэглэгч</param>
        /// <returns>Дагаж байвал true, үгүй бол false</returns>
        public bool IsFollowing(User follower, User target)
        {
            if (follower == null || target == null) return false;
            return follower.Following.Contains(target.Id);
        }
    }
}