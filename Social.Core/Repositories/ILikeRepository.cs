using System;

namespace Social.Core.Repositories
{
    public interface ILikeRepository
    {
        void AddLike(Guid postId, Guid userId);
        void RemoveLike(Guid postId, Guid userId);
        int GetLikeCount(Guid postId);
        bool HasLiked(Guid postId, Guid userId);
    }
}