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

            var userService = new UserService(userRepo);
            var tweetService = new TweetService(tweetRepo);

            while (true)
            {
                Console.WriteLine("\n1. Create User");
                Console.WriteLine("2. Create Tweet");
                Console.WriteLine("3. Show Tweets");
                Console.WriteLine("4. Like Tweet");
                Console.WriteLine("5. Comment Tweet");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Username: ");
                        var username = Console.ReadLine();

                        Console.Write("Display Name: ");
                        var display = Console.ReadLine();

                        Console.Write("Age: ");
                        var age = byte.Parse(Console.ReadLine());

                        var user = new User(username, display, age);
                        userService.CreateUser(user);

                        Console.WriteLine("User created!");
                        break;

                    case "2":
                        var users = userService.GetUsers();
                        if (users.Count == 0)
                        {
                            Console.WriteLine("Create a user first.");
                            break;
                        }

                        var user2 = users[0];

                        Console.Write("Tweet content: ");
                        var text = Console.ReadLine();

                        var tweet = new Tweet(user2.Id, text);
                        tweetService.CreateTweet(tweet);

                        Console.WriteLine("Tweet posted!");
                        break;

                    case "3":
                        var tweets = tweetService.GetAllTweets();

                        int i = 0;
                        foreach (var t in tweets)
                        {
                            Console.WriteLine($"{i}. {t.Content} | Likes:{t.LikeCount} | Comments:{t.Comments.Count}");
                            i++;
                        }
                        break;

                    case "4":
                        var tweetsForLike = tweetService.GetAllTweets();
                        if (tweetsForLike.Count == 0)
                        {
                            Console.WriteLine("No tweets available.");
                            break;
                        }

                        Console.Write("Tweet index: ");
                        int likeIndex = int.Parse(Console.ReadLine());

                        var likeUser = userService.GetUsers()[0];
                        tweetsForLike[likeIndex].Like(likeUser.Id);

                        Console.WriteLine("Liked!");
                        break;

                    case "5":
                        var tweetsForComment = tweetService.GetAllTweets();
                        if (tweetsForComment.Count == 0)
                        {
                            Console.WriteLine("No tweets available.");
                            break;
                        }

                        Console.Write("Tweet index: ");
                        int commentIndex = int.Parse(Console.ReadLine());

                        Console.Write("Comment: ");
                        var commentText = Console.ReadLine();

                        var commentUser = userService.GetUsers()[0];
                        tweetsForComment[commentIndex].AddComment(commentUser.Id, commentText);

                        Console.WriteLine("Comment added!");
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
