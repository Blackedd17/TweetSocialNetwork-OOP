using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;

namespace TweetingPlatform
{
    internal class Program
    {
        static void Main()
        {
            var userRepo = new InMemoryRepository<User>();
            var postRepo = new InMemoryRepository<Post>();

            var authService = new AuthService(userRepo);
            var userService = new UserService(userRepo);
            var postService = new PostService(postRepo);

            User currentUser = null;

            while (true)
            {
                // 1) Auth menu (login/register) — logged out үед
                while (currentUser == null)
                {
                    Console.WriteLine("\n==== AUTH MENU ====");
                    Console.WriteLine("1. Register");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("0. Exit");
                    Console.Write("Choose: ");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.Write("Username: ");
                            var username = Console.ReadLine();

                            Console.Write("Display Name: ");
                            var displayName = Console.ReadLine();

                            Console.Write("Age: ");
                            if (!byte.TryParse(Console.ReadLine(), out byte age))
                            {
                                Console.WriteLine("Invalid age.");
                                break;
                            }

                            Console.Write("Password: ");
                            var password = ReadPassword();

                            try
                            {
                                authService.Register(username, displayName, age, password);
                                Console.WriteLine("Registered successfully!");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Register failed: " + ex.Message);
                            }
                            break;

                        case "2":
                            Console.Write("Username: ");
                            var loginUser = Console.ReadLine();

                            Console.Write("Password: ");
                            var loginPass = ReadPassword();

                            var loggedIn = authService.Login(loginUser, loginPass);
                            if (loggedIn == null)
                            {
                                Console.WriteLine("Login failed.");
                            }
                            else
                            {
                                currentUser = loggedIn;
                                Console.WriteLine("Logged in as: " + currentUser.Username);
                            }
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }

                // 2) Main menu — logged in үед
                Console.WriteLine("\n==== MAIN MENU ====");
                Console.WriteLine("1. Create Tweet");
                Console.WriteLine("2. Show Tweets");
                Console.WriteLine("3. Like Tweets");
                Console.WriteLine("4. Comment Tweets");
                Console.WriteLine("5. Follow User");
                Console.WriteLine("6. Logout");
                Console.Write("Choose: ");

                var mainChoice = Console.ReadLine();

                switch (mainChoice)
                {
                    case "1":
                        Console.Write("Tweet content: ");
                        var text = Console.ReadLine();

                        var post = new Post(currentUser.Id, text);
                        postService.CreatePost(post);

                        Console.WriteLine("Tweet posted!");
                        break;

                    case "2":
                        var posts = postService.GetAllPosts();
                        if (posts.Count == 0)
                        {
                            Console.WriteLine("No tweets yet.");
                            break;
                        }

                        for (int i = 0; i < posts.Count; i++)
                        {
                            Console.WriteLine($"{i}. {posts[i].Content} | Likes:{posts[i].LikeCount} | Comments:{posts[i].Comments.Count}");
                        }
                        break;

                    case "3":
                        var postsForLike = postService.GetAllPosts();
                        if (postsForLike.Count == 0)
                        {
                            Console.WriteLine("No tweets available.");
                            break;
                        }

                        for (int i = 0; i < postsForLike.Count; i++)
                        {
                            Console.WriteLine($"{i}. {postsForLike[i].Content} | Likes:{postsForLike[i].LikeCount}");
                        }

                        Console.Write("Tweet index: ");
                        if (!int.TryParse(Console.ReadLine(), out int likeIndex) ||
                            likeIndex < 0 || likeIndex >= postsForLike.Count)
                        {
                            Console.WriteLine("Invalid index.");
                            break;
                        }

                        postsForLike[likeIndex].Like(currentUser.Id);
                        Console.WriteLine("Tweet liked!");
                        break;

                    case "4":
                        var postsForComment = postService.GetAllPosts();
                        if (postsForComment.Count == 0)
                        {
                            Console.WriteLine("No tweets available.");
                            break;
                        }

                        for (int i = 0; i < postsForComment.Count; i++)
                        {
                            Console.WriteLine($"{i}. {postsForComment[i].Content} | Comments:{postsForComment[i].Comments.Count}");
                        }

                        Console.Write("Tweet index: ");
                        if (!int.TryParse(Console.ReadLine(), out int commentIndex) ||
                            commentIndex < 0 || commentIndex >= postsForComment.Count)
                        {
                            Console.WriteLine("Invalid index.");
                            break;
                        }

                        Console.Write("Comment: ");
                        var commentText = Console.ReadLine();

                        postsForComment[commentIndex].AddComment(currentUser.Id, commentText);
                        Console.WriteLine("Comment added!");
                        break;

                    case "5":
                        var allUsers = userService.GetUsers();
                        if (allUsers.Count == 0)
                        {
                            Console.WriteLine("No users available.");
                            break;
                        }

                        Console.WriteLine("Users:");
                        for (int i = 0; i < allUsers.Count; i++)
                        {
                            Console.WriteLine($"{i}. {allUsers[i].Username} ({allUsers[i].DisplayName})");
                        }

                        Console.Write("User index to follow: ");
                        if (!int.TryParse(Console.ReadLine(), out int followIndex) ||
                            followIndex < 0 || followIndex >= allUsers.Count)
                        {
                            Console.WriteLine("Invalid index.");
                            break;
                        }

                        var target = allUsers[followIndex];
                        userService.Follow(currentUser, target);
                        Console.WriteLine("Now following: " + target.Username);
                        break;

                    case "6":
                        currentUser = null;
                        Console.WriteLine("Logged out.");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }

            return password;
        }
    }
}
