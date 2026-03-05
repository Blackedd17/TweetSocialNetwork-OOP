using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Core.Entities;
using Social.Core.Repositories;
using Social.Core.Services;

namespace SocialNetwork
{
    internal class Program
    {
        static void Main()
        {
            var userRepo = new InMemoryRepository<User>();
            var tweetRepo = new InMemoryRepository<Tweet>();

            var authService = new AuthService(userRepo);
            var tweetService = new TweetService(tweetRepo);
            User currentUser = null;

            while (true)
            {
                Console.WriteLine("\n==== MENU ====");
                Console.WriteLine("1. Register (Create User)");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Logout");
                Console.WriteLine("4. Create Tweet");
                Console.WriteLine("5. Show Tweets");
                Console.WriteLine("6. Like Tweet");
                Console.WriteLine("7. Comment Tweet");
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
                        var age = byte.Parse(Console.ReadLine());

                        Console.Write("Password: ");
                        var password = ReadPassword();

                        try
                        {
                            var registered = authService.Register(username, displayName, age, password);
                            Console.WriteLine("Registered: " + registered.Username);
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
                            Console.WriteLine("Login failed (wrong username or password).");
                        }
                        else
                        {
                            currentUser = loggedIn;
                            Console.WriteLine("Logged in as: " + currentUser.Username);
                        }
                        break;

                    case "3":
                        currentUser = null;
                        Console.WriteLine("Logged out.");
                        break;

                    case "4":
                        if (currentUser == null)
                        {
                            Console.WriteLine("Please login first.");
                            break;
                        }

                        Console.Write("Tweet content: ");
                        var text = Console.ReadLine();

                        var tweet = new Tweet(currentUser.Id, text);
                        tweetService.CreateTweet(tweet);

                        Console.WriteLine("Tweet posted!");
                        break;

                    case "5":
                        var tweets = tweetService.GetAllTweets();
                        for (int i = 0; i < tweets.Count; i++)
                        {
                            Console.WriteLine($"{i}. {tweets[i].Content} | Likes:{tweets[i].LikeCount} | Comments:{tweets[i].Comments.Count}");
                        }
                        break;

                    case "0":
                        return;

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
