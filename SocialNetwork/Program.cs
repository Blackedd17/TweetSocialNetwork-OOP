using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;
using TweetingPlatform.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Програмын эхлэх үндсэн class.
    /// Энэ class нь:
    /// - WinForms demo UI
    /// - Console social networking menu
    /// хоёрыг ажиллуулж болно.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method нь програмын эхлэх цэг.
        /// Эхлээд Form1 нээгдэнэ.
        /// Form хаагдсаны дараа console menu ажиллана.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ===== WINFORMS UI ЭХЛҮҮЛЭХ =====
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // ===== CONSOLE APP-Д АШИГЛАХ REPOSITORY, SERVICE-ҮҮД =====
            var userRepo = new InMemoryRepository<User>();
            var postRepo = new InMemoryRepository<Post>();

            var authService = new AuthService(userRepo);
            var userService = new UserService(userRepo);
            var postService = new PostService(postRepo);

            /// <summary>
            /// Одоогоор нэвтэрсэн хэрэглэгч.
            /// null байвал хэрэглэгч нэвтрээгүй гэсэн үг.
            /// </summary>
            User currentUser = null;

            // Програмын үндсэн давталт
            while (true)
            {
                // =========================================================
                // 1) AUTH MENU — хэрэглэгч нэвтрээгүй үед ажиллана
                // =========================================================
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
                            RegisterUser(authService);
                            break;

                        case "2":
                            currentUser = LoginUser(authService, userService, postService);
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }

                // =========================================================
                // 2) MAIN MENU — хэрэглэгч нэвтэрсний дараа ажиллана
                // =========================================================
                Console.WriteLine("\n==== MAIN MENU ====");
                Console.WriteLine("1. Create Tweet");
                Console.WriteLine("2. Show All Tweets");
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
                        CreatePost(currentUser, postService);
                        break;

                    case "2":
                        PostViewer.ShowPosts(postService.GetAllPosts(), userService);
                        break;

                    case "3":
                        SearchUsers(currentUser, userService, postService);
                        break;

                    case "4":
                        ViewFollowing(currentUser, userService, postService);
                        break;

                    case "5":
                        ViewFollowers(currentUser, userService, postService);
                        break;

                    case "6":
                        ProfileViewer.OpenProfile(currentUser, currentUser, userService, postService);
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

        /// <summary>
        /// Шинэ хэрэглэгч бүртгэх method.
        /// Username, display name, age, password авна.
        /// </summary>
        private static void RegisterUser(AuthService authService)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Display Name: ");
            var displayName = Console.ReadLine();

            Console.Write("Age: ");
            if (!byte.TryParse(Console.ReadLine(), out byte age))
            {
                Console.WriteLine("Invalid age.");
                return;
            }

            Console.Write("Password: ");
            var password = ConsoleHelper.ReadPassword();

            try
            {
                authService.Register(username, displayName, age, password);
                Console.WriteLine("Registered successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Register failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Хэрэглэгчийг username, password ашиглан нэвтрүүлнэ.
        /// Амжилттай бол User object буцаана.
        /// </summary>
        private static User LoginUser(AuthService authService, UserService userService, PostService postService)
        {
            Console.Write("Username: ");
            var loginUser = Console.ReadLine();

            Console.Write("Password: ");
            var loginPass = ConsoleHelper.ReadPassword();

            var loggedIn = authService.Login(loginUser, loginPass);
            if (loggedIn == null)
            {
                Console.WriteLine("Login failed.");
                return null;
            }

            Console.WriteLine("Logged in as: " + loggedIn.Username);
            PostViewer.ShowFeed(loggedIn, userService, postService);

            return loggedIn;
        }

        /// <summary>
        /// Одоогийн хэрэглэгч шинэ пост үүсгэнэ.
        /// </summary>
        private static void CreatePost(User currentUser, PostService postService)
        {
            Console.Write("Tweet content: ");
            var text = Console.ReadLine();

            var post = new TextPost(currentUser.Id, text);
            postService.CreatePost(post);

            Console.WriteLine("Tweet created!");
        }

        /// <summary>
        /// Хэрэглэгч username эсвэл display name-ээр бусад хэрэглэгч хайна.
        /// Олдсон хэрэглэгчдийн profile-ийг нээх боломжтой.
        /// </summary>
        private static void SearchUsers(User currentUser, UserService userService, PostService postService)
        {
            Console.Write("Search username/display name: ");
            var keyword = Console.ReadLine();

            var foundUsers = userService.SearchUsers(keyword);

            if (foundUsers.Count == 0)
            {
                Console.WriteLine("No users found.");
                return;
            }

            ProfileViewer.OpenUserList(foundUsers, currentUser, userService, postService);
        }

        /// <summary>
        /// Одоогийн хэрэглэгчийн following жагсаалтыг харуулна.
        /// </summary>
        private static void ViewFollowing(User currentUser, UserService userService, PostService postService)
        {
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
                return;
            }

            ProfileViewer.OpenUserList(followingUsers, currentUser, userService, postService);
        }

        /// <summary>
        /// Одоогийн хэрэглэгчийн follower жагсаалтыг харуулна.
        /// </summary>
        private static void ViewFollowers(User currentUser, UserService userService, PostService postService)
        {
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
                return;
            }

            ProfileViewer.OpenUserList(followerUsers, currentUser, userService, postService);
        }
    }
}