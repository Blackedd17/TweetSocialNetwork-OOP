using System;

namespace Social.Core.Interfaces
{
    public interface ILikeable
    {
        int LikeCount { get; }
        void Like(Guid userId);
    }
}
