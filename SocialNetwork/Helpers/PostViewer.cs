using Social.Core.Entities;
using Social.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetingPlatform.Helpers
{
    public class PostViewer
    {
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
                string authorName = author != null ? author.DisplayName : "Unknown";

                Console.WriteLine($"{i + 1}. [{authorName}] {posts[i].Content} | Likes:{posts[i].LikeCount} | Comments:{posts[i].Comments.Count}");
            }
        }
        public static void ShowFeed(User currentUser, UserService userService, PostService postService)
        {
            Console.WriteLine("\n==== YOUR FEED ====");

            var allPosts = postService.GetAllPosts();

            var feedPosts = allPosts.OrderByDescending(p => currentUser.Following.Contains(p.AuthorId)).ThenByDescending(p => p.CreatedAt).ToList();

            if (feedPosts.Count == 0)
            {
                Console.WriteLine("Your feed is empty.");
                return;
            }

            for (int i = 0; i < feedPosts.Count; i++)
            {
                var author = userService.GetById(feedPosts[i].AuthorId);
                string authorName = author != null ? author.DisplayName : "Unknown";

                Console.WriteLine($"{i + 1}. [{authorName}] {feedPosts[i].Content} | Likes:{feedPosts[i].LikeCount} | Comments:{feedPosts[i].Comments.Count}");
            }
        }
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
                string commenterName = commentUser != null ? commentUser.DisplayName : "Unknown";

                Console.WriteLine($"{i + 1}. [{commenterName}] {post.Comments[i].Text}");
            }
        }
    }
}
