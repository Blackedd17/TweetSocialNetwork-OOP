using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Core.Entities;
using Social.Core.Repositories;

namespace Social.Core.Services
{
    public class TweetService
    {
        private readonly IRepository<Tweet> tweetRepository;
        public TweetService(IRepository<Tweet> tweetRepository)
        {
            this.tweetRepository = tweetRepository;
        }
        public void CreateTweet(Tweet tweet)
        {
            tweetRepository.Add(tweet);
        }
        public List<Tweet> GetAllTweets()
        {
            return tweetRepository.GetAll();
        }
        public List<Tweet> GetTweetsByAuthor(Guid authorId)
        {
            return tweetRepository.GetAll()
                .Where(t => t.AuthorId == authorId)
                .ToList();
        }
    }
}
