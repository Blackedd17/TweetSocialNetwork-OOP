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
                Console.WriteLine("1. Create Post");
                Console.WriteLine("2. Show All Posts");
                Console.WriteLine("3. Search User");
                Console.WriteLine("4. View Following");
                Console.WriteLine("5. View Followers");
                Console.WriteLine("6. My Profile");
                Console.WriteLine("7. Logout");
                Console.Write("Choose: ");

                var mainChoice = Console.ReadLine();

                switch (mainChoice)
                {
                    case "1":
                        Console.Write("Post content: ");
                        var text = Console.ReadLine();

                        var post = new TextPost(currentUser.Id, text);
                        postService.CreatePost(post);

                        Console.WriteLine("Post created!");
                        break;

                    case "2":
                        ShowPosts(postService.GetAllPosts(), userService);
                        break;

                    case "3":
                        Console.Write("Search username/display name: ");
                        var keyword = Console.ReadLine();

                        var foundUsers = userService.SearchUsers(keyword);

                        if (foundUsers.Count == 0)
                        {
                            Console.WriteLine("No users found.");
                            break;
                        }

                        OpenUserList(foundUsers, currentUser, userService, postService);
                        break;

                    case "4":
                        var followingUsers = new List<User>();
                        foreach (var id in currentUser.Following)
                        {
                            var user = userService.GetById(id);
                            if (user != null)
                            {
                                followingUsers.Add(user);
                            }
                        }

                        if (followingUsers.Count == 0)
                        {
                            Console.WriteLine("You are not following anyone.");
                            break;
                        }

                        OpenUserList(followingUsers, currentUser, userService, postService);
                        break;

                    case "5":
                        var followerUsers = new List<User>();
                        foreach (var id in currentUser.Followers)
                        {
                            var user = userService.GetById(id);
                            if (user != null)
                            {
                                followerUsers.Add(user);
                            }
                        }

                        if (followerUsers.Count == 0)
                        {
                            Console.WriteLine("No followers yet.");
                            break;
                        }

                        OpenUserList(followerUsers, currentUser, userService, postService);
                        break;

                    case "6":
                        OpenProfile(currentUser, currentUser, userService, postService);
                        break;

                    case "7":
                        currentUser = null;
                        Console.WriteLine("Logged out.");
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        static void OpenUserList(List<User> users, User currentUser, UserService userService, PostService postService)
        {
            Console.WriteLine();
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {users[i].Username} ({users[i].DisplayName})");
            }

            Console.Write("Choose user number (0 to back): ");
            if (!int.TryParse(Console.ReadLine(), out int userIndex))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            if (userIndex == 0) return;

            if (userIndex < 1 || userIndex > users.Count)
            {
                Console.WriteLine("Invalid index.");
                return;
            }

            var selectedUser = users[userIndex - 1];
            OpenProfile(selectedUser, currentUser, userService, postService);
        }

        static void OpenProfile(User profileUser, User currentUser, UserService userService, PostService postService)
        {
            while (true)
            {
                Console.WriteLine("\n==== PROFILE ====");
                Console.WriteLine("Username: " + profileUser.Username);
                Console.WriteLine("Display Name: " + profileUser.DisplayName);
                Console.WriteLine("Age: " + profileUser.Age);
                Console.WriteLine("Followers: " + profileUser.Followers.Count);
                Console.WriteLine("Following: " + profileUser.Following.Count);

                var posts = postService.GetPostsByAuthor(profileUser.Id);

                Console.WriteLine("\nPosts:");
                if (posts.Count == 0)
                {
                    Console.WriteLine("No posts yet.");
                }
                else
                {
                    for (int i = 0; i < posts.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {posts[i].Content} | Likes:{posts[i].LikeCount} | Comments:{posts[i].Comments.Count}");
                    }
                }

                Console.WriteLine("\nOptions:");
                if (profileUser.Id != currentUser.Id)
                {
                    if (userService.IsFollowing(currentUser, profileUser))
                    {
                        Console.WriteLine("1. Unfollow");
                    }
                    else
                    {
                        Console.WriteLine("1. Follow");
                    }

                    Console.WriteLine("2. Like a post");
                    Console.WriteLine("3. Comment on a post");
                    Console.WriteLine("0. Back");
                }
                else
                {
                    Console.WriteLine("1. Delete my post");
                    Console.WriteLine("2. Like my post");
                    Console.WriteLine("3. Comment on my post");
                    Console.WriteLine("0. Back");
                }

                Console.Write("Choose: ");
                var choice = Console.ReadLine();

                if (choice == "0")
                {
                    break;
                }

                if (profileUser.Id != currentUser.Id)
                {
                    switch (choice)
                    {
                        case "1":
                            if (userService.IsFollowing(currentUser, profileUser))
                            {
                                userService.Unfollow(currentUser, profileUser);
                                Console.WriteLine("Unfollowed.");
                            }
                            else
                            {
                                userService.Follow(currentUser, profileUser);
                                Console.WriteLine("Followed.");
                            }
                            break;

                        case "2":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No posts to like.");
                                break;
                            }

                            Console.Write("Post number: ");
                            if (!int.TryParse(Console.ReadLine(), out int likeIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (likeIndex < 1 || likeIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            posts[likeIndex - 1].Like(currentUser.Id);
                            Console.WriteLine("Post liked.");
                            break;

                        case "3":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No posts to comment.");
                                break;
                            }

                            Console.Write("Post number: ");
                            if (!int.TryParse(Console.ReadLine(), out int commentIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (commentIndex < 1 || commentIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            Console.Write("Comment: ");
                            var commentText = Console.ReadLine();

                            posts[commentIndex - 1].AddComment(currentUser.Id, commentText);
                            Console.WriteLine("Comment added.");
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case "1":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No posts to delete.");
                                break;
                            }

                            Console.Write("Post number: ");
                            if (!int.TryParse(Console.ReadLine(), out int deleteIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (deleteIndex < 1 || deleteIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            postService.DeletePost(posts[deleteIndex - 1]);
                            Console.WriteLine("Post deleted.");
                            break;

                        case "2":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No posts to like.");
                                break;
                            }

                            Console.Write("Post number: ");
                            if (!int.TryParse(Console.ReadLine(), out int myLikeIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (myLikeIndex < 1 || myLikeIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            posts[myLikeIndex - 1].Like(currentUser.Id);
                            Console.WriteLine("Post liked.");
                            break;

                        case "3":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No posts to comment.");
                                break;
                            }

                            Console.Write("Post number: ");
                            if (!int.TryParse(Console.ReadLine(), out int myCommentIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (myCommentIndex < 1 || myCommentIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            Console.Write("Comment: ");
                            var myCommentText = Console.ReadLine();

                            posts[myCommentIndex - 1].AddComment(currentUser.Id, myCommentText);
                            Console.WriteLine("Comment added.");
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
            }
        }

        static void ShowPosts(List<Post> posts, UserService userService)
        {
            Console.WriteLine("\n==== POSTS ====");
            if (posts.Count == 0)
            {
                Console.WriteLine("No posts yet.");
                return;
            }

            for (int i = 0; i < posts.Count; i++)
            {
                var author = userService.GetById(posts[i].AuthorId);
                string authorName = author != null ? author.Username : "Unknown";

                Console.WriteLine($"{i + 1}. [{authorName}] {posts[i].Content} | Likes:{posts[i].LikeCount} | Comments:{posts[i].Comments.Count}");
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