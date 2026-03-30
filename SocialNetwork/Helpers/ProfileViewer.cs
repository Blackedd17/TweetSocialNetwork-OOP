using Social.Core.Entities;
using Social.Core.Services;
using System;
using System.Collections.Generic;

namespace TweetingPlatform.Helpers
{
    public class ProfileViewer
    {
        public static void OpenUserList(List<User> users, User currentUser, UserService userService, PostService postService)
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
        public static void OpenProfile(User profileUser, User currentUser, UserService userService, PostService postService)
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

                Console.WriteLine("\nTweets:");
                if (posts.Count == 0)
                {
                    Console.WriteLine("No tweets yet.");
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

                    Console.WriteLine("2. Like a tweet");
                    Console.WriteLine("3. Comment on a tweet");
                    Console.WriteLine("4. View comments");
                    Console.WriteLine("0. Back");
                }
                else
                {
                    Console.WriteLine("1. Delete my tweet");
                    Console.WriteLine("2. Like my tweet");
                    Console.WriteLine("3. Comment on my tweet");
                    Console.WriteLine("4. View comments");
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
                                Console.WriteLine("No tweets to like.");
                                break;
                            }

                            Console.Write("Tweet number: ");
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
                            Console.WriteLine("Tweet liked.");
                            break;

                        case "3":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No tweets to comment.");
                                break;
                            }

                            Console.Write("Tweet number: ");
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

                        case "4":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No tweets available.");
                                break;
                            }

                            Console.Write("Tweet number: ");
                            if (!int.TryParse(Console.ReadLine(), out int viewCommentIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (viewCommentIndex < 1 || viewCommentIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            PostViewer.ShowComments(posts[viewCommentIndex - 1], userService);
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

                            Console.Write("Tweet number: ");
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
                                Console.WriteLine("No tweets to like.");
                                break;
                            }

                            Console.Write("Tweet number: ");
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
                            Console.WriteLine("Tweet liked.");
                            break;

                        case "3":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No posts to comment.");
                                break;
                            }

                            Console.Write("Tweet number: ");
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

                        case "4":
                            if (posts.Count == 0)
                            {
                                Console.WriteLine("No tweets available.");
                                break;
                            }

                            Console.Write("Tweet number: ");
                            if (!int.TryParse(Console.ReadLine(), out int myViewCommentIndex))
                            {
                                Console.WriteLine("Invalid input.");
                                break;
                            }

                            if (myViewCommentIndex < 1 || myViewCommentIndex > posts.Count)
                            {
                                Console.WriteLine("Invalid index.");
                                break;
                            }

                            PostViewer.ShowComments(posts[myViewCommentIndex - 1], userService);
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
            }
        }
    }
}
