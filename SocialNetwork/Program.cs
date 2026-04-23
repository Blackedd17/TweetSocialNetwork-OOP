using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;
using Social.Infrastructure.Data;
using Social.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TweetingPlatform.Helpers;

namespace TweetingPlatform
{
    /// <summary>
    /// Програмын үндсэн entry point.
    /// 
    /// Энэ class нь:
    /// - SQLite database initialize хийх
    /// - Repository, Service layer-ийг үүсгэх
    /// - WinForms UI болон Console UI-г ажиллуулах
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main method нь програмын эхлэх цэг.
        /// Эхлээд WinForms UI ажиллана.
        /// Дараа нь Console menu ажиллана.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SQLitePCL.Batteries.Init();
            /// <summary>
            /// SQLite database initialize
            /// </summary>
            var db = new SqliteDbContext();
            db.ResetDatabase();

            /// <summary>
            /// Repository layer
            /// </summary>
            IRepository<User> userRepo = new SQLiteUserRepository(db);
            IPostRepository postRepo = new SQLitePostRepository(db);
            ICommentRepository commentRepo = new SQLiteCommentRepository(db);
            IReactionRepository reactionRepo = new SQLiteReactionRepository(db);

            /// <summary>
            /// Service layer
            /// </summary>
            AuthService authService = new AuthService(userRepo);
            UserService userService = new UserService(userRepo);
            PostService postService = new PostService(postRepo);

            // ===== WINFORMS UI =====
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            /// <summary>
            /// Одоогийн нэвтэрсэн хэрэглэгч
            /// </summary>
            User currentUser = null;

            // ===== CONSOLE LOOP =====
            while (true)
            {
                // ================= AUTH MENU =================
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

                // ================= MAIN MENU =================
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
        /// Шинэ хэрэглэгч бүртгэнэ.
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
        /// Login хийх.
        /// </summary>
        private static User LoginUser(AuthService authService, UserService userService, PostService postService)
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();

            Console.Write("Password: ");
            var password = ConsoleHelper.ReadPassword();

            var user = authService.Login(username, password);

            if (user == null)
            {
                Console.WriteLine("Login failed.");
                return null;
            }

            Console.WriteLine("Logged in as: " + user.Username);

            /// <summary>
            /// Feed харуулах
            /// </summary>
            PostViewer.ShowFeed(user, userService, postService);

            return user;
        }

        /// <summary>
        /// Post үүсгэнэ.
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
        /// Хэрэглэгч хайх.
        /// </summary>
        private static void SearchUsers(User currentUser, UserService userService, PostService postService)
        {
            Console.Write("Search: ");
            var keyword = Console.ReadLine();

            var users = userService.SearchUsers(keyword);

            if (users.Count == 0)
            {
                Console.WriteLine("No users found.");
                return;
            }

            ProfileViewer.OpenUserList(users, currentUser, userService, postService);
        }

        /// <summary>
        /// Following list харуулах.
        /// </summary>
        private static void ViewFollowing(User currentUser, UserService userService, PostService postService)
        {
            var list = new List<User>();

            foreach (var id in currentUser.Following)
            {
                var user = userService.GetById(id);
                if (user != null)
                    list.Add(user);
            }

            if (list.Count == 0)
            {
                Console.WriteLine("No following users.");
                return;
            }

            ProfileViewer.OpenUserList(list, currentUser, userService, postService);
        }

        /// <summary>
        /// Followers list харуулах.
        /// </summary>
        private static void ViewFollowers(User currentUser, UserService userService, PostService postService)
        {
            var list = new List<User>();

            foreach (var id in currentUser.Followers)
            {
                var user = userService.GetById(id);
                if (user != null)
                    list.Add(user);
            }

            if (list.Count == 0)
            {
                Console.WriteLine("No followers.");
                return;
            }

            ProfileViewer.OpenUserList(list, currentUser, userService, postService);
        }
    }
}