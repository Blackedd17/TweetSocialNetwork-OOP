using Social.Core.Entities;
using Social.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TweetingPlatform.Helpers
{
    /// <summary>
    /// Хэрэглэгчийн profile болон user list-ийг консолоор харуулах helper класс.
    ///
    /// Энэ класс нь:
    /// - Бүх хэрэглэгчийн жагсаалт харуулах (OpenUserList)
    /// - Нэг хэрэглэгчийн profile харах (OpenProfile)
    /// - Тухайн хэрэглэгчийн постуудыг үзэх (OpenUserPosts)
    ///
    /// Мөн:
    /// - Follow / Unfollow хийх
    /// - Постууд дээр interaction хийх (like, comment)
    /// боломжийг олгоно.
    /// </summary>
    public class ProfileViewer
    {
        /// <summary>
        /// Хэрэглэгчдийн жагсаалтыг харуулж, сонгосон хэрэглэгчийн profile руу орно.
        /// </summary>
        /// <param name="users">Бүх хэрэглэгчдийн жагсаалт</param>
        /// <param name="currentUser">Одоогийн хэрэглэгч</param>
        /// <param name="userService">User service</param>
        /// <param name="postService">Post service</param>
        public static void OpenUserList(
            List<User> users,
            User currentUser,
            UserService userService,
            PostService postService)
        {
            while (true)
            {
                Console.WriteLine("\n==== USER LIST ====");

                for (int i = 0; i < users.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {users[i].DisplayName} (@{users[i].Username})");
                }

                Console.WriteLine("0. Back");
                Console.Write("Choose user: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    continue;

                if (choice == 0) return;

                if (choice > 0 && choice <= users.Count)
                {
                    OpenProfile(users[choice - 1], currentUser, userService, postService);
                }
            }
        }

        /// <summary>
        /// Нэг хэрэглэгчийн profile-г нээж дараах үйлдлүүдийг хийх боломж олгоно:
        /// - Follow / Unfollow
        /// - Тухайн хэрэглэгчийн постуудыг харах
        /// </summary>
        /// <param name="profileUser">Profile эзэмшигч</param>
        /// <param name="currentUser">Одоогийн хэрэглэгч</param>
        /// <param name="userService">User service</param>
        /// <param name="postService">Post service</param>
        public static void OpenProfile(
            User profileUser,
            User currentUser,
            UserService userService,
            PostService postService)
        {
            while (true)
            {
                Console.WriteLine("\n==== PROFILE ====");
                Console.WriteLine($"Name: {profileUser.DisplayName}");
                Console.WriteLine($"Username: @{profileUser.Username}");
                Console.WriteLine($"Followers: {profileUser.Followers.Count}");
                Console.WriteLine($"Following: {profileUser.Following.Count}");

                bool isFollowing = currentUser.Following.Contains(profileUser.Id);

                Console.WriteLine("\n1. " + (isFollowing ? "Unfollow" : "Follow"));
                Console.WriteLine("2. View Tweets");
                Console.WriteLine("0. Back");
                Console.Write("Choose: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (profileUser.Id == currentUser.Id)
                        {
                            Console.WriteLine("You cannot follow yourself.");
                            break;
                        }

                        if (isFollowing)
                        {
                            currentUser.Unfollow(profileUser.Id);
                            profileUser.Followers.Remove(currentUser.Id);
                            Console.WriteLine("Unfollowed.");
                        }
                        else
                        {
                            currentUser.Follow(profileUser.Id);
                            profileUser.Followers.Add(currentUser.Id);
                            Console.WriteLine("Followed!");
                        }
                        break;

                    case "2":
                        OpenUserPosts(profileUser, currentUser, userService, postService);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        /// <summary>
        /// Тухайн хэрэглэгчийн постуудыг харуулж,
        /// сонгосон пост дээр interaction хийх боломж олгоно.
        /// </summary>
        /// <param name="profileUser">Profile эзэмшигч</param>
        /// <param name="currentUser">Одоогийн хэрэглэгч</param>
        /// <param name="userService">User service</param>
        /// <param name="postService">Post service</param>
        private static void OpenUserPosts(
            User profileUser,
            User currentUser,
            UserService userService,
            PostService postService)
        {
            var posts = postService
                .GetAllPosts()
                .Where(p => p.AuthorId == profileUser.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            if (posts.Count == 0)
            {
                Console.WriteLine("No tweets.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\n==== USER TWEETS ====");

                for (int i = 0; i < posts.Count; i++)
                {
                    Console.WriteLine(
                        $"{i + 1}. {posts[i].Content} | Likes:{posts[i].LikeCount} | Comments:{posts[i].Comments.Count}"
                    );
                }

                Console.WriteLine("0. Back");
                Console.Write("Choose tweet: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    continue;

                if (choice == 0) return;

                if (choice > 0 && choice <= posts.Count)
                {
                    PostViewer.InteractWithPost(posts[choice - 1], currentUser, userService);
                }
            }
        }
    }
}