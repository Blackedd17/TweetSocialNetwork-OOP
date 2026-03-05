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

            var u1 = new User("anar", "Anar", 21);
            userService.CreateUser(u1);

            var t1 = new Tweet(u1.Id, "My first tweet!");
            tweetService.CreateTweet(t1);

            t1.Like(u1.Id);
            t1.AddComment(u1.Id, "Nice!");

            Console.WriteLine("Users: " + userService.GetUsers().Count);
            Console.WriteLine("Tweets: " + tweetService.GetAllTweets().Count);
            Console.WriteLine("Likes: " + t1.LikeCount);
            Console.WriteLine("Comments: " + t1.Comments.Count);
        }
    }
}
