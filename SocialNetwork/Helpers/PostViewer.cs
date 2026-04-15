using Social.Core.Entities;
using Social.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetingPlatform.Helpers
{
    /// <summary>
    /// Пост (Tweet)-уудыг консолоор харуулах helper класс.
    ///
    /// Энэ класс нь:
    /// - Бүх постуудыг жагсаах (ShowPosts)
    /// - Хэрэглэгчийн feed харуулах (ShowFeed)
    /// - Comment-уудыг харуулах (ShowComments)
    /// - Пост дээр interaction хийх (InteractWithPost)
    ///
    /// Feed нь:
    /// - Follow хийсэн хэрэглэгчдийн постыг эхэнд харуулна
    /// - Шинэ постуудыг дээгүүр эрэмбэлнэ
    /// </summary>
    public class PostViewer
    {
        /// <summary>
        /// Бүх постуудыг жагсааж харуулна.
        /// </summary>
        /// <param name="posts">Post жагсаалт</param>
        /// <param name="userService">User service</param>
        public static void ShowPosts(List<Post> posts, UserService userService)
        {
            Console.WriteLine("\n==== TWEETS ====");

            if (posts.Count == 0)
            {
                Console.WriteLine("No tweets yet.");
                return;
            }

            for (int i = 0; i < posts.Count; i++)
            {
                var author = userService.GetById(posts[i].AuthorId);

                // Хэрвээ user олдохгүй бол "Unknown" гэж харуулна
                string authorName = author != null ? author.DisplayName : "Unknown";

                Console.WriteLine(
                    $"{i + 1}. [{authorName}] {posts[i].Content} | Likes:{posts[i].LikeCount} | Comments:{posts[i].Comments.Count}"
                );
            }
        }

        /// <summary>
        /// Хэрэглэгчийн feed-г харуулна.
        /// 
        /// Feed нь:
        /// - Follow хийсэн хүмүүсийн постыг эхэнд
        /// - Дараа нь шинэ постуудыг эрэмбэлж харуулна
        /// </summary>
        /// <param name="currentUser">Одоогийн хэрэглэгч</param>
        /// <param name="userService">User service</param>
        /// <param name="postService">Post service</param>
        public static void ShowFeed(User currentUser, UserService userService, PostService postService)
        {
            Console.WriteLine("\n==== YOUR FEED ====");

            var allPosts = postService.GetAllPosts();

            // Feed sorting:
            // 1. Follow хийсэн хүмүүсийн пост
            // 2. Шинэ пост эхэнд
            var feedPosts = allPosts
                .OrderByDescending(p => currentUser.Following.Contains(p.AuthorId))
                .ThenByDescending(p => p.CreatedAt)
                .ToList();

            if (feedPosts.Count == 0)
            {
                Console.WriteLine("Your feed is empty.");
                return;
            }

            for (int i = 0; i < feedPosts.Count; i++)
            {
                var author = userService.GetById(feedPosts[i].AuthorId);
                string authorName = author != null ? author.DisplayName : "Unknown";

                Console.WriteLine(
                    $"{i + 1}. [{authorName}] {feedPosts[i].Content} | Likes:{feedPosts[i].LikeCount} | Comments:{feedPosts[i].Comments.Count}"
                );
            }
        }

        /// <summary>
        /// Тухайн постын comment-уудыг харуулна.
        /// </summary>
        /// <param name="post">Сонгосон пост</param>
        /// <param name="userService">User service</param>
        public static void ShowComments(Post post, UserService userService)
        {
            Console.WriteLine("\n---- COMMENTS ----");

            if (post.Comments.Count == 0)
            {
                Console.WriteLine("No comments yet.");
                return;
            }

            for (int i = 0; i < post.Comments.Count; i++)
            {
                var commentUser = userService.GetById(post.Comments[i].UserId);

                string commenterName = commentUser != null
                    ? commentUser.DisplayName
                    : "Unknown";

                Console.WriteLine(
                    $"{i + 1}. [{commenterName}] {post.Comments[i].Text}"
                );
            }
        }

        /// <summary>
        /// Нэг пост дээр дараах үйлдлүүдийг хийх боломж олгоно:
        /// - Like хийх
        /// - Comment бичих
        /// - Comment харах
        /// </summary>
        /// <param name="post">Сонгосон пост</param>
        /// <param name="currentUser">Одоогийн хэрэглэгч</param>
        /// <param name="userService">User service</param>
        public static void InteractWithPost(
            Post post,
            User currentUser,
            UserService userService)
        {
            while (true)
            {
                Console.WriteLine("\n--- POST ACTION ---");
                Console.WriteLine("1. Like");
                Console.WriteLine("2. Comment");
                Console.WriteLine("3. View Comments");
                Console.WriteLine("0. Back");
                Console.Write("Choose: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        post.Like(currentUser.Id);
                        Console.WriteLine("Liked!");
                        break;

                    case "2":
                        Console.Write("Write comment: ");
                        var text = Console.ReadLine();

                        post.AddComment(currentUser.Id, text);
                        Console.WriteLine("Comment added!");
                        break;

                    case "3":
                        ShowComments(post, userService);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}